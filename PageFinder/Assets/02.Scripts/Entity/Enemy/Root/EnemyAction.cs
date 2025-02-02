using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem.LowLevel;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.ParticleSystem;

public class EnemyAction : EnemyAnimation
{
    #region Enemy Coroutine
    protected override IEnumerator EnemyCoroutine()
    {
        while (!isDie)
        {
            // �÷��̾ �׾��� ���
            if (playerObj == null)
                break;

            Action();
            Animation();
            yield return null;
        }
    }

    protected void Action()
    {
        // ��Ÿ�� ���
        SetAllCoolTime();

        // ����(didPerceive �״��), �˹�(didPerceive �״��)
        // �ӹ�(didPerceive �״��), ����(didPerceive = false)
        if (state == State.DEBUFF && debuffState != DebuffState.NONE)
            return;

        // idle ���¿����� Idle �ִϸ��̼��� ������ ��� ���·� ����
        // �׷��Ƿ� Idle �ִϸ��̼� ���¿����� ��� �������� ������ �ʵ��� ��
        if (state == State.IDLE && idleState != IdleState.NONE)
            return;

        // ���� ���°� �Ǹ� ������ ������ attackState�� None���� �����
        // ���� �ִϸ��̼� ���� �������� �ʵ��� �����ϱ� ����
        if (state == State.ATTACK && attackState != AttackState.NONE)
            return;

        SetRootState();
        SetDetailState();
    }
    #endregion

    #region State

    protected virtual void SetRootState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        switch (attackDistType)
        {
            case AttackDistType.SHORT:
                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    if (distance <= cognitiveDist)
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy())
                            state = State.ATTACK;
                        else
                            state = State.MOVE; // ȸ��
                    }
                    else
                        state = State.MOVE;
                }
                else
                {
                    if (distance <= cognitiveDist)
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy())
                        {
                            didPerceive = true;
                            state = State.IDLE; // ���� ���
                        }
                        else
                            state = State.MOVE; // ȸ��

                        //Debug.Log("RootState didPerceive False : �����Ÿ� �ȿ� ����");
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance <= 1)
                            state = State.IDLE; // ���� ���
                        else
                            state = State.MOVE; // ����

                        //Debug.Log($"RootState didPerceive False  Diestance{distance}: {state}");
                    }
                }
                break;

            case AttackDistType.LONG:
                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    if (IsEnemyInCamera())
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy())
                            state = State.ATTACK;
                        else
                            state = State.MOVE; // ȸ��
                    }
                    else
                        state = State.MOVE;
                }
                else
                {
                    if (IsEnemyInCamera())
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy())
                        {
                            didPerceive = true;
                            state = State.IDLE; // ���� ���
                        }
                        else
                            state = State.MOVE; // ȸ��

                        //Debug.Log("RootState didPerceive False : �����Ÿ� �ȿ� ����");
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance <= 1)
                            state = State.IDLE; // ���� ���
                        else
                            state = State.MOVE; // ����

                        //Debug.Log($"RootState didPerceive False  Diestance{distance}: {state}");
                    }
                }
                break;
        }
    }

    protected void SetDetailState()
    {
        switch (state)
        {
            case State.IDLE:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                SetIdleState();
                SetAgentData(transform.position);
                break;

            case State.MOVE:
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                SetMoveState();
                SetMoveAction();
                break;

            case State.ATTACK:
                moveState = MoveState.NONE;
                debuffState = DebuffState.NONE;

                SetAttackState();
                SetAgentData(transform.position);
                break;

            case State.DEBUFF:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;

                SetAgentData(transform.position);
                break;

            case State.DIE:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                SetAgentData(transform.position);
                break;
        }
    }

    private void SetIdleState()
    {
        // IdleState.First : �� ó�� ���۽�, Stun�� ���� �Ŀ� ����
        // �׷��Ƿ� �� �Լ� ���η� ������ �����Ƿ� ���� ó������ �ʴ´�.
        if (didPerceive)
            idleState = IdleState.ATTACKWAIT;
        else
            idleState = IdleState.PATROLWAIT;
    }

    private void SetMoveState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);
        switch (attackDistType)
        {
            case AttackDistType.SHORT:
                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    // ȸ������ ���
                    if (personality == Personality.PATROL)
                    {
                        moveState = MoveState.RUN;
                        return;
                    }

                    if (distance <= cognitiveDist)
                    {
                        // �÷��̾ �տ� ���� ���� ���
                        if (!CheckPlayerInFrontOfEnemy())
                            moveState = MoveState.ROTATE;
                    }
                    else
                        moveState = MoveState.CHASE;
                }
                else
                {
                    if (distance <= cognitiveDist)
                    {
                        // ȸ������ ���
                        if (personality == Personality.PATROL)
                        {
                            moveState = MoveState.RUN;
                            return;
                        }

                        // �÷��̾ �տ� ���� ���� ���
                        if (!CheckPlayerInFrontOfEnemy())
                            moveState = MoveState.ROTATE;
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance > 1)
                            moveState = MoveState.PATROL;
                    }
                }
                break;

            case AttackDistType.LONG:
                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    // ȸ������ ���
                    if (personality == Personality.PATROL)
                    {
                        moveState = MoveState.RUN;
                        return;
                    }

                    if (IsEnemyInCamera())
                    {
                        // �÷��̾ �տ� ���� ���� ���
                        if (!CheckPlayerInFrontOfEnemy())
                            moveState = MoveState.ROTATE;

                        // �÷��̾�� �Ÿ��� nĭ �̳��� ��� �������� �߰��ؾ� �� 
                    }
                    else
                        moveState = MoveState.NONE;
                }
                else
                {
                    if (IsEnemyInCamera())
                    {
                        // ȸ������ ���
                        if (personality == Personality.PATROL)
                        {
                            moveState = MoveState.RUN;
                            return;
                        }

                        // �÷��̾ �տ� ���� ���� ���
                        if (!CheckPlayerInFrontOfEnemy())
                            moveState = MoveState.ROTATE;
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance > 1)
                            moveState = MoveState.PATROL;
                    }
                }
                break;
        }
       
    }


    protected virtual void SetAttackState()
    {
        // �ϱ� ���� �⺻���ݸ� ����
        attackState = AttackState.BASIC;
    }

    #endregion

    #region Action ����

    private void SetMoveAction()
    {
        switch (moveState)
        {
            case MoveState.NONE:
                break;

            case MoveState.PATROL:
                currDestination = patrolDestinations[patrolDestinationIndex];
                SetAgentData(currDestination, false);
                break;

            case MoveState.ROTATE:
                currDestination = transform.position;
                SetAgentData(currDestination, false, false);
                Rotate();
                break;

            case MoveState.CHASE:
                currDestination = playerObj.transform.position;
                SetAgentData(currDestination, false);
                break;

            case MoveState.RUN:
                SetAgentData(currDestination, false);
                break;

            default:
                SetAgentData(currDestination, false);
                Debug.LogWarning(moveState);
                break;
        }
    }


    protected virtual void BasicAttack()
    {
        // ��Ʈ �ڽ� Ȱ��ȭ �ڵ�� �ٲٱ�

        float distance = Vector3.Distance(playerObj.transform.position, enemyTr.position);

        if (distance <= cognitiveDist)
            playerState.CurHp -= atk;
    }

    /// <summary>
    /// �⺻ ���� �ִϸ��̼� ���� �� ����
    /// </summary>
    protected virtual void BasicAttackEnd()
    {
        attackState = AttackState.NONE;
    }

    #endregion

    #region CoolTime
    protected virtual void SetAllCoolTime()
    {
        SetIdleTime();
        SetDebuffTime();
    }

    private void SetIdleTime()
    {
        if (state == State.IDLE && idleState == IdleState.NONE)
            return;

        switch (idleState)
        {
            case IdleState.FIRSTWAIT:
                if(currFirstWaitTime > 0)
                {
                    currFirstWaitTime -= Time.deltaTime;
                    return;
                }

                currFirstWaitTime = maxFirstWaitTime;
                state = State.MOVE; // patrol
                break;

            case IdleState.PATROLWAIT:
                if (currPatrolWaitTime > 0)
                {
                    currPatrolWaitTime -= Time.deltaTime;
                    return;
                }

                PatrolDestinationIndex += 1;
                currDestination = patrolDestinations[PatrolDestinationIndex];
                currPatrolWaitTime = maxPatrolWaitTime;
                state = State.MOVE; // patrol
                break;

            case IdleState.ATTACKWAIT:
                if (currAttackWaitTime > 0)
                {
                    currAttackWaitTime -= Time.deltaTime;
                    return;
                }
               
                currAttackWaitTime = maxAttackWaitTime;
                state = State.ATTACK; // attack
                break;
        }
        idleState = IdleState.NONE;
    }

    private void SetDebuffTime()
    {
        if (state != State.DEBUFF)
            return;

        if (currDebuffTime < 0)
        {
            state = State.IDLE;
            return;
        }

        currDebuffTime -= Time.deltaTime;
    }

    #endregion

    #region Rotate
    private void Rotate()
    {
        Vector3 dir = (playerObj.transform.position - 
            new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z)).normalized;

        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, Quaternion.LookRotation(dir), 7f * Time.deltaTime); 
        // �ð��� ���� ȸ�� �ӵ����� �÷��̾� ������ �ӵ��� ����Ͽ� �ٷ� �÷��̾� ���� �ٶ� �� �ֵ��� ���߿� �����ϱ�
        // ����� �÷��̾ ��� ���ۺ��� ���� ���ݸ��ϰ� ȸ���� ��, �̰� Slerp�� ������ ���� ���� ��
    }

    protected bool CheckPlayerInFrontOfEnemy()
    {
        Vector3 pos = new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z); 

        switch(attackDistType)
        {
            case AttackDistType.SHORT:
                // ���� ���鿡 �÷��̾ ������ ���
                if (Physics.Raycast(pos, enemyTr.forward, cognitiveDist, LayerMask.GetMask("PLAYER")))
                    return true;
                else
                    return false;

            case AttackDistType.LONG:
                // ���� ���鿡 �÷��̾ ������ ���
                if (Physics.Raycast(pos, enemyTr.forward, Vector3.Distance(enemyTr.position, playerObj.transform.position), LayerMask.GetMask("PLAYER")))
                    return true;
                else
                    return false;

            default:
                return false;
        }
    }
    #endregion

    #region Long Distance Attack Enemy
    protected void SetBullet(GameObject bulletPrefab, int angle, float damage)
    {
        Vector3 targetDir = Quaternion.AngleAxis(angle, Vector3.up) * enemyTr.forward;
        Vector3 spawnPos = enemyTr.position + targetDir;
        spawnPos.y = playerObj.transform.position.y + 1;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity); // Witched �տ� �� ����(-60,0,60)�� �Ѿ� ����
        Bullet bulletScr = DebugUtils.GetComponentWithErrorLogging<Bullet>(bullet, "Bullet");
        bulletScr.Damage = damage;

        Vector3 targetPos = enemyTr.position + targetDir * 100; // �ش� �������� �� �ٱ����� ������ ������ �� �ֵ��� *100���� ����
        bulletScr.Fire(targetPos);
    }

    private bool IsEnemyInCamera()
    {
        Vector3 enemyUIPos =  Camera.main.WorldToViewportPoint(enemyTr.position);

        if (enemyUIPos.x >= 0 && enemyUIPos.x <= 1 && enemyUIPos.y >= 0 && enemyUIPos.y <= 1 && enemyUIPos.z > 0)
            return true;
        return false;
    }

    #endregion

    private void SetAgentData(Vector3 pos, bool isStop = true, bool isRotate = true)
    {
        //if (!agent.isOnNavMesh)
        //{
        //    Debug.LogError("AgenTdata : NavMeshAgent�� NavMesh ���� ���� �ʽ��ϴ�! Base Offset�� Ȯ���ϼ���.");
        //}

        agent.destination = pos;
        agent.isStopped = isStop;
        agent.updateRotation = isRotate;
    }
}