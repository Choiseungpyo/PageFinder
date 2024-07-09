using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField]
    private Slider hpBar;

    // ���� �ʱ� ��ǥ
    public Vector3 originalPos;

    public enum PosType
    {
        GROUND,
        SKY
    }

    public enum MoveType
    {
        PATH, // ��� �̵�
        RANDOM, // ���� �̵�
        TRACE, // ���� �̵�
        FIX // ����
    }

    public enum AttackType
    {
        PREEMPTIVE, // ���� ���� (�������� ��������)
        SUSTAINEDPREEMPTIVE, // ���� ���� ���� (�������� �ٱ�����)
        AVOIDANCE, // ȸ��
        GUARD // ��ȣ 
    }

    public enum AttackRangeType
    {
        SHORT,
        LONG
    }


    [SerializeField] // ������ : ����, ����
    protected PosType posType = PosType.GROUND; 
    [SerializeField] // �ൿ ���� : ����̵�, �����̵�, �����̵�, ����
    protected MoveType moveType = MoveType.RANDOM; 
    [SerializeField] // ���� ���� : ����, ���� ����, ȸ��, ��ȣ
    protected AttackType attackType = AttackType.PREEMPTIVE;
    [SerializeField] // ���� ���� : �ٰŸ�, ���Ÿ�
    protected AttackRangeType attackRangeType = AttackRangeType.SHORT;

    protected MeshRenderer meshRenderer;
    // ���ʹ��� ��� ����
    protected bool isDie = false;

    public override float HP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            hpBar.value = currHP;
            //Debug.Log(name + " : " + HP);
            if (currHP <= 0)
            {
                Die();
            }
        }
    }
    public override void Start()
    {
        base.Start();

        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}
