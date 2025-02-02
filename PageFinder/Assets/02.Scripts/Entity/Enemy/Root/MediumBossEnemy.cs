using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MediumBossEnemy : HighEnemy
{
    #region Variables
    [Header("Reinforcement Default Attack")]

    [SerializeField]
    protected GameObject ReinforcementAttack_Prefab;

    [SerializeField]
    protected int reinforcementAtkCnt; // ��ȭ ���� Ƚ��
    protected int currBasicAtkCnt; 

    [SerializeField]
    protected Gradation gradation;
    #endregion

    #region Init

    protected override void InitStat()
    {
        base.InitStat();

        currBasicAtkCnt = 0;
        reinforcementAtkCnt = 4;
    }

    #endregion

    #region State

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();
        Debug.Log($"������ ���� ���� : {attackValue}");
        switch (attackValue)
        {
            case -2:
                attackState = AttackState.REINFORCEMENT;
                break;

            case -1:
                attackState = AttackState.BASIC;
                break;

            case 0:
            case 1:
            case 2:
                attackState = AttackState.SKILL;
                break;
        }
        Debug.Log($"{attackValue}   {attackState}");
    }

    #endregion

    #region Reinforcement Attack
    /// <summary>
    /// ��ȭ ���� �ִϸ��̼� Events���� ȣ���ϴ� �Լ�
    /// </summary>
    protected virtual void ReinforcementAttack()
    {
        // ��ȭ �⺻ ������ �⺻ ���� ��Ÿ���� ������ �� + �⺻ ���� Ƚ���� N��°�� �� �����Ѵ�.
    }

    #endregion

    #region Skill

    protected override int GetTypeOfAttackToUse()
    {
        /* <�� ��� �� ��ƾ>
             *  �ϱ� : skillCoolTime.Count == 0 => ���� false
             *  ��� : ��ų 1�� => �ش� ��ų ��Ÿ�� üũ => 0�̸� true �ƴϸ� false
             *  �߰����� : ��ų 2�� => ��Ȳ�� ���� � ��ų�� ������� üũ 
             */

        // ��ų �켱���� ���� ��
        // �켱 ������ ���� ��ų ������� ��Ÿ���� ���Ҵ��� üũ : ���ڰ� ���� ���� �켱 ������ ����
        for (int priority = 0; priority < skillPriority.Count; priority++)
        {
            int skillNum = skillPriority[priority];

            // �ش� ��ų ��Ÿ�Ӱ� ��� ���� üũ
            if (currSkillCoolTimes[skillNum] <= 0 && skillConditions[skillNum])
            {
                currSkillNum = skillNum;
                return skillNum;
            }
        }

        // ��ȭ ������ ���
        if (currBasicAtkCnt == reinforcementAtkCnt)
            return -2;

        // �⺻ ����
        return -1;
    }

    #endregion

    #region Animation
    protected override void AttackAni()
    {
        switch (attackState)
        {
            case AttackState.BASIC:
                SetAniVariableValue(AttackState.BASIC);
                break;

            case AttackState.REINFORCEMENT:
                SetAniVariableValue(AttackState.REINFORCEMENT);
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
    /// �⺻ ������ ���� �� �ִϸ��̼� Event���� ȣ���ϴ� �Լ�
    /// </summary>
    protected override void BasicAttackEnd()
    {
        base.BasicAttackEnd();

        currBasicAtkCnt++;
    }

    /// <summary>
    /// ��ȭ ������ ���� �� �ִϸ��̼� Event���� ȣ���ϴ� �Լ�
    /// </summary>
    private void ReinforcementAttackEnd()
    {
        currBasicAtkCnt = 0;
        attackState = AttackState.NONE;
    }

    #endregion
}
