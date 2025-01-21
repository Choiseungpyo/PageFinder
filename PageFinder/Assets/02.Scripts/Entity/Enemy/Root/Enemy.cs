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

    #region enum
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
        ABNORMAL,
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
        TRACE,
        ROTATE
    }

    protected enum AttackState
    {
        NONE,
        ATTACKWAIT,
        DEFAULT, //DEFAULT Attack
        REINFORCEMENT, //REINFORCEMENT Attack
        SKILL
    }

    // CircleRange ��ũ��Ʈ���� �����ϱ� ������ Public ex)
    public enum AbnomralState
    {
        NONE,
        STUN,
        KNOCKBACK,
        BINDING,
        AIR,
    }

    #endregion

    #region Variables
    [Header("State")]
    protected State state = State.IDLE; // ���ʹ��� ���� ����
    protected IdleState idleState; // ���ʹ��� ���� ����
    protected MoveState moveState;
    protected AttackState attackState;
    protected AbnomralState abnormalState;

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
    private int moveDist = 0;

    [Header("Stun")]

    // ����
    protected float oriAttackSpeed = 1;
    protected float currAttackSpeed = 1;

    // ������Ʈ
    protected Transform enemyTr;
    protected GameObject playerObj;

    // ���ش� ����: player -> playerState
    //protected Player playerScr;
    protected PlayerState playerState;
    protected NavMeshAgent agent;

    protected Rigidbody rb;

    // ���ʹ��� ��� ����
    protected bool isDie = false;

    #endregion

    public virtual int DefaultAtkPercent
    {
        get { return defaultAtkPercent; }
        set
        {
            currHP = value;
        }
    }

    public int MoveDist
    {
        get { return moveDist; }
        set
        {
            moveDist = value;
        }
    }

    public virtual float OriAttackSpeed
    {
        get
        {
            return oriAttackSpeed;
        }
        set
        {
            oriAttackSpeed = value;
        }
    }

    public virtual float CurrAttackSpeed
    {
        get
        {
            return currAttackSpeed;
        }
        set
        {
            currAttackSpeed = value;
        }
    }

    public IEnumerator ChangeAttackSpeed(float time, float value)
    {
        yield return new WaitForSeconds(time);
        CurrAttackSpeed = value;
    }

    public IEnumerator ChangeMoveSpeed(float time, float value)
    {
        yield return new WaitForSeconds(time);
        CurrAttackSpeed = value;
    }

    /// <summary>
    /// ���� �̻��� ����
    /// </summary>
    /// <param name="stateEffectName"></param>
    /// <param name="time"></param>
    /// <param name="pos"></param>
    public void SetStateEffect(AbnomralState type, float time, Vector3 pos)
    {
        state = State.ABNORMAL;
        switch (type)
        {
            case AbnomralState.STUN:
                abnormalState = AbnomralState.STUN;
                break;

            case AbnomralState.KNOCKBACK:
                abnormalState = AbnomralState.KNOCKBACK;
                break;

            case AbnomralState.BINDING:
                abnormalState = AbnomralState.BINDING;
                break;

            case AbnomralState.AIR:
                abnormalState = AbnomralState.AIR;
                break;

            default:
                abnormalState = AbnomralState.NONE;
                break;
        }

        stateEffectPos = pos;
        maxAbnormalTime = time;
        currAbnormalTime = maxAbnormalTime;
    }


    public override void Start()
    {
        enemyTr = DebugUtils.GetComponentWithErrorLogging<Transform>(transform, "Transform");
        playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        //playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
        agent = DebugUtils.GetComponentWithErrorLogging<NavMeshAgent>(gameObject, "NavMeshAgent");
        rb = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(gameObject, "Rigidbody");

        // �� ����
        isDie = false;
        posToMove[0] = transform.position;
        posToMove[1] = posToMove[0] + transform.TransformDirection(Vector3.forward) * moveDist; //posToMove[0] + transform.TransformDirection(Vector3.forward) * moveDist

        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        currentPosIndexToMove = 1;
        agent.stoppingDistance = 0;

        currHP = maxHP;

        // Slider Bar
        hpBar = GetComponentInChildren<SliderBar>();
        shieldBar = GetComponentInChildren<ShieldBar>();
        hpBar.SetMaxValueUI(maxHP);
        hpBar.SetCurrValueUI(currHP);
        //MaxShield = 0;

        abnormalState = AbnomralState.NONE;

        currFindCoolTime = maxFindCoolTime;


        
        agent.acceleration = 1000f; // ���� �׻� �ִ� �ӵ�(agent.speed)�� �̵��ϵ��� ����
        agent.angularSpeed = 360f; // �÷��̾��� �̼ӿ� ������� �ٷ� ȸ���� �� �ֵ��� ����
    }

    public void SetStatus(BattlePage page, int index)
    {
        moveDist = page.moveDist[index];
        maxHP = page.maxHP[index];
        atk = page.atk[index];
        currAttackSpeed = page.atkSpeed[index]; 
    }
}
