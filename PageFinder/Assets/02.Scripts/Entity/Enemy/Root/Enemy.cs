using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Enemy : Entity
{
    protected enum PosType
    {
        GROUND,
        SKY
    }

    protected enum FindPattern
    {
        PATH, // ��� �̵�
        FIX // ����
    }

    protected enum AttackPattern
    {
        PREEMPTIVE, // ���� ���� (�������� ��������)
        SUSTAINEDPREEMPTIVE, // ���� ���� ���� (�������� �ٱ�����)
        AVOIDANCE, // ȸ��
        GUARD // ��ȣ 
    }

    protected enum Rank
    { 
        LOW,
        HIGH,
        MEDIUMBOSS,
        FINALBOSS
    }


    protected enum State
    {
        IDLE,
        ABONORMAL,
        MOVE,
        ATTACK,
        DIE
    }

    protected enum IdleState
    {
        NONE,
        DEFAULT
    }

    protected enum AbnormalState
    {
        NONE,
        STUN,
        BINDING
    }

    protected enum MoveState
    {
        NONE,
        FIND,
        TRACE
    }

    protected enum AttackState
    {
        NONE,
        ATTACKWAIT,
        DEFAULT,
        REINFORCEMENT,
        SKILL
    }

    [Header("State")]
    
    [SerializeField]
    protected State state = State.IDLE; // ���ʹ��� ���� ����
    [SerializeField]
    protected IdleState idleState; // ���ʹ��� ���� ����
    [SerializeField]
    protected AbnormalState abnormalState;
    [SerializeField]
    protected MoveState moveState;
    [SerializeField]
    protected AttackState attackState;
    [SerializeField] // ������ : ����, ����
    protected PosType posType = PosType.GROUND;
    protected Rank rank;

    [Header("Pattern")]
    [SerializeField] // �ൿ ���� : ����̵�, �����̵�, ����
    protected FindPattern findPattern = FindPattern.PATH; 
    [SerializeField] // ���� ���� : ����, ���� ����, ȸ��, ��ȣ
    protected AttackPattern attackPattern = AttackPattern.PREEMPTIVE;

    // ���� ���� �Լ�
    protected bool playerRecognitionStatue = false; // �÷��̾ �� ���̶� �����ߴ��� Ȯ���ϴ� ����

    [Header("Abnormal")]
    [SerializeField]
    protected float maxAbnormalTime;
    protected float currAbnormalTime = 0;

    [Header("Find")]
    [SerializeField]
    protected float maxFindCoolTime;
    protected float currFindCoolTime;


    [Header("DefaultAttack")]
    [SerializeField]
    protected int defaultAtkPercent = 100; // �⺻ ���� ���� �ۼ�Ʈ
    // �⺻ ����
    [SerializeField]
    protected float maxDefaultAtkCoolTime;
    protected float currDefaultAtkCoolTime;

    [Header("Dist")]
    [SerializeField] // ���� �����Ÿ�
    protected float atkDist = 2.0f;
    [SerializeField] // ���� �����Ÿ�
    protected float cognitiveDist = 10.0f;

    [Header("Move")]
    [SerializeField] // �̵� ��ġ
    protected Vector3[] posToMove = { Vector3.zero, Vector3.zero };
    protected int currentPosIndexToMove = 0;

    [Header("Stun")]
    [SerializeField]
    protected float stunTime = 0.2f; // ���� �ð�

    [SerializeField]
    protected Slider hpBar;

    // ������Ʈ
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected Player playerScr;
    protected TokenManager tokenManager;
    protected NavMeshAgent agent;
    protected Exp exp;
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

    public virtual int DefaultAtkPercent
    {
        get { return defaultAtkPercent; }
        set
        {
            currHP = value;
        }
    }

    public override void Start()
    {
        base.Start();

        enemyTr = GetComponent<Transform>();
        playerObj = GameObject.FindWithTag("PLAYER");
        playerScr = playerObj.GetComponent<Player>();
        exp = playerObj.GetComponent<Exp>();
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
        agent = GetComponent<NavMeshAgent>();
        enemyTr = GetComponent<Transform>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        // �� ����
        isDie = false;
        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        currentPosIndexToMove = 0;
        agent.stoppingDistance = 0;

        currFindCoolTime = maxFindCoolTime;

    }
}
