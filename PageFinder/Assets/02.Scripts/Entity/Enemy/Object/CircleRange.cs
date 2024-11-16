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

    string stateEffectName;
    string subjectName;
    float targetCircleSize;
    float speed;
    float abnormalTime;
    float damage;
    float moveDist;


    [SerializeField]
    private Transform subjectPos;

    Player playerScr;

    private void Start()
    {
        playerScr = GameObject.FindWithTag("PLAYER").GetComponent<Player>();

        circleToGrowInSize = transform.GetChild(0).gameObject;
        targetCircle = transform.GetChild(1).gameObject;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���� üũ�� �����Ѵ�.
    /// </summary>
    /// <param name="stateEffect"></param>
    /// <param name="subjectName"></param>
    /// <param name="targetCircleSize"></param>
    /// <param name="speed"></param>
    /// <param name="defaultCircleSize"></param>
    public void StartRangeCheck(string stateEffectName, string subjectName, float targetCircleSize, float speed, float abnormalTime, float damage, float moveDist = 0)
    {
        this.stateEffectName = stateEffectName;
        this.subjectName = subjectName;
        this.targetCircleSize = targetCircleSize;
        this.speed = speed;
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
            circleToGrowInSize.transform.localScale = Vector3.MoveTowards(circleToGrowInSize.transform.localScale, Vector3.one * targetCircleSize, Time.deltaTime * speed);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

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
            if (hits[i].name.Equals(subjectName))
                continue;

            Debug.Log("�� �� ���� �ȿ� ���� �� : "+hits[i].name);

            // ��
            if (hits[i].CompareTag("ENEMY"))
            {
                //Debug.Log(hits[i].name + stateEffectName);
                // ���� ȿ�� 

                switch (stateEffectName)
                {
                    case "KnockBack":
                        hits[i].GetComponent<Enemy>().SetStateEffect(stateEffectName, abnormalTime, hits[i].transform.position + (hits[i].transform.position - subjectPos.position).normalized * moveDist);
                        break;
                    case "Air":
                        hits[i].GetComponent<Enemy>().SetStateEffect(stateEffectName, abnormalTime, hits[i].transform.position + Vector3.up * moveDist);
                        break;
                    default:
                        hits[i].GetComponent<Enemy>().SetStateEffect(stateEffectName, abnormalTime, Vector3.zero);
                        break;
                }

                hits[i].GetComponent<Enemy>().HP -= damage;
                continue;
            }

            //Debug.Log("�÷��̾ ���� ���� �ȿ� �����ֽ��ϴ�." + Vector3.Distance(subjectPos.position, playerScr.transform.position));
            //Debug.Log(hits[i].name);

            // �÷��̾�
            playerScr.HP -= damage;
            // �÷��̾� ȿ�� ���� �Լ��� ���߿� ȣ���ϱ�

        }
    }

}
