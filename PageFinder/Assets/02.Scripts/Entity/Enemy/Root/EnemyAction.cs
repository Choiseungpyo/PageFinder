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
        get { return currHP; }
        set
        {
            currHP -= value;
            Hit();
            hpBar.value = currHP;
            
            //Debug.Log(name + " : " + HP);
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

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(!isUpdaterCoroutineWorking)
            StartCoroutine(Updater());
    }


    private void OnDestroy()
    {
        if (tokenManager != null)
            tokenManager.MakeToken(new Vector3(transform.position.x, 0.25f, transform.position.z));
        if (exp != null)
            exp.IncreaseExp(50);
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
            SetAllCoolTime();
            SetAllState();

            switch (state)
            {
                case State.IDLE:
                    abnormalState = AbnormalState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    SetIdleState();
                    IdleAction();
                    break;

                case State.ABONORMAL:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    SetAbnormalState();
                    AbnormalAction();
                    break;

                case State.MOVE:
                    idleState = IdleState.NONE;
                    abnormalState = AbnormalState.NONE;
                    attackState = AttackState.NONE;

                    SetMoveState();
                    MoveAction();
                    break;

                case State.ATTACK:
                    idleState = IdleState.NONE;
                    abnormalState = AbnormalState.NONE;
                    moveState = MoveState.NONE;

                    SetAttackState();
                    AttackAction();
                    break;

                case State.DIE:
                    idleState = IdleState.NONE;
                    abnormalState = AbnormalState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    Die();
                    break;
            }
            yield return null;
        }
    }

    protected virtual void SetAllState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        if (distance <= atkDist)
        {
            // ���� �÷��̾� �տ� �ִ� ���
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else
                state = State.MOVE;

        }
        else if (distance <= cognitiveDist)
            state = State.MOVE;
        else // ���� ���� �ٱ��� ���
        {
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

    #region State ���� �Լ�

    protected void SetIdleState()
    {
        // ���߿� Idle ���� �������� ������ ������ �׶� ���� �����ϱ�
        idleState = IdleState.DEFAULT;
    }

    protected void SetAbnormalState()
    {
        switch (abnormalState)
        {
            case AbnormalState.BINDING:
                break;

            case AbnormalState.STUN:
                StunAction();
                break;
        }
    }

    protected void SetMoveState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        switch(attackPattern)
        {
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

        switch (moveState)
        {
            case MoveState.FIND:
                FindState();
                break;

            case MoveState.TRACE:
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

    protected void AbnormalAction()
    {
        switch (abnormalState)
        {
            case AbnormalState.BINDING:
                break;

            case AbnormalState.STUN:
                StunAction();
                break;

        }
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
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Find"))
            return;

        agent.destination = posToMove[currentPosIndexToMove];
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    private void TraceAction()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Trace"))
            return;

        if (distance <= atkDist)
        {
            agent.destination = transform.position;

            // �÷��̾ �ڱ� �ڽ� �տ� ���� ���� ���
            if (!CheckIfThereIsPlayerInFrontOfEnemy())
                SetEnemyDir();
        }
        else if (distance < cognitiveDist)
            agent.destination = playerObj.transform.position - (playerObj.transform.position - enemyTr.position).normalized * atkDist;  //  ���� ��Ÿ� �������� ��ġ


        // agent ���� ��
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    protected void DefaultAttackAction()
    {
        agent.isStopped = true;
    }

    private void StunAction()
    {
        agent.isStopped = true;
    }

    #endregion

    #region ��Ÿ�� ���� �Լ�
    protected virtual void SetAllCoolTime()
    {
        if(state == State.IDLE)
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
    #endregion

    #region ��� �̵� ���� �Լ�
    /// <summary>
    /// ��� �̵�
    /// </summary>
    private void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], enemyTr.position);

        // �̼��� ���̷� �������� ���� ��쵵 �ֱ� ������ 0�� �ƴ϶� 0.5f�� ����
        if (distance > 0.5f)
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

    #region ��
    /// <summary>
    /// ���� ���ظ� ���� �� �÷��̾� �ʿ��� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void Hit()
    {
        state = State.ABONORMAL;
    }

    private void SetEnemyDir()
    {
        //Debug.Log("������ ���� ---------------------------------> ȸ�� �� ����");

        Vector3 dir = playerObj.transform.position - new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z);
        agent.updateRotation = false;
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, Quaternion.LookRotation(dir), 1.8f * Time.deltaTime);
    }

    protected bool CheckIfThereIsPlayerInFrontOfEnemy()
    {
        // ���� ���鿡 �÷��̾ ������ ���
        if (Physics.Raycast(enemyTr.position, enemyTr.forward, atkDist, LayerMask.GetMask("PLAYER")))
            return true;
        else
            return false;
    }

    #endregion
}
