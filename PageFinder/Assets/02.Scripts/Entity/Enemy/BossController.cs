using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : Enemy
{
    public enum State
    {
        IDLE,
        MOVE,
        TRACE,
        ATTACK,
        SKILL,
        DIE
    }


    // ���ʹ��� ���� ����
    public State state = State.IDLE;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    // ���� �����Ÿ�
    public float attackDist = 4.0f;

    // ��ų ��Ÿ��
    protected float currentSkillCoolTime = 0;
    protected float maxSkillCoolTime = -1;
    protected bool usingSkill = false;

    // �� �߾�
    public Vector3 mapCenterPos = Vector3.zero;

    protected Transform monsterTr;
    private GameObject playerObj;
    protected Transform playerTr;
    protected Player playerScr;
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

        // ���� Ŭ�������� �ʱ�ȭ���� �ʾ��� ��� 10���� �ʱ�ȭ
        if(maxSkillCoolTime == -1)
            maxSkillCoolTime = 10;

        currentSkillCoolTime = maxSkillCoolTime;
        usingSkill = false;


        StartCoroutine(CheckEnemyMoveState());
        StartCoroutine(EnemyAction());
    }

    // Update is called once per frame
    void Update()
    {
        SetSkillCoolTime();
    }

    private void OnDestroy()
    {
        if (tokenManager != null)
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
    protected virtual void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= atk;
            Debug.Log("PLAYER HP: " + playerScr.HP);
        }

        //meshRenderer.material.color = Color.magenta; //palette.GetCurrentColor();
    }

    protected virtual IEnumerator CheckEnemyMoveState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            // �� �߾Ӱ� �÷��̾� ��ġ ��
            float distance = Vector3.Distance(playerTr.transform.position, mapCenterPos);

            // �÷��̾ ���� ������ ������ ���� ��� �������� �ʵ��� �Ѵ�.
            if (distance > 50) // �� ���� : 50
                continue;

            distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);

            if (!CheckSkillCoolTimeIsEnded()) // ��ų ��Ÿ���� ������ ���� ���
            {
                state = State.TRACE;
                if (distance <= attackDist)
                {
                    state = State.ATTACK;
                }
            }
            else // ��ų ��Ÿ���� ���� ���
            {
                if (CheckIfSkillIsUsing()) // ��ų�� ������� ���
                    continue;

                // ó�� ��ų ����ϴ� ���
                usingSkill = true;

                // ��ų�� ������ ���� �Ʒ� �ڵ� �߰��ϱ�
                //currentSkillCoolTime = 0;

                state = State.SKILL;
            }
        }
    }


    protected virtual IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    break;
                case State.MOVE:
                    meshRenderer.material.color = Color.green;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    Debug.Log("Trace");
                    meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    Debug.Log("Attack");
                    meshRenderer.material.color = Color.red;
                    break;
                case State.SKILL:
                    meshRenderer.material.color = Color.yellow;
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void SetSkillCoolTime()
    {
        if (CheckIfSkillIsUsing())
            return;

        currentSkillCoolTime += Time.deltaTime;
        //Debug.Log(currentSkillCoolTime);
    }

    protected bool CheckSkillCoolTimeIsEnded()
    {
        return currentSkillCoolTime >= maxSkillCoolTime ? true : false;
    }

    protected bool CheckIfSkillIsUsing()
    {
        return usingSkill ? true : false;
    }

}
