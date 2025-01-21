using Google.GData.AccessControl;
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

    [SerializeField]
    private GameObject teleportEffectObj;


    public override float HP
    {
        get
        {
            return currHP; //+ currShield;  // 100 + 50   - 55
        }
        set
        {
            // ���ҽ��ѵ� ���尡 �����ִ� ���
            //if (value > currHP)
            //{
            //    CurrShield = value - currHP;
            //}
            //else // ���ҽ��ѵ� ���尡 �������� ���� ���
            //{
            //    CurrShield = 0;
            //    currHP = value;
            //}

            currHP = value;

            Hit();
            hpBar.SetCurrValueUI(currHP);
            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerState.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerState.Coin += 100;
                else
                    playerState.Coin += 250;
                isDie = true;
                // <�ؾ��� ó��>
                EnemyManager.Instance.DestroyEnemy("enemy", gameObject);
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
            Projectile projectTile = DebugUtils.GetComponentWithErrorLogging<Projectile>(ReinforcementAttackObjects[i], "Projectile");
            projectTile.Init(gameObject, "- ReinforcementAttackAction" + i, 10, reinforcementAttackPos.GetChild(i)); // 60�� 3����
        }

        skillCondition[1] = true;
        firstRunAboutSkill2 = false;

        teleportEffectObj.SetActive(false);

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
            Projectile projectTile = DebugUtils.GetComponentWithErrorLogging<Projectile>(ReinforcementAttackObjects[i], "Projectile");
            projectTile.Init(reinforcementAttackPos.GetChild(i).position - enemyTr.position);
        }
        //Debug.Log("ReinforcementAttack");
    }

    protected override void CheckSkillsCondition()
    {
        CheckTeleportCondition();
        CheckDimensionalConnection();

        if (abnormalState == AbnomralState.BINDING || abnormalState == AbnomralState.AIR)
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
        float distance = Vector3.Distance(new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z), playerObj.transform.position);

        // ���� �Ÿ��� �������� �� + ��ų ������ Ȱ��ȭ���� �ʾ��� ��
        if (distance <= teleportFugitiveDist && !skillCondition[0])
        {
            //Debug.Log($"�ڷ���Ʈ ���� ���� : {distance}     {teleportFugitiveDist}");
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
            //Debug.Log("Hp 40% �̸�");
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
        //Debug.Log("TelePort");
    }

    private void TeleportEffect()
    {
        StartCoroutine(StartTeleportEffect());
    }

    IEnumerator StartTeleportEffect()
    {
        teleportEffectObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        teleportEffectObj.SetActive(false);
    }


    private void FolderGeist()
    {
        float damage = atk * (200 / defaultAtkPercent); //atk * (450 / defaultAtkPercent)
        
        CircleRangeScr.StartRangeCheck(1, Enemy.AbnomralState.STUN, 5, 2, damage, 1);
        //Debug.Log("FolderGeist");
    }

    public void DimensionalConnection()
    {
        //MaxShield = maxHP * 0.2f;
        //Debug.Log("DimensionalConnection");
    }


    public override void Skill0AniEnd() 
    {
        // ���� ������ ��ȭ ������ �ǵ��� �Ѵ�.
        currDefaultAtkCnt = maxDefaultAtkCnt;

        SkillAniEnd();
    }


    public override void Skill2AniEnd()
    {
        for (int i = 0; i < jiruruCnt; i++)
            EnemyManager.Instance.ActivateEnemy("Jiruru");
        //SetStateEffect("Stun", 3, Vector3.zero);
        SkillAniEnd();
    }
}
