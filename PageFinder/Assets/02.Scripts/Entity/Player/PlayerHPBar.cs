using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : SliderBar
{
    public GameObject Gradation_Prefab;

    List<GameObject> gradations = new List<GameObject>();
    int gradationCntPerHp = 200;

    Player player;

    // Start is called before the first frame update
    private void Start()
    {
        player  = GameObject.FindWithTag("PLAYER").GetComponent<Player>();
        SetGradation();
    }

    /// <summary>
    /// ������ �����Ѵ�.
    /// </summary>
    public void SetGradation()
    {
        Transform gradationSet = transform.GetChild(2);
        int gradationCnt = (int)(player.MAXHP / gradationCntPerHp);
        GameObject gradation;
        float[] standardX = { -1.126f, 1.19f };
        float intervalX = (-standardX[0] + standardX[1]) / (gradationCnt + 1);
        Vector3 pos;

        // �ּҰ� : -37.81    �ִ밪 : 39.58
        for (int i = 0; i < gradations.Count; i++)
            Destroy(gradations[i]);

        gradations.Clear();
        
        for (int i=0; i< gradationCnt; i++)
        {
            pos = new Vector3(standardX[0] + intervalX * (i+1) , 0f, 0);
            gradation = Instantiate(Gradation_Prefab, transform.position, Quaternion.identity, gradationSet);
            gradation.transform.localPosition = pos;

            if(gradationCnt % 10 == 0) // ���� ������ 10�� ����϶����� ���� ũ�� ���̱�
                gradation.transform.localScale = new Vector3(0.005f - 0.001f * (gradationCnt / 10), 0.003f, 1);
            gradations.Add(gradation);
        }
    }
}
