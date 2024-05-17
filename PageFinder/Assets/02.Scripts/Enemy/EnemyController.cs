using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }

    // ���ʹ��� ���� ����
    public State state = State.IDLE;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    // ���� �����Ÿ�
    private float attackDist = 4.0f;
    // ���ʹ��� ��� ����
    public bool isDie = false;
    // ������� �ð�
    private float monsterDieTime = 3.0f;

    private Transform monsterTr;
    private GameObject playerObj;
    private Transform playerTr;
    private TokenManager tokenManager;
    private NavMeshAgent agent;
    private MeshRenderer meshRenderer;
    private Exp exp;
    private Palette palette;
    // Start is called before the first frame update
    void Start()
    {
        monsterTr = GetComponent<Transform>();

        playerObj = GameObject.FindWithTag("PLAYER");
        playerTr = playerObj.GetComponent<Transform>();
        exp = playerObj.GetComponent<Exp>();
        palette = playerObj.GetComponent<Palette>();
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
        meshRenderer = GetComponent<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(CheckEnemyState());
        StartCoroutine(EnemyAction());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        tokenManager.MakeToken(new Vector3(transform.position.x, 0.25f, transform.position.z));
        exp.IncreaseExp(50);
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("Weapon"))
            return;

        /*if (!player.CheckAttackAniIsPlaying())
            return;*/
        // �÷��̾ ���� ���� + ����� �ε����� ��
        meshRenderer.material.color = palette.ReturnCurrentColor();
        state = State.DIE;
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
            meshRenderer.material.color = Color.green;
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if(distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if(distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }
    IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    //meshRenderer.material.color = Color.green;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    //meshRenderer.material.color = Color.gray;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    //meshRenderer.material.color = Color.black;
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }

    }

    public void Die()
    {
        Destroy(this.gameObject, monsterDieTime);
    }
}
