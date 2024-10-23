using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Witched : MediumBossEnemy
{
    [SerializeField]
    private GameObject[] ReinforcenmentAttackTarget = new GameObject[3];
    private GameObject[] ReinforcementAttackObjects = new GameObject[3];
    [SerializeField]
    private Transform reinforcementAttackPos;

    [Header("Skill - Teleport")]
    [SerializeField]
    private float teleportRunDist;
    [SerializeField]
    private float teleportFugitiveDist;

    [Header("Circle Range")]
    [SerializeField]
    private CircleRange CircleRangeScr;

    private bool firstRunAboutSkill2;

    string[] jiruruNames = new string[4];

    public override void Start()
    {
        // base.Start���� �ش� �ڷ�ƾ���� �̸� ������ �ʵ��� ����.
        isUpdaterCoroutineWorking = true;
        isAnimationCoroutineWorking = true;

        base.Start();

        //Vector3 posTomove = Vector3.zero;
        // ��ȭ ���� ����ü
        for (int i = 0; i < ReinforcementAttackObjects.Length; i++)
        {
            ReinforcementAttackObjects[i] = Instantiate(ReinforcementAttack_Prefab, new Vector3(enemyTr.position.x, -10, enemyTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform);
            ReinforcementAttackObjects[i].GetComponent<Projectile>().Init(gameObject.name, "- ReinforcementAttackAction" + i, 10, reinforcementAttackPos, ReinforcenmentAttackTarget[i]); // 60�� 3����
        }

        skillCondition[1] = true;
        firstRunAboutSkill2 = false;

        jiruruNames[0] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(3, 0, 3), Vector3.zero);
        jiruruNames[1] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(3, 0, -3), Vector3.zero);
        jiruruNames[2] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(-3 , 0,  3), Vector3.zero);
        jiruruNames[3] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(-3, 0, -3), Vector3.zero);

        for (int i = 0; i < jiruruNames.Count(); i++)
            EnemyManager.Instance.DeactivateEnemy(jiruruNames[i]);

        StartCoroutine(Updater());
        StartCoroutine(Animation());
    }

    /// <summary>
    /// ��ȭ ���� �ִϸ��̼� ���� �� ��ȭ ���� ���۽� ȣ���ϴ� �Լ�
    /// </summary>
    protected override void ReinforcementAttack()
    {
        // ��ȭ �⺻ ������ �⺻ ���� ��Ÿ���� ������ �� + �⺻ ���� Ƚ���� 4��°�� �� �����Ѵ�.

        // ��ȭ �⺻ ���� Ȱ��ȭ
        for (int i = 0; i < ReinforcementAttackObjects.Length; i++)
        {
            ReinforcementAttackObjects[i].SetActive(true);
            ReinforcementAttackObjects[i].GetComponent<Projectile>().SetDirToMove();
        }
    }

    protected override void CheckSkillsCondition()
    {
        CheckTeleportCondition();
        CheckDimensionalConnection();

        if (stateEffect == StateEffect.BINDING || stateEffect == StateEffect.AIR)
        {
            for (int i = 0; i < skillCondition.Count; i++)
            {
                // ������ ��ų�� ���
                if (moveSkillTypeData[i])
                {
                    skillCondition[i] = false;
                    break;
                }
            }
        }
    }

    private void CheckTeleportCondition()
    {
        float distance = Vector3.Distance(enemyTr.position, playerObj.transform.position);

        // ���� �Ÿ��� �������� �� + ��ų ������ Ȱ��ȭ���� �ʾ��� ��
        if (distance <= teleportFugitiveDist && !skillCondition[0])
            skillCondition[0] = true;
    }

    private void CheckDimensionalConnection()
    {
        if (firstRunAboutSkill2)
            return;

        if (currHP < maxHP * 0.4 && !skillCondition[2])
        {
            firstRunAboutSkill2 = true;
            skillCondition[2] = true;
            Debug.Log("Hp 40% �̸�");
        }
    }

    /// <summary>
    /// Skill Teleport �ִϸ��̼ǽ� ���� ���� ��ġ�� ȣ�� 
    /// </summary>
    private void Teleport()
    {
        Vector3 teleportPos = playerObj.transform.position + (enemyTr.position - playerObj.transform.position).normalized * teleportRunDist;

        enemyTr.position = teleportPos;
        Debug.Log("TelePort");
    }

    private void FolderGeist()
    {
        float damage = 1; //atk * (450 / defaultAtkPercent)
        CircleRangeScr.StartRangeCheck("KnockBack", "Witched", 10, 5, 1, damage, 1);
    }

    private void DimensionalConnection()
    {
        MaxShield = maxHP * 0.2f;
    }


    private void Skill0AniEnd()
    {
        SkillAniEnd();

        // ���� ������ ��ȭ ������ �ǵ��� �Ѵ�.
        currDefaultAtkCnt = maxDefaultAtkCnt;
    }

    private void Skill2AniEnd()
    {
        // �ǽ� ����
        if (currShield <= 0)
        {
            SetStateEffect("Stun", 3, Vector3.zero);
            Debug.Log("�ǽ� ����");
        }
            
        else // �ǽ� ����
        {
            Debug.Log("�ǽ� ����");
            for(int i=0; i< jiruruNames.Count(); i++)
            {
                EnemyManager.Instance.ActivateEnemy("Jiruru");
            }
        }   
    }
}
