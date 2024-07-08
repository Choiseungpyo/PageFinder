using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class Stingray : EnemyController
{
    public GameObject Bullet_Prefab;

    GameObject[] Bullet = new GameObject[3];

    int bulletIndex = 0;
    int maxReloadTime = 3;
    float currentReloadTime = 0;

    private void Update()
    {
        SetReloadTime();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        // �⺻ �ɷ�ġ ����
        // �̼�, �ִ� ü��, ���� ü��, ���ݷ�, ����
        moveSpeed = 0.8f;
        maxHP = 40.0f;
        atk = 10.0f;

        // Ÿ�� ����
        posType = PosType.SKY; // ����
        moveType = MoveType.RANDOM; // ���� �̵�
        attackType = AttackType.PREEMPTIVE;

        traceDist = 10;
        attackDist = 7;
        //cognitiveDist = 5f;

        base.Start();

        // �Ѿ��� ��ü ��ü�� ��ȣ�� üũ�Ѵ�. 
        int parentNum = 0;
        for (int i = 0; i < 4; i++)
        {
            if (name.Contains(i.ToString()))
            {
                parentNum = i;
                break;
            }
        }

        for(int i=0; i< Bullet.Length; i++)
        {
            Bullet[i] = Instantiate(Bullet_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Bullet").transform);
            Bullet[i].GetComponent<StingrayBullet>().ParentNum = parentNum;
        }
    }

    /// <summary>
    /// ���� �̵�
    /// </summary>
    public override void MoveRandom()
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

        //Debug.Log(state);
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    Debug.Log("Idle");
                    break;
                case State.MOVE:
                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    Debug.Log("Trace");
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    Debug.Log("Attack");
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = 5;
                    agent.isStopped = false;
                    FireBullet();
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void FireBullet()
    {
        if (currentReloadTime < maxReloadTime)
            return;

        if (bulletIndex >= Bullet.Length)
            return;

        Vector3 bulletDir = (playerTr.position - monsterTr.position).normalized;
        bulletDir.y = 0;

        Bullet[bulletIndex].GetComponent<StingrayBullet>().CanMove = true;
        Bullet[bulletIndex].GetComponent<StingrayBullet>().TargetDir = bulletDir;
        Bullet[bulletIndex].transform.position = new Vector3(monsterTr.position.x, 2, monsterTr.position.z);
        bulletIndex++;
        currentReloadTime = 0;
    }

    public int BulletIndex
    {
        get
        {
            return bulletIndex;
        }
        set
        {
            bulletIndex = value;
        }
    }

    void SetReloadTime()
    {
        if (currentReloadTime >= maxReloadTime)
            return;

        currentReloadTime += Time.deltaTime;
    }

    // ����ü �߻� ���� 

    /* <������ ���� ��ƾ>
     * 1. ���� �����Ÿ� �̳�
     * 2. ����ü 1ȸ �߻� (������) 
     * 3. 1�� �� 2������ 
     * 
     */
}
