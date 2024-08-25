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

    int maxReloadTime = 3;
    float currentReloadTime = 0;

    public virtual void Update()
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
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform); //GameObject.Find("Bullet").transform
            projectile[i].name = gameObject.name + " - Projectile" + i;
            projectile[i].GetComponent<Projectile>().ParentName = gameObject.name;
            projectile[i].SetActive(false);
        }
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            SetCurrentSkillCoolTime();
            ChangeCurrentStateToSkillState();

            switch (state)
            {
                case State.IDLE:
                    ani.SetBool("isIdle", true);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);
                    break;
                case State.MOVE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", true);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = true;
                    FireProjectileObject();
                    break;
                case State.STUN:
                    ani.SetFloat("stunTime", stunTime);
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", true);

                    agent.isStopped = true;
                    break;
                case State.SKILL:
                    // �ش� �� Ŭ�������� �������Ͽ� ���ϴ� ��ų�� ȣ���Ѵ�. 
                    Debug.Log("Skill ���");
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return null;
        }
    }

    protected void FireProjectileObject()
    {
        if (currentReloadTime < maxReloadTime)
            return;

        int projectileIndex = FindBulletThatCanBeUsed();
        if (projectileIndex == -1) // ����� �� �ִ� �Ѿ��� ���� ��� 
            return;
        //Debug.Log("�Ѿ� �߻�");
        projectile[projectileIndex].SetActive(true);
        projectile[projectileIndex].GetComponent<Projectile>().Init();
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

    protected void SetReloadTime()
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
