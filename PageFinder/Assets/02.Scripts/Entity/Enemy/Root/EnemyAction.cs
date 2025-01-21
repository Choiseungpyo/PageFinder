using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class EnemyAction : EnemyAnimation
{
    protected bool isUpdaterCoroutineWorking = false;

    public override float HP
    {
        get
        {
            return currHP; //+ currShield;  // 100 + 50   - 55
        }
        set
        {
            // ���ҽ��ѵ� ���尡 �����ִ� ���
            //if (value > currHP)
            //{
            //    CurrShield = value - currHP;
            //}
            //else // ���ҽ��ѵ� ���尡 �������� ���� ���
            //{
            //    CurrShield = 0;
            //    currHP = value;
            //}
            currHP = value;

            Hit();
            hpBar.SetCurrValueUI(currHP);
            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerState.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerState.Coin += 100;
                else
                    playerState.Coin += 250;

                isDie = true;
                // <�ؾ��� ó��>
                EnemyManager.Instance.DestroyEnemy("enemy", gameObject);
                //Debug.Log("�� ��Ȱ��ȭ");
                // �÷��̾� ����ġ ȹ��
                // ��ū ���� 
                //Die();
            }

        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(!isUpdaterCoroutineWorking)
            StartCoroutine(Updater());
    }

    /// <summary>
    /// ���� ���ظ� ���� �� �÷��̾� �ʿ��� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void Hit()
    {
        //SetStateEffect("Stun", 0.2f, Vector3.zero);
        //Debug.Log("Hit");
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MAP")) // �ʿ� ����� �� ���� �ٽ� ���� 
        {
            SetCurrentPosIndexToMove();
        }
    }

    private void OnDrawGizmos()
    {
        if (state == State.MOVE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, cognitiveDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, atkDist);
        }
    }
    protected IEnumerator Updater()
    {
        isUpdaterCoroutineWorking = true;

        while (!isDie)
        {
            if (playerObj == null)
                break;

            SetAllCoolTime();
            SetRootState();

            switch (state)
            {
                case State.IDLE:
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    SetIdleState();
                    IdleAction();
                    break;

                case State.ABNORMAL:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    AbnormalState();
                    break;

                case State.MOVE:
                    idleState = IdleState.NONE;
                    attackState = AttackState.NONE;

                    SetMoveState();
                    MoveAction();
                    break;

                case State.ATTACK:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;

                    SetAttackState();
                    AttackAction();
                    break;

                case State.DIE:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    Die();
                    break;
            }
            yield return null;
        }
    }

    #region State ���� �Լ�

    protected virtual void SetRootState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        // �����̻��� ���
        if (state == State.ABNORMAL)
            return;

        // ���� ������ �� �ִϸ��̼��� ������ ������ �� �ֵ��� �� 
        // �ִϸ��̼��� Event���� �ִϸ��̼��� ������ ��� ~AniEnd()�� ȣ���� �� AttackState.None���� �����س��� ���� �̿�
        if (state == State.ATTACK && attackState != AttackState.NONE)
            return;

        if (distance <= atkDist)
        {
            // ���� �÷��̾� �տ� �ִ� ���
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else// �÷��̾ �ٶ󺸵��� ȸ���ϰ� �� 
                state = State.MOVE;
        }
        else if (distance <= cognitiveDist)
        {
            if (abnormalState == AbnomralState.BINDING)
                state = State.IDLE;
            else
                state = State.MOVE;
        }
        else // ���� ���� �ٱ��� ���
        {
            if (abnormalState == AbnomralState.BINDING)
            {
                state = State.IDLE;
                return;
            }
                
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

    protected void SetIdleState()
    {
        // ���߿� Idle ���� �������� ������ ������ �׶� ���� �����ϱ�
        idleState = IdleState.DEFAULT;
    }


    protected void SetMoveState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        switch(attackPattern)
        {
            // ���� �Ÿ��� ������ �ʾ��� ��� Ž��
            // ���� �Ÿ��� ������ ��� ����
            case AttackPattern.PREEMPTIVE:
            case AttackPattern.AVOIDANCE:
            case AttackPattern.GUARD:

                if (distance > cognitiveDist)
                    moveState = MoveState.FIND;
                else
                    moveState = MoveState.TRACE;

                break;

            case AttackPattern.SUSTAINEDPREEMPTIVE:

                // �÷��̾ �� ���̶� �������� ���
                if (playerRecognitionStatue)
                    moveState = MoveState.TRACE;
                else
                {
                    if (distance > cognitiveDist)
                        moveState = MoveState.FIND;
                    else
                        moveState = MoveState.TRACE;
                }

                break;
            default:
                Debug.LogWarning(attackPattern);
                break;
        }

        // �÷��̾ �������� �ʰ� ȸ���� �ؾ��ؾ��ϴ� ���
        if (distance <= atkDist)
        {
            agent.destination = transform.position;

            // �÷��̾ �ڱ� �ڽ� �տ� ���� ���� ���
            if (!CheckIfThereIsPlayerInFrontOfEnemy())
                moveState = MoveState.ROTATE;
        } 

        switch (moveState)
        {
            case MoveState.FIND:
                FindState();
                break;

            case MoveState.TRACE:
                break;

            case MoveState.ROTATE:
                break;

            default:
                Debug.LogWarning(moveState);
                break;
        }
    }

    protected virtual void SetAttackState()
    {
        if (currDefaultAtkCoolTime <= 0)
            attackState = AttackState.DEFAULT;
        else // ��Ÿ���� ���� �ʾ��� ��� Idle -> Attack Wait
            attackState = AttackState.ATTACKWAIT;
    }

    private void FindState()
    {
        switch (findPattern)
        {
            case FindPattern.PATH:
                MovePath();
                break;
            case FindPattern.FIX:
                break;
            default:
                Debug.LogWarning(findPattern);
                break;
        }
    }

    #endregion

    #region Action ���� �Լ�

    protected void IdleAction()
    {
        agent.destination = transform.position;
        agent.isStopped = true;
    }

    protected void MoveAction()
    {
        switch(moveState)
        {
            case MoveState.NONE:
                break;

            case MoveState.FIND:
                FindAction();
                break;

            case MoveState.TRACE:
                TraceAction();
                break;

            case MoveState.ROTATE:
                RotateAction();
                break;

            default:
                Debug.LogWarning(moveState);
                break;

        }
    }

    protected virtual void AttackAction()
    {
        switch(attackState)
        {
            case AttackState.NONE:
                break;

            case AttackState.ATTACKWAIT:
                AttackWaitAction();
                break;

            case AttackState.DEFAULT:
                DefaultAttackAction();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    private void FindAction()
    {
        switch(findPattern)
        {
            case FindPattern.PATH:
                agent.destination = posToMove[currentPosIndexToMove];
                agent.isStopped = false;
                agent.updateRotation = true;
                break;

            case FindPattern.FIX:
                agent.isStopped = true;
                break;
        }
       
    }

    private void TraceAction()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        if (distance <= atkDist)
            agent.destination = transform.position;
        else if (distance < cognitiveDist)
        {
            Vector3 pos = playerObj.transform.position - (playerObj.transform.position - enemyTr.position).normalized * (atkDist - 0.2f);
            pos.y = transform.position.y;

            agent.destination = pos;  // ���� ��Ÿ� �������� ��ġ
        }
            

        // agent ���� ��
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    private void RotateAction()
    {
        SetEnemyDir();
    }

    protected void DefaultAttackAction()
    {
        agent.isStopped = true;
    }

    protected void AttackWaitAction()
    {
        if (!CheckIfThereIsPlayerInFrontOfEnemy())
            SetEnemyDir();
    }

    private void AbnormalState()
    {
        agent.isStopped = true;

        switch (abnormalState)
        {
            case AbnomralState.STUN:
                break;

            case AbnomralState.KNOCKBACK:
                enemyTr.position = Vector3.MoveTowards(enemyTr.position, stateEffectPos, Time.deltaTime * 3);
                break;

            case AbnomralState.AIR:
                Debug.Log("Air �̵���");
                // �ִϸ��̼� ��ü���� �ϴ÷� ������� �������� �����ִ� ������ �ϴ°� ������
                // ���� �����ϴ� �ͺ��ٴ� �� ���� ���ƺ���. 
                // ���� : ������ �������κ��� ���� NavMeshAgent ������ ���̶��� �����ϱ� ���ŷο�.

                enemyTr.position = Vector3.MoveTowards(enemyTr.position, stateEffectPos, Time.deltaTime * 3);
                break;
        }
    }

    #endregion

    #region �ð� ���� �Լ�
    protected virtual void SetAllCoolTime()
    {
        if (abnormalState != AbnomralState.NONE)
            SetAbnormalTime();

        if (state == State.IDLE)
        {
            if(idleState == IdleState.DEFAULT)
                SetCurrFindCoolTime();
        }
        else if (state == State.MOVE)
        {
            if (moveState == MoveState.TRACE)
                SetAttackCooltime();
        }
        else if (state == State.ATTACK)
        {
            if (attackState == AttackState.ATTACKWAIT || attackState == AttackState.DEFAULT)
                SetAttackCooltime();
        }
    }

    protected virtual void SetAttackCooltime()
    {
        SetCurrDefaultAtkCoolTime();
    }

    protected void SetCurrDefaultAtkCoolTime()
    {
        if (currDefaultAtkCoolTime < 0)
            return;

        currDefaultAtkCoolTime -= Time.deltaTime;
    }

    protected void SetCurrFindCoolTime()
    {
        if (currFindCoolTime < 0)
            return;

        currFindCoolTime -= Time.deltaTime;
    }

    protected void SetAbnormalTime()
    {
        if (currAbnormalTime < 0)
        {
            state = State.IDLE;
            abnormalState = AbnomralState.NONE;
            return;
        }

        currAbnormalTime -= Time.deltaTime;
    }

    #endregion

    #region ��� �̵� ���� �Լ�
    /// <summary>
    /// ��� �̵�
    /// </summary>
    private void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], enemyTr.position);

        // �̼��� ���̷� �������� ���� ��쵵 �ֱ� ������ 0�� �ƴ϶� 0.5f�� ����
        if (distance > 1f)
            return;

        // ��ǥ������ �������� ���
        SetCurrentPosIndexToMove();
        currFindCoolTime = maxFindCoolTime;
    }

    /// <summary>
    /// ���� posIndexToMove ���� �����Ѵ�.
    /// </summary>
    private void SetCurrentPosIndexToMove()
    {
        if (currentPosIndexToMove >= posToMove.Length - 1) // �ִ� �ε��� ���� �����ϱ� ���� 0���� �ٽ� ���µǵ��� ����
            currentPosIndexToMove = 0;
        else
            currentPosIndexToMove++;
    }

    #endregion

    private void SetEnemyDir()
    {
        //Debug.Log("������ ���� ---------------------------------> ȸ�� �� ����");

        Vector3 dir = playerObj.transform.position - new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z);
        agent.updateRotation = false;
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, Quaternion.LookRotation(dir), 3f * Time.deltaTime);
    }

    protected bool CheckIfThereIsPlayerInFrontOfEnemy()
    {
        Vector3 pos = new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z); 

        // ���� ���鿡 �÷��̾ ������ ���
        if (Physics.Raycast(pos, enemyTr.forward, atkDist, LayerMask.GetMask("PLAYER")))
            return true;
        else
            return false;
    }

    private void DefaultAttack()
    {
        float distance = Vector3.Distance(playerObj.transform.position, enemyTr.position);

        if(distance <= atkDist)
            playerState.CurHp -= atk * (defaultAtkPercent / 100);
    }
}
