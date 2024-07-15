using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class LongRangeAttackEnemy : EnemyController
{
    public GameObject Projectile_Prefab;

    //List �� �����Ͽ� ���� �ɵ������� ������ �� �ְ� �غ���
    GameObject[] projectile = new GameObject[3];

    int projectileCnt = 5;
    int maxReloadTime = 3;
    float currentReloadTime = 0;

    private void Update()
    {
        SetReloadTime();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // ����ü ����
        for (int i = 0; i < projectile.Length; i++)
        {
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectile").transform); //GameObject.Find("Bullet").transform
            projectile[i].name = gameObject.name + " - Projectile" + i;
            projectile[i].GetComponent<StingrayBullet>().ParentName = gameObject.name;
            projectile[i].SetActive(false);
        }
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    //Debug.Log("Idle");
                    break;
                case State.MOVE:
                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    //Debug.Log("Trace");
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    //Debug.Log("Attack");
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = 5;
                    agent.isStopped = false;
                    FireProjectileObject();
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void FireProjectileObject()
    {
        if (currentReloadTime < maxReloadTime)
            return;

        int projectileIndex = FindBulletThatCanBeUsed();
        if (projectileIndex == -1) // ����� �� �ִ� �Ѿ��� ���� ��� 
            return;

        projectile[projectileIndex].SetActive(true);
        projectile[projectileIndex].GetComponent<StingrayBullet>().Init();
        currentReloadTime = 0;
    }

    /// <summary>
    /// ����� �� �ִ� ����ü�� ã�´�.
    /// </summary>
    /// <returns>-1 : ����� �� �ִ� ����ü ���� / 0~Bullet.Length-1 : ����� �� �ִ� ����ü �ε���</returns>
    int FindBulletThatCanBeUsed()
    {
        for (int i = 0; i < projectile.Length; i++)
        {
            if (projectile[i].activeSelf) // ������� �Ѿ� 
                continue;
            return i;
        }
        return -1;
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
