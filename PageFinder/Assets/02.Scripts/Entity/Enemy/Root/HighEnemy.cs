using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighEnemy : EnemyAction
{
    [Header("Skill")]
    [SerializeField]
    protected List<string> skillNames = new List<string>();
    [SerializeField]
    protected List<float> maxSkillCoolTimes = new List<float>(); // ��ų ��Ÿ�� - �ν����� â���� ���� 
    protected List<float> currSkillCoolTimes = new List<float>(); // ���� ��ų ��Ÿ�� 
    [SerializeField]
    protected List<int> skillPriority = new List<int>(); // ��ų �켱��
    [SerializeField]
    protected string currSkillName = "";
    protected List<bool> skillCondition = new List<bool>(); // ��ų ����

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < maxSkillCoolTimes.Count; i++)
        {
            currSkillCoolTimes.Add(maxSkillCoolTimes[i]);
            skillCondition.Add(false);
        }

        // ��ų ���� ������ ���� ������ �ߴ��� üũ
        if (!(maxSkillCoolTimes.Count == currSkillCoolTimes.Count
            && maxSkillCoolTimes.Count == skillNames.Count
            && maxSkillCoolTimes.Count == skillPriority.Count)
            && maxSkillCoolTimes.Count == skillCondition.Count)
        {
            Debug.LogError("maxSkillCoolTimes : " + maxSkillCoolTimes.Count);
            Debug.LogError("currSkillCoolTimes : " + currSkillCoolTimes.Count);
            Debug.LogError("skillNames : " + skillNames.Count);
            Debug.LogError("skillPriority : " + skillPriority.Count);
            Debug.LogError("skillCondition : " + skillCondition.Count);
        }
    }


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


    #region State ���� �Լ�

    protected override void SetAllState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        // ��ų�� �����ϰ� �ִ� ��� Attack - SkillN �ִϸ��̼��� �����ϵ��� �ϱ� ����
        if (!currSkillName.Equals(""))
            return;

        if (distance <= atkDist)
        {
            // ���� �÷��̾� �տ� �ִ� ���
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else
            {
                if (skillCondition[0])
                    return;

                state = State.MOVE;
            }
                

        }
        else if (distance <= cognitiveDist)
            state = State.MOVE;
        else // ���� ���� �ٱ��� ���
        {
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


    protected override void SetAttackCooltime()
    {
        SetCurrDefaultAtkCoolTime();
        SetCurrSkillCoolTime();
        CheckSkillsCondition();
    }

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
        for (int priority = 0; priority < skillNames.Count; priority++)
        {
            // �켱 ������ ���� ��ų ������� ��Ÿ���� ���Ҵ��� üũ
            for (int indexToCheck = 0; indexToCheck < skillPriority.Count; indexToCheck++)
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

    }

    /// <summary>
    /// ���� ��ų ��Ÿ���� �����Ѵ�.
    /// </summary>
    private void ResetCurrSkillCoolTime()
    {
        for (int i = 0; i < skillNames.Count; i++)
        {
            if (currSkillName.Equals(skillNames[i]))
            {
                currSkillCoolTimes[i] = maxSkillCoolTimes[i];
                break;
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
                AttackWaitAni();
                break;

            case AttackState.DEFAULT:
                DefaultAttackAni();
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected override void DefaultIdleAni()
    {
        base.DefaultIdleAni();

        ani.SetBool("isSkill", false);
    }

    protected override void FindAni()
    {
        base.FindAni();

        ani.SetBool("isSkill", false);
    }

    protected override void TraceAni()
    {
        base.TraceAni();

        ani.SetBool("isSkill", false);
    }

    protected override void AttackWaitAni()
    {
        base.AttackWaitAni();
        ani.SetBool("isSkill", false);
    }

    protected override void DefaultAttackAni()
    {
        base.DefaultAttackAni();
        ani.SetBool("isSkill", false);
    }

    protected override void StunAni()
    {
        base.StunAni();
        ani.SetBool("isSkill", false);
    }

    protected override void DieAni()
    {
        base.DieAni();
        ani.SetBool("isSkill", false);
    }

    protected virtual void SkillAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", true);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);
        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
        ani.SetBool("isSkill", true);

        if(!currSkillName.Equals(""))
        {
            for(int i=0; i<skillNames.Count; i++)
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
        for (int i = 0; i < skillNames.Count; i++)
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
        for (int i = 0; i < skillNames.Count; i++)
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
