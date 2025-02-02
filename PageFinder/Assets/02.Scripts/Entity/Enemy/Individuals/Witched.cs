using Google.GData.AccessControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Witched : MediumBossEnemy
{
    [Header("Skill - Teleport")]
    [SerializeField]
    private float teleportRunDist;
    [SerializeField]
    private float teleportFugitiveDist;

    [Header("Circle Range")]
    [SerializeField]
    private CircleRange CircleRangeScr;

    private bool firstRunAboutSkill2;

    [SerializeField]
    private GameObject teleportEffectObj;

    // ��ȭ ����
    [SerializeField]
    private GameObject bulletPrefab;

    //�������̽�Ʈ ��ų ����� ����
    bool isSkill1InUse;

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

            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerState.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerState.Coin += 100;
                else
                    playerState.Coin += 250;

                // <�ؾ��� ó��>
                EnemyPooler.Instance.ReleaseEnemy(enemyType, gameObject);
                //Debug.Log("�� ��Ȱ��ȭ");
                // �÷��̾� ����ġ ȹ��
                // ��ū ���� 
                //Die();
            }

        }
    }

    protected override void InitStat()
    {
        base.InitStat();

        skillConditions[1] = true;
        firstRunAboutSkill2 = false;

        teleportEffectObj.SetActive(false);
        isSkill1InUse = false;
    }

    protected override void BasicAttack()
    {
        SetBullet(bulletPrefab, 0, atk);
    }

    /// <summary>
    /// ��ȭ ���� �ִϸ��̼� ���� �� ��ȭ ���� ���۽� ȣ���ϴ� �Լ�
    /// </summary>
    protected override void ReinforcementAttack()
    {
        int[] angles = { -60, 0, 60 };

        foreach (int angle in angles)
            SetBullet(bulletPrefab, angle, atk);
    }

    protected override void CheckSkillsCondition()
    {
        CheckTeleportCondition();
        CheckDimensionalConnection();
    }

    private void CheckTeleportCondition()
    {
        float distance = Vector3.Distance(new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z), playerObj.transform.position);

        // ���� �Ÿ��� �������� �� + ��ų ������ Ȱ��ȭ���� �ʾ��� ��
        if (distance <= teleportFugitiveDist && !skillConditions[0])
        {
            //Debug.Log($"�ڷ���Ʈ ���� ���� : {distance}     {teleportFugitiveDist}");
            skillConditions[0] = true;
        }
    }

    private void CheckDimensionalConnection()
    {
        if (firstRunAboutSkill2)
            return;

        if (currHP < maxHP * 0.4 && !skillConditions[2])
        {
            firstRunAboutSkill2 = true;
            skillConditions[2] = true;
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
        if (isSkill1InUse)
            return;

        isSkill1InUse = true;
        float damage = atk * 2; //atk * (450 / defaultAtkPercent)
        Debug.Log(damage);
        CircleRangeScr.StartRangeCheck(1, Enemy.DebuffState.STUN, 5, 2, damage, 1);
    }

    public void DimensionalConnection()
    {
        //MaxShield = maxHP * 0.2f;
        //Debug.Log("DimensionalConnection");
    }

    public override void SkillEnd()
    {
        switch (currSkillNum)
        {
            // �ڷ���Ʈ
            case 0:
                break;

            // ���� ���̽�Ʈ
            case 1:
                // ���� ������ ��ȭ ������ �ǵ��� �Ѵ�.
                currBasicAtkCnt = reinforcementAtkCnt;
                isSkill1InUse = false;
                break;

            // ���� ����
            case 2:
                GameData.Instance.CurrWaveNum += 1;
                // 2���̺�� �Ѿ�鼭 ����� 4���� ��ȯ
                break;

            default:
                break;
        }

        base.SkillEnd();
    }
}
