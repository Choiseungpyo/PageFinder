using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MediumBossEnemy : HighEnemy, IShield
{
    [Header("Reinforcement Default Attack")]

    [SerializeField]
    protected GameObject ReinforcementAttack_Prefab;

    [SerializeField]
    protected int maxDefaultAtkCnt = 4;
    protected int currDefaultAtkCnt = 0;

    [Header("Shield")]
    [SerializeField]
    private Gradation grdation;
    [SerializeField]
    private ShieldBar shieldBar;

    float maxShield;
    float currShield;

    public override float HP
    {
        get 
        { 
            return currHP + currShield;  // 100 + 50   - 55
        }
        set
        {
            // ���ҽ��ѵ� ���尡 �����ִ� ���
            if(value > currHP)
            {
                CurrShield = value - currHP;
            }
            else // ���ҽ��ѵ� ���尡 �������� ���� ���
            {
                CurrShield = 0;
                currHP = value;
            }

            Hit();
            hpBar.SetCurrValueUI(currHP);
            
            if (currHP <= 0)
            {
                // <�ؾ��� ó��>

                // �÷��̾� ����ġ ȹ��
                // ��ū ���� 
                isDie = true;
                Die();
            }
        }
    }

    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;

            hpBar.SetMaxValueUI(maxHP);
            grdation.SetGradation(maxHP);
        }
    }

    #region ���� ����

    public float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            // �ǵ带 ������ ���

            maxShield = value;
            hpBar.SetMaxValueUI(maxHP + maxShield);
            
            grdation.SetGradation(maxHP + maxShield);

            
            shieldBar.SetMaxShieldValueUI(maxHP, currHP, maxShield);
            CurrShield = maxShield;
        }
    }

    public float CurrShield
    {
        get
        {
            return currShield;
        }
        set
        {
            currShield = value;

            shieldBar.SetCurrValueUI(currShield);

            // ���带 �� ������� ���
            if (currShield <= 0)
            {
                currShield = 0;
                grdation.SetGradation(maxHP);
            }
        }
    }

    #endregion


    public override void Start()
    {
        AddAnivariableNames("isReinforcementAttack");

        base.Start();
        currDefaultAtkCnt = 0;

        grdation.SetGradation(maxHP);

        MaxShield = 50;
    }

    #region ���� ���� �Լ�

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        // ����� ��ų�� �ִ� ���
        if (attackValue >= 0)
            attackState = AttackState.SKILL;
        else if (attackValue == -1)
            attackState = AttackState.DEFAULT;
        else if (attackValue == -2)
            attackState = AttackState.REINFORCEMENT;
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

            case AttackState.REINFORCEMENT:
                ReinforcementAttackAction();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    private void ReinforcementAttackAction()
    {
        agent.isStopped = true;
    }

    #endregion

    #region ��Ÿ�� ���� �Լ�

    protected override void SetAllCoolTime()
    {
        if (state == State.IDLE)
        {
            if (idleState == IdleState.DEFAULT)
                SetCurrFindCoolTime();
        }
        else if (state == State.MOVE)
        {
            if (moveState == MoveState.TRACE)
                SetAttackCooltime();
        }
        else if (state == State.ATTACK)
        {
            if (attackState == AttackState.ATTACKWAIT || attackState == AttackState.DEFAULT || attackState == AttackState.REINFORCEMENT)
                SetAttackCooltime();
        }
    }

    #endregion

    #region ��ȭ �⺻ ���� ���� �Լ�

    /// <summary>
    /// ��ȭ ���� �ִϸ��̼� Events���� ȣ���ϴ� �Լ�
    /// </summary>
    protected virtual void ReinforcementAttack()
    {
        // ��ȭ �⺻ ������ �⺻ ���� ��Ÿ���� ������ �� + �⺻ ���� Ƚ���� N��°�� �� �����Ѵ�.
    }

    protected override int CheckIfThereAreAnySkillsAvailable()
    {
        /* <�� ��� �� ��ƾ>
         *  �ϱ� : skillCoolTime.Count == 0 => ���� false
         *  ��� : ��ų 1�� => �ش� ��ų ��Ÿ�� üũ => 0�̸� true �ƴϸ� false
         *  �߰����� : ��ų 2�� => ��Ȳ�� ���� � ��ų�� ������� üũ 
         */

        /// �⺻ ������ �ϰ� �ִ� ���� ���
        if (attackState == AttackState.DEFAULT || attackState == AttackState.REINFORCEMENT)
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
    protected override int GetTypeOfAttackToUse()
    {
        int skillIndexToUse = -1;

        // �켱������ ���� ������� ��Ÿ���� ���� ��ų�� �ε��� Ȯ��
        skillIndexToUse = CheckIfThereAreAnySkillsAvailable();

        // ����� ��ų�� �ִ� ���
        if (skillIndexToUse >= 0)
            return skillIndexToUse;

        // �⺻ ���� ��Ÿ���� ���� ���
        if (currDefaultAtkCoolTime <= 0)
        {
            if (currDefaultAtkCnt == maxDefaultAtkCnt)
                return -2;
            return -1;
        }

        return -3;
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

            case AttackState.REINFORCEMENT:
                SetAniVariableValue("isAttack", "isReinforcementAttack");
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected override void DefaultAttackAniEnd()
    {
        base.DefaultAttackAniEnd();

        currDefaultAtkCnt++;
    }

    /// <summary>
    /// ��ȭ ���� �ִϸ��̼��� ������ �� ȣ���ϴ� �Լ�
    /// </summary>
    private void ReinforcementAttackAniEnd()
    {
        currDefaultAtkCnt = 0;
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
    }

    #endregion
}
