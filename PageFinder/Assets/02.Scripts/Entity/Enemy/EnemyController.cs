using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Enemy
{
    public enum State
    {
        IDLE,
        MOVE,
        TRACE,
        ATTACK,
        DIE
    }


    // ���ʹ��� ���� ����
    public State state = State.IDLE;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    // ���� �����Ÿ�
    protected float attackDist = 4.0f;
    // ���� �����Ÿ�
    public float cognitiveDist = 10.0f;

    public Vector3[] posToMove = { Vector3.zero, Vector3.zero };
    protected int currentPosIndexToMove = 0;


    protected Transform monsterTr;
    private GameObject playerObj;
    protected Transform playerTr;
    private Player playerScr;
    private TokenManager tokenManager;
    protected NavMeshAgent agent;
    private Exp exp;
    private Palette palette;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        monsterTr = GetComponent<Transform>();

        GetPlayerScript();

        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
        agent = GetComponent<NavMeshAgent>();

        currentPosIndexToMove = 0;

        StartCoroutine(CheckEnemyState());
        StartCoroutine(EnemyAction());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(tokenManager != null)
            tokenManager.MakeToken(new Vector3(transform.position.x, 0.25f, transform.position.z));
        if (exp != null)
            exp.IncreaseExp(50);
    }

    // �÷��̾� �Լ� ��������
    public void GetPlayerScript()
    {
        playerObj = GameObject.FindWithTag("PLAYER");
        playerTr = playerObj.GetComponent<Transform>();
        playerScr = playerObj.GetComponent<Player>();
        exp = playerObj.GetComponent<Exp>();
        palette = playerObj.GetComponent<Palette>();
    }
    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.name);
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= atk;
            Debug.Log("PLAYER HP: " + playerScr.HP);
        }
        else if(coll.CompareTag("MAP") && moveType == 1) // ���� �̵��� �ʿ� ����� �� ���� �ٽ� ����
        {
            Debug.Log("���� ���� ����");
            SetCurrentPosIndexToMove();
            float distance = 0;
            // ���� �̵��� ��ǥ�� �����ϰ� ����
            while (distance < cognitiveDist) // ���� ��ǥ�� ���� ���� ������ ���� ������ ��ǥ�� �Ÿ��� �ּ� 3�̻� �� �� �ְ� ����
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);
            }
        }

        //meshRenderer.material.color = Color.magenta; //palette.ReturnCurrentColor();
        //state = State.DIE;
    }
    private void OnDrawGizmos()
    {
        if(state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
    IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            //meshRenderer.material.color = Color.green;
            yield return new WaitForSeconds(0.3f);
            
            if (moveType == 0) // ��� �̵�
                MovePath();
            else if (moveType == 1) // ���� �̵�
                MoveRandom();
            else if (moveType == 2) // ���� �̵�
                MoveTrace();
            else
                Debug.LogWarning(moveType);
        }

        // ���� ��ƾ
        /*
         *  1. ������ ���� ���� ���� �̵� (��� �̵�, ���� �̵�, ���� �̵�)
         *  2. �� �߰�
         *  3. ����
         *  4. ���� (����, ���� ����, ȸ��, ��ȣ)
         */ 
         
    }
    IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    //meshRenderer.material.color = Color.green;
                    //agent.SetDestination(playerTr.position);
                    //agent.isStopped = false;
                    //state = State.MOVE;
                    Debug.Log("Idle");
                    break;
                case State.MOVE:
                    //meshRenderer.material.color = Color.green;
                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    Debug.Log("Trace");
                    //meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    Debug.Log("Attack");
                    //meshRenderer.material.color = Color.red;
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// ��� �̵�
    /// </summary>
    public virtual void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        if (distance <= 1)
            SetCurrentPosIndexToMove();

        if (!CheckCognitiveDist())
        { 
            return;
        }

        
        distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        Debug.Log(distance);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
            return;
        }
        else if (distance <= traceDist)
        {
            state = State.TRACE;
            return;
        }

    }

    /// <summary>
    /// ���� �̵�
    /// </summary>
    public virtual void MoveRandom() 
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        if (distance <= 1.5f)
        {
            SetCurrentPosIndexToMove();

            // ���� �̵��� ��ǥ�� �����ϰ� ����
            while (distance < cognitiveDist || agent.pathPending) // ���� ��ǥ�� ���� ���� ������ ���� ������ ��ǥ�� �Ÿ��� �ּ� 3�̻� �� �� �ְ� ����
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);
               
            }
        }

        Debug.Log(CheckCognitiveDist());
        Debug.Log(attackType);
        if (!CheckCognitiveDist())
            return;

        distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
        }
        else if (distance <= traceDist)
        {
            state = State.TRACE;
        }

        Debug.Log(state);
    }

    public void MoveTrace()
    {
        float distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
        }
        else if (traceDist > 0) // ��� �����ϵ��� ����
        {
            state = State.TRACE;
        }
        else
        {
            state = State.IDLE;
        }
    }

    protected bool CheckCognitiveDist()
    {
        float distance = Vector3.Distance(originalPos, playerTr.transform.position);

        if (attackType == 0) // ���� ���� �������� ����
        {
            //Debug.Log(distance);
            if (distance <= cognitiveDist)
                return true;
            else
                return false;
        }
        else if(attackType == 1) // ���� ���� �ٱ����� ����
        {
            return true;
        }
        else
        {
            Debug.LogWarning(attackType);
            return false;
        }
    }

    protected void SetCurrentPosIndexToMove()
    {
           if (currentPosIndexToMove >= posToMove.Length - 1) // �ִ� �ε��� ���� �����ϱ� ���� 0���� �ٽ� ���µǵ��� ����
                currentPosIndexToMove = 0;
            else
                currentPosIndexToMove++;
    }

    protected float ReturnRandomValue(float min, float max)
    {
        if(Random.Range(0,2) == 0)
            return -Random.Range(min, max);
        else 
            return Random.Range(min, max);
    }

}
