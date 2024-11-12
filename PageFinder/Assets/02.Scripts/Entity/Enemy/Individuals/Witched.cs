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

    int jiruruCnt = 4;

    int maxHitCnt = 5;
    int currHitCnt = 0;
    bool hadExcutedDimensionalConnection = false;

    public override float HP
    {
        get
        {
            return currHP + currShield;  // 100 + 50   - 55
        }
        set
        {
            // ���ҽ��ѵ� ���尡 �����ִ� ���
            if (value > currHP)
            {
                CurrShield = value - currHP;
            }
            else // ���ҽ��ѵ� ���尡 �������� ���� ���
            {
                CurrShield = 0;
                currHP = value;
            }

            if (hadExcutedDimensionalConnection)
                currHP++;


            Hit();
            hpBar.SetCurrValueUI(currHP);
            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerScr.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerScr.Coin += 100;
                else
                    playerScr.Coin += 250;
                isDie = true;
                // <�ؾ��� ó��>
                EnemyManager.Instance.DestroyEnemy("enemy", gameObject.name);
                //Debug.Log("�� ��Ȱ��ȭ");
                // �÷��̾� ����ġ ȹ��
                // ��ū ���� 
                //Die();
            }

        }
    }


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
        Debug.Log("ReinforcementAttack");
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
        {
            Debug.Log($"�ڷ���Ʈ ���� ���� : {distance}     {teleportFugitiveDist}");
            skillCondition[0] = true;
        }
            
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
            hadExcutedDimensionalConnection = true;
        }
    }

    /// <summary>
    /// Skill Teleport �ִϸ��̼ǽ� ���� ���� ��ġ�� ȣ�� 
    /// </summary>
    private void Teleport()
    {
        Vector3 teleportPos = transform.position + (enemyTr.position - playerObj.transform.position).normalized * teleportRunDist;
        teleportPos.y = transform.position.y;

        enemyTr.position = teleportPos;
        Debug.Log("TelePort");
    }

    private void FolderGeist()
    {
        float damage = atk * (200 / defaultAtkPercent); //atk * (450 / defaultAtkPercent)
        
        CircleRangeScr.StartRangeCheck("KnockBack", gameObject.name, 5, 3, 1, damage, 1);
        Debug.Log("FolderGeist");
    }

    private void DimensionalConnection()
    {
        //MaxShield = maxHP * 0.2f;
        Debug.Log("DimensionalConnection");
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
        if (currHitCnt >= maxHitCnt)
        {
            SetStateEffect("Stun", 3, Vector3.zero);
            Debug.Log("�ǽ� ����");
        }
            
        else // �ǽ� ����
        {
            Debug.Log("�ǽ� ����");
            for(int i=0; i< jiruruCnt; i++)
                EnemyManager.Instance.ActivateEnemy("Jiruru");
        }

        SkillAniEnd();
    }
}
