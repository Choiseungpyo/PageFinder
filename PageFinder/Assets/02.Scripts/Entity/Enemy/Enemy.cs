using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Enemy : Entity
{
    public enum PosType
    {
        GROUND,
        SKY
    }

    public enum MoveType
    {
        PATH, // ��� �̵�
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

    [SerializeField] // ������ : ����, ����
    protected PosType posType = PosType.GROUND;
    [SerializeField] // �ൿ ���� : ����̵�, �����̵�, ����
    protected MoveType moveType = MoveType.PATH; 
    [SerializeField] // ���� ���� : ����, ���� ����, ȸ��, ��ȣ
    protected AttackType attackType = AttackType.PREEMPTIVE;

    [SerializeField]
    private Slider hpBar;

    // ���� �ʱ� ��ǥ
    public Vector3 originalPos;


    [SerializeField]
    protected int defaultAtkPercent = 100; // �⺻ ���� ���� �ۼ�Ʈ
    [SerializeField]
    protected float stunTime = 0.2f; // ���� �ð�

    // �⺻ ����
    protected float maxDefaultAtkCoolTime = 2f;
    protected float currDefaultAtkCoolTime = 0;

    // ��ų
    [SerializeField]
    protected List<float> maxSkillCoolTimes = new List<float>(); // ��ų ��Ÿ�� - �ν����� â���� ���� 
    protected List<float> currSkillCoolTimes = new List<float>(); // ���� ��ų ��Ÿ�� 
    protected List<bool> skillUsageStatus =  new List<bool>();



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
                // �ؾ��� ó�� 
                // �÷��̾� ����ġ ȹ��
                // ��ū ���� 
                Die();
            }
        }
    }

    public virtual int DefaultAtkPercent
    {
        get { return defaultAtkPercent; }
        set
        {
            currHP = value;
        }
    }

    public virtual float MaxDefaultAtkCoolTime
    {
        get { return maxDefaultAtkCoolTime; }
        set
        {
            maxDefaultAtkCoolTime = value;
        }
    }

    public virtual float CurrDefaultAtkCoolTime
    {
        get { return currDefaultAtkCoolTime; }
        set
        {
            currDefaultAtkCoolTime = value;
        }
    }

    public override void Start()
    {
        base.Start();

        isDie = false;

        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        for (int i = 0; i < maxSkillCoolTimes.Count; i++)
            currSkillCoolTimes.Add(maxSkillCoolTimes[i]);

        for (int i = 0; i < maxSkillCoolTimes.Count; i++)
            skillUsageStatus.Add(false);

        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}
