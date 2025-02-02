using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Playables;

public class Enemy : Entity
{

    #region enum
    public enum EnemyType 
    { 
        Jiruru,
        Bansha,
        Witched
    };

    public enum State
    {
        IDLE,
        MOVE,
        ATTACK,
        DEBUFF,
        DIE
    }

    public enum IdleState
    {
        NONE,
        FIRSTWAIT, // �� ó�� ���� ����(EX.STUN ����) : �ִϸ��̼� ���� �� Find�� �Ѿ
        PATROLWAIT,// ��� ���� �� ���, Stun ���� : �ִϸ��̼� ���� �� Find�� �Ѿ
        ATTACKWAIT, // Perceive -> attack �Ѿ �� 0.5�� ���� : �ִϸ��̼� ���� �� Attack(Basic, Reinforce, Skill)���� �Ѿ
    }

    public enum MoveState
    {
        NONE,
        PATROL,
        CHASE,
        ROTATE, // �����Ÿ��� �÷��̾ ������ �տ� ��� ȸ���ؾ��ϴ� ���
        RUN
    }

    public enum AttackState
    {
        NONE,
        BASIC, //BASIC Attack
        REINFORCEMENT, //REINFORCEMENT Attack
        SKILL
    }

    // CircleRange ��ũ��Ʈ���� �����ϱ� ������ Public ex)
    public enum DebuffState
    {
        NONE,
        STIFF,
        KNOCKBACK,
        BINDING,
        STUN,
    }

    public enum PosType
    {
        GROUND,
        SKY
    }

    public enum Rank
    {
        MINION,
        NORMAL,
        ELITE,
        BOSS
    }

    public enum Personality
    {
        STATIC,
        CHASER,
        PATROL
    }

    public enum PatrolType
    {
        PATH,
        FIX
    }

    public enum AttackDistType
    {
        SHORT,
        LONG
    }
    #endregion

    #region Variables
    protected EnemyType enemyType;

    // State
    public State state; // ���ʹ��� ���� ����
    public IdleState idleState;
    public MoveState moveState;
    public AttackState attackState;
    public DebuffState debuffState;

    protected PosType posType = PosType.GROUND;
    [SerializeField]
    protected Rank rank;
    [SerializeField]
    protected Personality personality;
    protected PatrolType patrolType;

    [SerializeField]
    protected AttackDistType attackDistType;

    //Idle
    protected float maxFirstWaitTime;
    protected float maxPatrolWaitTime;
    protected float maxAttackWaitTime;

    protected float currFirstWaitTime;
    protected float currPatrolWaitTime;
    protected float currAttackWaitTime;

    // Debuff
    protected float maxDebuffTime;
    protected float currDebuffTime;

    // Patrol
    protected Vector3 currDestination;
    protected List<Vector3> patrolDestinations = new List<Vector3>();
    protected int patrolDestinationIndex;

    // AttackSpeed
    protected float oriAttackSpeed;
    protected float currAttackSpeed;

    protected float cognitiveDist;

    protected bool didPerceive; // �� �������� ���� �����ߴ��� ����

    protected bool isDie;

    // Ink
    protected InkType inkType;
    protected int inkTypeResistance;

    // Stagger
    protected int staggerResistance;

    // Component
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected EnemyUI enemyUI;

    // ���ش� ����: player -> playerState
    //protected Player playerScr;
    protected PlayerState playerState;
    protected NavMeshAgent agent;
    protected Rigidbody rb;


    #endregion

    #region Properties
    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;
            enemyUI.SetMaxHPBarUI(maxHP);
        }
    }

    public override float HP
    {
        get
        {
            return currHP;
        }
        set
        {
            currHP = value;

            enemyUI.SetCurrHPBarUI(maxHP, currHP, currShield);

            if (currHP <= 0)
            {
                Die();
            }
        }
    }

    public override float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            // �ǵ带 ������ ���
            maxShield = value;
            enemyUI.SetMaxShieldUI(maxHP);
            CurrShield = maxShield;
        }
    }

    public override float CurrShield
    {
        get
        {
            return currShield;
        }
        set
        {
            currShield = value;

            // ���带 �� ������� ���
            if (currShield <= 0)
                currShield = 0;

            enemyUI.SetCurrShieldUI(maxHP, currHP, currShield);
        }
    }

    protected virtual float OriAttackSpeed
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

    protected virtual float CurrAttackSpeed
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

    protected int PatrolDestinationIndex
    {
        get
        {
            return patrolDestinationIndex;
        }
        set
        {
            if (value >= patrolDestinations.Count || value < 0)
                patrolDestinationIndex = 0;
            else
                patrolDestinationIndex = value;
        }
    }

    public bool IsDie
    {
        get { return isDie; }
        set { isDie = value; }
    }
    #endregion

    /// <summary>
    /// Debuff�� Hit
    /// </summary>
    /// <param name="inkType"></param>
    /// <param name="damage"></param>
    /// <param name="debuffState"></param>
    /// <param name="debuffTime"></param>
    /// <param name="knockBackDir"></param>
    public void Hit(InkType inkType, float damage, DebuffState debuffState, float debuffTime, Vector3 subjectPos = default)
    {
        float diff = 0.0f;

        // ��ũ ���� ����
        if (this.inkType == inkType)
            damage = damage - (damage * inkTypeResistance / 100.0f);

        damage = damage - def; // ���� ����

        // def�� �� Ŀ�� ȸ���Ǵ� ���
        if (damage < 0)
            damage = 0;

        // ���尡 �ִ� ���
        if (currShield > 0)
        {
            diff = currShield - damage;
            CurrShield -= damage;

            if (diff < 0)
                HP += diff;
        }
        else
            HP -= damage;

        enemyUI.StartCoroutine(enemyUI.DamagePopUp(inkType, damage));

        if (HP <= 0)
            return;

        if (debuffState != DebuffState.NONE)
            SetDebuff(debuffState, debuffTime, (enemyTr.position - subjectPos).normalized);
        else
            Debug.LogWarning(debuffState);
    }

    /// <summary>
    /// ��ũ�� Hit
    /// </summary>
    /// <param name="inkType"></param>
    /// <param name="damage"></param>
    public void Hit(InkType inkType, float damage)
    {
        float diff = 0.0f;

        // ��ũ ���� ����
        if (this.inkType == inkType)
            damage = damage - (damage * inkTypeResistance / 100.0f);

        damage = damage - def; // ���� ����

        // def�� �� Ŀ�� ȸ���Ǵ� ���
        if (damage < 0)
            damage = 0;

        // ���尡 �ִ� ���
        if (currShield > 0)
        {
            diff = currShield - damage;
            CurrShield -= damage;

            if (diff < 0)
                HP += diff;
        }
        else
            HP -= damage;

        enemyUI.StartCoroutine(enemyUI.DamagePopUp(inkType, damage));
    }

    public override void Start()
    {
        InitComponent();
        InitStat();
    }

    #region Init
    protected virtual void InitComponent()
    {
        enemyTr = DebugUtils.GetComponentWithErrorLogging<Transform>(transform, "Transform");
        playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        agent = DebugUtils.GetComponentWithErrorLogging<NavMeshAgent>(gameObject, "NavMeshAgent");
        rb = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(gameObject, "Rigidbody");
        enemyUI = DebugUtils.GetComponentWithErrorLogging<EnemyUI>(gameObject, "EnemyUI");
    }

    /// <summary>
    /// ������ �о�ͼ� �� �Ҵ�� ���
    /// </summary>
    /// <param name="enemyData"></param>
    public virtual void InitStat(EnemyData enemyData)
    {
        rank = enemyData.rank;
        posType = enemyData.posType;
        personality = enemyData.personality;
        patrolType = enemyData.patrolType;
        inkType = enemyData.inkType;


        maxHP = enemyData.hp;
        atk = enemyData.atk;
        def = enemyData.def;
        cognitiveDist = enemyData.cognitiveDist;
        inkTypeResistance = enemyData.inkTypeResistance;
        staggerResistance = enemyData.staggerResistance;
        

        oriAttackSpeed = enemyData.atkSpeed;
        originalMoveSpeed = enemyData.moveSpeed;
        maxFirstWaitTime = enemyData.firstWaitTime;
        maxAttackWaitTime = enemyData.attackWaitTime;

        //dropItem = 

        transform.rotation = Quaternion.Euler(enemyData.spawnDir);
        patrolDestinations = enemyData.destinations;
    }

    /// <summary>
    /// ������ �Է� �� �⺻���� ����
    /// </summary>
    protected virtual void InitStat()
    {
        Debug.Log("�⺻ �ʱ�ȭ");
        // rank
        // personality
        // ���ݿ� ���� ���� ����
        SetPersonality();

        maxDebuffTime = 0;
        currDebuffTime = maxDebuffTime;
        
        currAttackSpeed = oriAttackSpeed;
        
        patrolDestinationIndex = 0;
        agent.stoppingDistance = 0;
        transform.position = patrolDestinations[patrolDestinationIndex];
        PatrolDestinationIndex += 1;
        
        currDestination = patrolDestinations[patrolDestinationIndex];
        // ü�¿� ���� UI ����
        MAXHP = maxHP;
        HP = 100;
        MaxShield = 100;

        maxPatrolWaitTime = Random.Range(1, 2);

        currFirstWaitTime = maxFirstWaitTime;
        currPatrolWaitTime = maxPatrolWaitTime;
        currAttackWaitTime = maxAttackWaitTime;

        isDie = false;

        agent.acceleration = 1000f; // ���� �׻� �ִ� �ӵ�(agent.speed)�� �̵��ϵ��� ����
        agent.angularSpeed = 360f; // �÷��̾��� �̼ӿ� ������� �ٷ� ȸ���� �� �ֵ��� ����
    }

    #endregion

    #region Change Stat

    /// <summary>
    /// ������ ������ ��� �̿��ϴ� �Լ�
    /// </summary>
    /// <param name="time">���� �ð�</param>
    /// <param name="percentageToApply">������ �ۼ�������</param>
    /// <returns></returns>
    public IEnumerator ChangeAttackSpeed(float time, float percentageToApply)
    {
        CurrAttackSpeed = CurrAttackSpeed * percentageToApply;
        yield return new WaitForSeconds(time);
        CurrAttackSpeed = OriAttackSpeed;
    }

    /// <summary>
    /// �̼��� ������ ��� �̿��ϴ� �Լ�
    /// </summary>
    /// <param name="time">���� �ð�</param>
    /// <param name="percentageToApply">������ �ۼ�������</param>
    /// <returns></returns>
    public IEnumerator ChangeMoveSpeed(float time, float percentageToApply)
    {
        MoveSpeed = MoveSpeed * percentageToApply;
        yield return new WaitForSeconds(time);
        MoveSpeed = OriginalMoveSpeed;
    }

    #endregion

    #region Set DetailState

    /// <summary>
    /// ���� �̻��� ����
    /// </summary>
    /// <param name="stateEffectName"></param>
    /// <param name="time"></param>
    /// <param name="pos"></param>
    public void SetDebuff(DebuffState type, float time, Vector3 dir = default)
    {
        state = State.DEBUFF;
        idleState = IdleState.NONE;
        moveState = MoveState.NONE;
        attackState = AttackState.NONE;

        agent.destination = enemyTr.position;
        agent.isStopped = true;

        maxDebuffTime = time;
        currDebuffTime = maxDebuffTime;

        switch (type)
        {
            case DebuffState.STIFF:
                debuffState = DebuffState.STIFF;
                break;

            case DebuffState.KNOCKBACK:
                debuffState = DebuffState.KNOCKBACK;

                // �ڷ�ƾ�� �������� �� ��� ó���� ������ �����غ���
                StartCoroutine(KnockBack(enemyTr.position + dir));
                break;

            case DebuffState.BINDING:
                debuffState = DebuffState.BINDING;
                break;

            case DebuffState.STUN:
                debuffState = DebuffState.STUN;
                break;

            default:
                debuffState = DebuffState.NONE;
                break;
        }
        //Debug.Log($"Enemy Debuff : {type} - {time}��");
    }

    private void SetPersonality()
    {
        switch(personality)
        {
            case Personality.STATIC:
                state = State.IDLE;
                idleState = IdleState.FIRSTWAIT;
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                didPerceive = false;
                break;

            case Personality.CHASER:
                state = State.MOVE;
                idleState = IdleState.NONE;
                moveState = MoveState.CHASE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                didPerceive = true;
                break;

            // ���� ���� ���� ����
            //case Personality.PATROL:
            //    

            //    state = State.IDLE;
            //    idleState = IdleState.FIRSTWAIT;
            //    moveState = MoveState.NONE;
            //    attackState = AttackState.NONE;
            //    debuffState = DebuffState.NONE;

            //    didPerceive = false;
            //    break;

            default:
                Debug.LogWarning(personality);
                break;

        }
    }

    #endregion

    private IEnumerator KnockBack(Vector3 targetPos)
    {
        // n�� ���� �ش� �������� �̵��� �� �ֵ��� �ؾ� ��
        agent.enabled = false;
        //Debug.Log("�˹� �ڷ�ƾ ����");

        //Debug.Log($"���� �� ��ġ : {enemyTr.position}");
        //Debug.Log($"��ǥ �� ��ġ : {targetPos}");
        float dist = Vector3.Distance(enemyTr.position, targetPos);
        while (dist >= 0.1f)
        {
            transform.position = Vector3.MoveTowards(enemyTr.position, targetPos, Time.deltaTime / currDebuffTime);
            yield return null;
        }

        //Debug.Log("�˹� �ڷ�ƾ ��");
        agent.enabled = true;
    }
}
