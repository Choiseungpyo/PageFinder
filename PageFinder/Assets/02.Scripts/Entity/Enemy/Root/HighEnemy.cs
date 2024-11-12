using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighEnemy : EnemyAction
{
    [Header("Skill")]
    [SerializeField]
    protected string[] skillNames;
    [SerializeField]
    protected bool[] moveSkillTypeData;
    [SerializeField]
    protected float[] maxSkillCoolTimes; // ��ų ��Ÿ�� - �ν����� â���� ���� 
    protected List<float> currSkillCoolTimes = new List<float>(); // ���� ��ų ��Ÿ�� 
    [SerializeField]
    protected float[] skillPriority; // ��ų �켱��
    [SerializeField]
    protected string currSkillName = "";
    protected List<bool> skillCondition = new List<bool>(); // ��ų ����

    public override void Start()
    {
        AddAnivariableNames("isSkill");

        base.Start();
        
        for (int i = 0; i < maxSkillCoolTimes.Length; i++)
        {
            currSkillCoolTimes.Add(maxSkillCoolTimes[i]);
            skillCondition.Add(false);
        }

        // ��ų ���� ������ ���� ������ �ߴ��� üũ
        if (!(maxSkillCoolTimes.Length == currSkillCoolTimes.Count
            && maxSkillCoolTimes.Length == skillNames.Length
            && maxSkillCoolTimes.Length == skillPriority.Length)
            && maxSkillCoolTimes.Length == skillCondition.Count)
        {
            Debug.LogError("maxSkillCoolTimes : " + maxSkillCoolTimes.Length);
            Debug.LogError("currSkillCoolTimes : " + currSkillCoolTimes.Count);
            Debug.LogError("skillNames : " + skillNames.Length);
            Debug.LogError("skillPriority : " + skillPriority.Length);
            Debug.LogError("skillCondition : " + skillCondition.Count);
        }
    }

    #region State ���� �Լ�

    protected override void SetRootState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        // �����̻��� ���
        if (state == State.STUN)
            return;

        // ��ų�� �����ϰ� �ִ� ��� Attack - SkillN �ִϸ��̼��� �����ϵ��� �ϱ� ����
        if (!currSkillName.Equals(""))
            return;

        if (distance <= atkDist)
        {
            // ���� �÷��̾� �տ� �ִ� ���
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else
                state = State.MOVE;
        }
        else if (distance <= cognitiveDist)
        {
            if (stateEffect == StateEffect.BINDING)
                state = State.IDLE;
            else
                state = State.MOVE;
        }
        else // ���� ���� �ٱ��� ���
        {
            if (stateEffect == StateEffect.BINDING)
            {
                state = State.IDLE;
                return;
            }

            // ���� �������� �÷��̾ �������� ��
            if (attackPattern == AttackPattern.SUSTAINEDPREEMPTIVE && playerRecognitionStatue)
            {
                state = State.MOVE;
                return;
            }

            if (currFindCoolTime > 0)
                state = State.IDLE;
            else
                state = State.MOVE;
        }
    }

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        // ����� ��ų�� �ִ� ���
        if (attackValue >= 0)
            attackState = AttackState.SKILL;
        else if(attackValue == -1)
            attackState = AttackState.DEFAULT;
        else
            attackState = AttackState.ATTACKWAIT;
    }

    #endregion

    #region �׼� ���� �Լ�

    protected override void AttackAction()
    {
        switch (attackState)
        {
            case AttackState.ATTACKWAIT:
                break;

            case AttackState.DEFAULT:
                DefaultAttackAction();
                break;
            case AttackState.SKILL:
                SkillAction();
                break;
            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected void SkillAction()
    {
        agent.isStopped = true;
    }

    #endregion

    #region �ð� ���� �Լ�

    protected override void SetAttackCooltime()
    {
        SetCurrDefaultAtkCoolTime();
        SetCurrSkillCoolTime();
        CheckSkillsCondition();
    }

    /// <summary>
    /// ���� ��ų ��Ÿ���� �����Ѵ�.
    /// </summary>
    private void ResetCurrSkillCoolTime()
    {
        for (int i = 0; i < skillNames.Length; i++)
        {
            if (currSkillName.Equals(skillNames[i]))
            {
                if (currSkillName.Equals("Skill2"))
                {
                    maxSkillCoolTimes[i] = 200;
                    Debug.Log("�ִ밪 ���� : " + maxSkillCoolTimes[i]);
                }
                   

                currSkillCoolTimes[i] = maxSkillCoolTimes[i];
                Debug.Log(currSkillName + currSkillCoolTimes[i]);
                break;
            }
        }
    }

    #endregion

    #region ��ų ���� �Լ�

    protected virtual int CheckIfThereAreAnySkillsAvailable()
    {
        /* <�� ��� �� ��ƾ>
         *  �ϱ� : skillCoolTime.Count == 0 => ���� false
         *  ��� : ��ų 1�� => �ش� ��ų ��Ÿ�� üũ => 0�̸� true �ƴϸ� false
         *  �߰����� : ��ų 2�� => ��Ȳ�� ���� � ��ų�� ������� üũ 
         */

        /// �⺻ ������ �ϰ� �ִ� ���� ���
        if (attackState == AttackState.DEFAULT)
            return -1;

        // ��ų �켱���� ���� ��
        for (int priority = 0; priority < skillNames.Length; priority++)
        {
            // �켱 ������ ���� ��ų ������� ��Ÿ���� ���Ҵ��� üũ : ���ڰ� ���� ���� �켱 ������ ����
            for (int indexToCheck = 0; indexToCheck < skillPriority.Length; indexToCheck++)
            {
                // �켱�� 
                if (skillPriority[indexToCheck] != priority)
                    continue;

                // �ش� ��ų ��Ÿ�Ӱ� ��� ���� üũ
                if (currSkillCoolTimes[indexToCheck] <= 0 && skillCondition[indexToCheck])
                {
                    currSkillName = skillNames[indexToCheck];
                    return indexToCheck;
                }
                break;
            }
        }

        return -1;
    }

    /// <summary>
    /// ���ݰ� ��ų ����� �����Ѵ�. 
    /// </summary>
    /// <returns> -2 :���� ��� X    -1 :Attack     0~n :Skill N </returns>
    protected virtual int GetTypeOfAttackToUse()
    {
        int skillIndexToUse = -1;

        // �켱������ ���� ������� ��Ÿ���� ���� ��ų�� �ε��� Ȯ��
        skillIndexToUse = CheckIfThereAreAnySkillsAvailable();

        // ����� ��ų�� �ִ� ���
        if (skillIndexToUse >= 0)
            return skillIndexToUse;

        // �⺻ ���� ��Ÿ���� ���� ���
        if (currDefaultAtkCoolTime <= 0)
            return -1;

        return -2;
    }

    private void SetCurrSkillCoolTime()
    {
        for (int i = 0; i < currSkillCoolTimes.Count; i++)
        {
            if (currSkillName.Equals(skillNames[i])) // �ش� ��ų ������� ����
                continue;

            if (currSkillCoolTimes[i] < 0)
                continue;

            currSkillCoolTimes[i] -= Time.deltaTime;
        }
    }

    protected virtual void CheckSkillsCondition()
    {
        if(stateEffect == StateEffect.BINDING)
        {
            for(int i=0; i<skillCondition.Count; i++)
            {
                // ������ ��ų�� ���
                if (moveSkillTypeData[i])
                {
                    skillCondition[i] = false;
                    break;
                }
            }
        }
    }

    #endregion

    #region �ִϸ��̼� ���� �Լ�

    protected override void AttackAni()
    {
        switch (attackState)
        {
            case AttackState.ATTACKWAIT:
                SetAniVariableValue("isAttack", "isAttackWait");
                break;

            case AttackState.DEFAULT:
                SetAniVariableValue("isAttack", "isDefaultAttack");
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected virtual void SkillAni()
    {
        SetAniVariableValue("isAttack", "isSkill");

        if(!currSkillName.Equals(""))
        {
            for(int i=0; i<skillNames.Length; i++)
            {
                if (currSkillName.Equals(skillNames[i]))
                    ani.SetBool(skillNames[i], true); // ��ų �ε����� ���� string�� �����ϵ��� �����ϱ�
                else
                    ani.SetBool(skillNames[i], false);
            }
        }
    }

    /// <summary>
    /// Skill �ִϸ��̼��� ������ ȣ��Ǵ� �Լ� (Inspector - Events)
    /// </summary>
    protected void SkillAniEnd()
    {
        // ��ų ����
        for (int i = 0; i < skillNames.Length; i++)
        {
            // ���� ����� ��ų 
            if (currSkillName.Equals(skillNames[i]))
            {
                if(rank == Rank.MEDIUMBOSS)
                    skillCondition[i] = false;
                break;
            }
        }

        // ��ų ��Ÿ��
        ResetCurrSkillCoolTime();
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;

        // �ִϸ��̼�
        for (int i = 0; i < skillNames.Length; i++)
        {
            ani.SetBool(skillNames[i], false);
        }

        // ���� ��ų �̸�
        currSkillName = "";
    }

    #endregion


    /// <summary>
    /// ���� ���ظ� ���� �� �÷��̾� �ʿ��� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    protected override void Hit()
    {
        // ���, ����, �߰����� ���ʹ� �÷��̾�� ���ݴ����� �� ������ �ɸ��� �ʱ� ������ ����д�.
    }
}
