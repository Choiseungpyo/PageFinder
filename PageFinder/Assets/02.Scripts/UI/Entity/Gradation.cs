using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Gradation : MonoBehaviour
{
    [SerializeField]
    private GameObject Gradation_Prefab;

    [SerializeField]
    private int gradationCntPerHp = 100;

    List<GameObject> gradations = new List<GameObject>();

    private void Start()
    {
        AddGradation(10);
    }


    private void AddGradation(int cnt)
    {
        GameObject gradation;
        for (int i = 0; i < cnt; i++)
        {
            gradation = Instantiate(Gradation_Prefab, transform.position, Quaternion.identity, transform);
            gradations.Add(gradation);
        }

        for (int i = 0; i < gradations.Count; i++)
            gradations[i].SetActive(false);
    }


    /// <summary>
    /// ������ �����Ѵ�.
    /// </summary>
    public void SetGradation(float totalValue)
    {
        int gradationCnt = (int)(totalValue / gradationCntPerHp);
        float[] standardX = { -1, 1 };
        float intervalX = (-standardX[0] + standardX[1]) / (gradationCnt + 1);
        Vector3 pos;

        // �ּҰ� : -37.81    �ִ밪 : 39.58

        // ������ �����Ǿ� �ִ� �ͺ��� �����ϸ� �����Ѵ�. 
        if (gradations.Count < gradationCnt)
            AddGradation(gradationCnt - gradations.Count);


        for (int i = 0; i < gradationCnt; i++)
        {
            pos = new Vector3(standardX[0] + intervalX * (i + 1), 0f, 0);
            gradations[i].transform.localPosition = pos;
            gradations[i].SetActive(true);

            if (gradationCnt % 10 == 0) // ���� ������ 10�� ����϶����� ���� ũ�� ���̱�
                gradations[i].transform.localScale = new Vector3(0.005f - 0.001f * (gradationCnt / 10), 0.004f, 1);

        }
    }
}
