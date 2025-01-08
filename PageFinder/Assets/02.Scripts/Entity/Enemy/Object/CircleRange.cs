using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;

public class CircleRange : MonoBehaviour
{
    GameObject circleToGrowInSize;
    GameObject targetCircle;

    Enemy.AbnomralState abnormalState;
    float targetCircleSize;
    float abnormalTime;
    float damage;
    float moveDist;
    int skillIndex;

    [SerializeField]
    private Transform subjectPos;

    Player playerScr;
    MediumBossEnemy mediumBossEnemy; // ���� ������ �߰����� �̻󿡼��� ���� ��ų�� ����ϱ� ������ MediumBossEnemy�� ���� 

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
        mediumBossEnemy = DebugUtils.GetComponentWithErrorLogging<MediumBossEnemy>(transform.parent.gameObject, "MediumBossEnemy");
    }

    private void Start()
    {
        circleToGrowInSize = transform.GetChild(0).gameObject;
        targetCircle = transform.GetChild(1).gameObject;
        gameObject.SetActive(false);
    }

   /// <summary>
   /// ���� üũ ����
   /// </summary>
   /// <param name="skillIndex">��ų �ε���</param>
   /// <param name="stateEffectName">������ ���� ȿ��</param>
   /// <param name="subjectName">ȣ���� ��ü �̸�</param>
   /// <param name="targetCircleSize">��ǥ ����</param>
   /// <param name="speed">���� ä������ �ӵ�</param>
   /// <param name="abnormalTime">����ȿ�� ����ð�</param>
   /// <param name="damage">���� ������</param>
   /// <param name="moveDist">Ž�� �Ÿ�</param>
    public void StartRangeCheck(int skillIndex, Enemy.AbnomralState abnormalState, float targetCircleSize, float abnormalTime, float damage, float moveDist = 0)
    {        
        this.skillIndex = skillIndex;
        this.abnormalState = abnormalState;
        this.targetCircleSize = targetCircleSize * 2;
        this.abnormalTime = abnormalTime;
        this.damage = damage;
        this.moveDist = moveDist;

        targetCircle.transform.localScale = Vector3.one * this.targetCircleSize; 
        circleToGrowInSize.transform.localScale = Vector3.one;

        gameObject.SetActive(true);

        StartCoroutine(GrowSizeOfCircle());
    }

    /// <summary>
    /// ��ǥ�� ũ����� ���� ũ�⸦ ������Ų��.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrowSizeOfCircle()
    {
        while (circleToGrowInSize.transform.localScale.x < targetCircleSize)
        {
            // Time.deltaTime * targetCircleSize/2 : 1�ʴ� �� ���� ���� Ŀ������ ��
            // ���Ѵٸ� speed �߰��ؼ� ������ ���� ����
            circleToGrowInSize.transform.localScale = Vector3.MoveTowards(circleToGrowInSize.transform.localScale, Vector3.one * targetCircleSize, Time.deltaTime * targetCircleSize/2);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

         CheckObjectsInRange();
        
        gameObject.SetActive(false);
    }


    /// <summary>
    /// ���� �ȿ� �ִ� ������Ʈ�� üũ�Ѵ�.
    /// </summary>
    void CheckObjectsInRange()
    {
        Collider[] hits = Physics.OverlapSphere(subjectPos.position, 9, LayerMask.GetMask("ENEMY", "PLAYER"));

        for (int i = 0; i < hits.Length; i++)
        {
            // ���� ������ ������ �ڽ��� ���� ����
            if (hits[i].name.Equals(transform.parent.name))
                continue;

            // ��
            if (hits[i].CompareTag("ENEMY"))
            {
                //Debug.Log(hits[i].name + stateEffectName);
                Enemy hitEnemyScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(hits[i].gameObject, "Enemy");

                // ���� ȿ�� 
                switch (abnormalState)
                {
                    case Enemy.AbnomralState.STUN:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, Vector3.zero);
                        break;

                    case Enemy.AbnomralState.KNOCKBACK:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, hits[i].transform.position + (hits[i].transform.position - subjectPos.position).normalized * moveDist);
                        break;

                    case Enemy.AbnomralState.BINDING:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, Vector3.zero);
                        break;

                    case Enemy.AbnomralState.AIR:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, hits[i].transform.position + Vector3.up * moveDist);
                        break;

                    default:
                        //hitEnemyScr.SetStateEffect(Enemy.AbnomralState.NONE, abnormalTime, Vector3.zero);
                        break;
                }

                hitEnemyScr.HP -= damage;
                continue;
            }

            // �÷��̾�
            playerScr.HP -= damage;
            // �÷��̾� ȿ�� ���� �Լ��� ���߿� ȣ���ϱ�
        }

        InitSkillAni();
    }

    /// <summary>
    /// ��ų �ִϸ��̼��� �ʱ�ȭ�Ѵ�.
    /// </summary>
    void InitSkillAni()
    {
        // ���� ������ ������ �ִϸ��̼� ���� �ʱ�ȭ
        switch (skillIndex)
        {
            case 0:
                mediumBossEnemy.Skill0AniEnd();
                break;

            case 1:
                mediumBossEnemy.Skill1AniEnd();
                break;

            case 2:
                mediumBossEnemy.Skill2AniEnd();
                break;
        }
    }
}
