using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class MediumBossEnemy : HighEnemy
{
    [Header("Reinforcement Default Attack")]

    [SerializeField]
    protected GameObject ReinforcementAttack_Prefab;

    [SerializeField]
    protected int maxDefaultAtkCnt = 4;
    protected int currDefaultAtkCnt = 0;

    public override void Start()
    {
        base.Start();

        currDefaultAtkCnt = 0;
    }

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

    private void ReinforcementAttackAction()
    {
        agent.isStopped = true;
    }

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

            case AttackState.REINFORCEMENT:
                ReinforcementAttackAni();
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

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

    protected override void DefaultAttackAniEnd()
    {
        base.DefaultAttackAniEnd();

        currDefaultAtkCnt++;
    }


    #region �ִϸ��̼� ���� �Լ�

    protected override void DefaultIdleAni()
    {
        base.DefaultIdleAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void FindAni()
    {
        base.FindAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void TraceAni()
    {
        base.TraceAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void AttackWaitAni()
    {
        base.AttackWaitAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void DefaultAttackAni()
    {
        base.DefaultAttackAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void SkillAni()
    {
        base.SkillAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void StunAni()
    {
        base.StunAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void DieAni()
    {
        base.DieAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected virtual void ReinforcementAttackAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", true);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
        ani.SetBool("isReinforcementAttack", true);
        ani.SetBool("isSkill", false);
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
