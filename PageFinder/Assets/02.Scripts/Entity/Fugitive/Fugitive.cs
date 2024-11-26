using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Fugitive : Entity
{
    private enum State
    {
        MOVE,
        STUN
    }

    private enum MoveState
    {
        MOVETORALLY, // ���� ����Ʈ�� �̵�
        WALKAWAYFROMFUGITIVE, // �����ڷκ��� ����
        RUNFROMPLAYER // �÷��̾�κ��� ����
    }

    private enum TargetPosType
    {
        RALLY,
        FUGITIVE,
        PLAYER
    }

    private State state = State.MOVE; // ���ʹ��� ���� ����
    private MoveState moveState = MoveState.MOVETORALLY; // ���ʹ��� �̵� ����

    private float playerCognitiveDist = 5; // �÷��̾� ���� �Ÿ�
    private float fugitiveCognitiveDist = 3; // ������ ���� �Ÿ�

    private NavMeshPath path;

    private Vector3 targetPos; // �̵��� ��ǥ
    private bool[] canChangeTargetPos = { true, true, true};


    int currPosIndex;
    private float moveDistance = 5;

    private bool isDie = false;

    private Vector3 otherFugitivePos;

    Vector3[] rallyPoints;

    float currStunTime;

    protected Transform playerTr;
    protected NavMeshAgent agent;
    private RallyPoints rallyData;
    /*
     * <����>
     * 1. EnemyMananger���� ���� ����Ʈ �ޱ� 
     * 2. �ش� ���� ����Ʈ�鿡�� �������� ��������Ʈ �ϳ� ���� �ش� ��ǥ�� �̵� ��ǥ �ʱ�ȭ
     * 3. state == MOVETORALLY
     *      
     * <����>
     * 1. �÷��̾� ���� ���� ���� �÷��̾ ������ �÷��̾�->�ڽ� �������� n��ŭ ������ �Ÿ��� ���� ��ǥ �̵�
     * 2. ������ ���� ���� ���� �����ڰ� ������ ������->�ڽ� �������� n��ŭ ������ �Ÿ��� ���� ��ǥ�� �̵�
     * 3. 1,2�� �ش����� �ʴ´ٸ� ���� ����Ʈ�� �̵�
     */

    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;
            if (hpBar != null)
                hpBar.SetMaxValueUI(maxHP);
            HP = value;
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
            currHP = value; //def
            //Debug.Log("Hit!\n ���� ü��: " + currHP);
            // Tatget�� ���
            if (hpBar != null)
                hpBar.SetCurrValueUI(currHP);

            if (currHP <= 0)
            {
                EnemyManager.Instance.DestroyEnemy("fugitive", gameObject);
            }
        }
    }

    public float PlayerCognitiveDist
    {
        get
        {
            return playerCognitiveDist;
        }
        set
        {
            playerCognitiveDist = value;
        }
    }

    public float FugitiveCognitiveDist
    {
        get
        {
            return fugitiveCognitiveDist;
        }
        set
        {
            fugitiveCognitiveDist = value;
        }
    }

    public float MoveDistance
    {
        get
        {
            return moveDistance;
        }
        set
        {
            moveDistance = value;
        }
    }

    public override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        playerTr = GameObject.FindWithTag("PLAYER").transform;
        rallyData = GameObject.Find("RallyPoints").GetComponent<RallyPoints>();

        agent.speed = moveSpeed * 3.5f;

        //if (name.Contains("Target"))
        //{
        //    hpBar = GetComponentInChildren<SliderBar>();
        //    shieldBar = GetComponentInChildren<ShieldBar>();
        //    hpBar.SetMaxValueUI(maxHP);
        //    hpBar.SetCurrValueUI(currHP);
        //    MaxShield = 0;
        //}

        isDie = false;

        StartCoroutine(Updater());
    }

    protected IEnumerator Updater()
    {
        while (!isDie)
        {
            setCoolTime();

            switch (state)
            {
                case State.MOVE:
                    //SetMoveState();
                    MoveAction();
                    break;

                //case State.STUN:
                //    StunAction();
                //    break;

                default:
                    Debug.LogWarning(state);
                    break;
            }
            yield return null;
        }
    }


    private void MoveAction()
    {
        switch (moveState)
        {
            case MoveState.MOVETORALLY:
                //agent.isStopped = false;
                // ������ ���� ����Ʈ ����
                SetRandomRallyPoint();
                break;

            //case MoveState.WALKAWAYFROMFUGITIVE:
            //    agent.isStopped = false;
            //    // �ٸ� ������ -> �� ���⺤�ͷ� ����
            //    setTargetPos(1, otherFugitivePos);
            //    agent.destination = targetPos;
            //    break;

            //case MoveState.RUNFROMPLAYER:
            //    agent.isStopped = false;
            //    // �÷��̾� -> �� ���⺤�ͷ� ����
            //    setTargetPos(0, playerTr.position);
            //    agent.destination = targetPos;
            //    break;

            default:
                Debug.LogWarning(moveState);
                break;
        }
    }

    private void SetMoveState()
    {
        float dist = Vector3.Distance(playerTr.position, transform.position);
        
        //// �÷��̾ ���� �����ȿ� ������ ���
        //if (dist < playerCognitiveDist)
        //{
        //    Debug.Log("�÷��̾ ���� �����ȿ� ���Խ��ϴ�.");
        //    moveState = MoveState.RUNFROMPLAYER;
        //    return;
        //}

        //// ���� ���� �����ڰ� �ִ� ���
        //if (checkIfOtherFugitiveIsInCoginitiveRange())
        //{
        //    Debug.Log("������ ���� �����ȿ� ���Խ��ϴ�.");
        //    moveState = MoveState.WALKAWAYFROMFUGITIVE;
        //    return;
        //}

        if (!canChangeTargetPos[0] || !canChangeTargetPos[1])
            return;

        // ���� ����Ʈ�� �̵�
        moveState = MoveState.MOVETORALLY;
    }

    private void SetRandomRallyPoint()
    {
        // �� �������� ���
        if(!canChangeTargetPos[2])
        {
            if (Vector3.Distance(targetPos, transform.position) < 1f)
            {
                rallyData.SetUseState(currPosIndex, false);
                canChangeTargetPos[2] = true;
            }
            return;
        }

        
        canChangeTargetPos[2] = false;

        int randomRallyPointIndex = Random.Range(0, rallyPoints.Length); //rallyPoints.Length

        while (targetPos == rallyPoints[randomRallyPointIndex] && rallyData.CheckIfCanUseRallyPoint(randomRallyPointIndex))
        {
            targetPos = rallyPoints[Random.Range(0, rallyPoints.Length)];
        }

        targetPos = rallyPoints[randomRallyPointIndex];
        currPosIndex = randomRallyPointIndex;
        rallyData.SetUseState(randomRallyPointIndex, true);
        agent.destination = targetPos;
    }

    //private void setTargetPos(int index, Vector3 pos)
    //{
    //    float dist = Vector3.Distance(targetPos, transform.position);
    //    // �� �������� ���
    //    if (!canChangeTargetPos[index])
    //    {
    //        if(dist < 1f)
    //        {
    //            canChangeTargetPos[index] = true;
    //            Debug.Log("������ ����!");
    //        }
                
    //        return;
    //    }

    //    canChangeTargetPos[index] = false;

    //    Vector3 dir = (transform.position - pos).normalized;
    //    Vector3 tmpPos = transform.position + new Vector3(dir.x, 0, dir.z) * moveDistance;

    //    agent.destination = tmpPos;
    //    NavMeshPathStatus status = agent.pathStatus;

    //    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    obj.transform.position = tmpPos;

    //    if (status == NavMeshPathStatus.PathComplete)
    //        Debug.Log("PathComplete");
    //    else if (status == NavMeshPathStatus.PathPartial)
    //        Debug.Log("PathPartial");
    //    else if (status == NavMeshPathStatus.PathInvalid)
    //        Debug.Log("PathInvalid");

    //    // ������ ����
    //    // ������ �� �ִ� ��ġ�� ���
    //    if (NavMesh.CalculatePath(transform.position, tmpPos, NavMesh.AllAreas, path)) //
    //    {
    //        targetPos = tmpPos;
    //        Debug.Log("�÷��̾� �ݴ�������� ���� :" + targetPos);
    //    }
    //    else
    //    {
    //        targetPos = getNearestRallyPos(); // ���� ����� ���� ����Ʈ�� ����
    //        Debug.Log("�÷��̾� �ݴ�������� �����ߴµ� ���� �ٱ��̶� ���� ����� ���� ����Ʈ�� ����" + targetPos);
    //    }
           
    //}

    private void StunAction()
    {
        agent.isStopped = true;
    }

    private void setCoolTime()
    {
        if (state != State.STUN)
            return;

        currStunTime -= Time.deltaTime;
        if (currStunTime < 0)
        {
            currStunTime = 0;
            state = State.MOVE;
        }
    }

    //bool checkIfOtherFugitiveIsInCoginitiveRange()
    //{
    //    Collider[] objs = Physics.OverlapSphere(transform.position, fugitiveCognitiveDist, LayerMask.GetMask("ENEMY"));
    //    float maxDist = 0;
    //    int maxDistObjIndex = -1;
    //    float dist = 0;

    //    for (int i = 0; i < objs.Length; i++)
    //    {
    //        dist = Vector3.Distance(transform.position, objs[i].transform.position);
    //        if (dist > maxDist)
    //        {
    //            maxDistObjIndex = i;
    //            maxDist = dist;
    //        }
    //    }

    //    // ���� ���� �ٸ� �����ڵ��� ���� ���
    //    if (maxDistObjIndex == -1)
    //    {
    //        otherFugitivePos = Vector3.zero;
    //        return false;
    //    }

    //    // ���� ���� �ٸ� �����ڵ��� �ִ� ���
    //    otherFugitivePos = objs[maxDistObjIndex].transform.position;
    //    return true;
    //}

    public void SetRallyPoints(Vector3[] pos)
    {
        rallyPoints = pos;
    }

    // ���� ����� ���� ����Ʈ ���
    private Vector3 getNearestRallyPos()
    {
        float dist;
        float maxDist = 0;
        int rallyPointIndex = 0;

        for (int i = 0; i < rallyPoints.Length; i++)
        {
            dist = Vector3.Distance(transform.position, rallyPoints[i]);
            if (dist > maxDist)
            {
                rallyPointIndex = i;
                maxDist = dist;
            }
        }

        return rallyPoints[rallyPointIndex];
    }

    public void SetStatus(RiddlePage page, int index)
    {
        playerCognitiveDist = page.playerCognitiveDist[index];
        fugitiveCognitiveDist = page.fugitiveCognitiveDist[index];
        moveDistance = page.moveDistance[index];
        SetRallyPoints(page.rallyPoints);
        moveSpeed = page.moveSpeed[index];
        MAXHP = page.maxHp[index];
    }
}
