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
        STUN,
        MOVE,
        ATTACK,
        DIE
    }

    protected enum IdleState
    {
        NONE,
        DEFAULT
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

    protected enum StateEffect
    {
        NONE,
        STUN,
        KNOCKBACK,
        BINDING,
        AIR,
    }

    [Header("State")]
    protected State state = State.IDLE; // ���ʹ��� ���� ����
    protected IdleState idleState; // ���ʹ��� ���� ����
    protected MoveState moveState;
    protected AttackState attackState;
    protected StateEffect stateEffect;

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
    protected Vector3 stateEffectPos = Vector3.zero;

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


    // ������Ʈ
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected Player playerScr;
    protected TokenManager tokenManager;
    protected NavMeshAgent agent;
    protected Exp exp;
    protected MeshRenderer meshRenderer;
    protected Rigidbody rb;

    // ���ʹ��� ��� ����
    protected bool isDie = false;

    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;
            hpBar.SetCurrValueUI(MAXHP);
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

    public void SetStateEffect(string stateEffectName, float time, Vector3 pos)
    {
        switch (stateEffectName)
        {
            case "Stun":
                state = State.STUN;
                stateEffect = StateEffect.STUN;
                break;
            case "KnockBack":
                state = State.STUN;
                stateEffect = StateEffect.KNOCKBACK;
                stateEffectPos = pos;
                Debug.Log("KnockBack" + pos);
                break;
            case "Binding":
                stateEffect = StateEffect.BINDING;
                break;
            case "Air":
                state = State.STUN;
                stateEffect = StateEffect.AIR;
                stateEffectPos = pos;
                Debug.Log("Air" + pos);
                break;
        }

        maxAbnormalTime = time;
        currAbnormalTime = maxAbnormalTime;
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
        rb = GetComponent<Rigidbody>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        // �� ����
        isDie = false;

        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        currentPosIndexToMove = 0;
        agent.stoppingDistance = 0;

        stateEffect = StateEffect.NONE;

        currFindCoolTime = maxFindCoolTime;
    }
}
