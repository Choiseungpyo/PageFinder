using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsManager :  Singleton<UtilsManager>
{
    Collider minDistObject;
    Collider[] objects;

    private void Start()
    {
        
    }
    /// <summary>
    /// ���� ����� �Ÿ��� ������Ʈ ���� ���� ������ ã��
    /// </summary>
    /// <param name="originPos">Ž�� ���� ����</param>
    /// <param name="searchDistance">Ž�� �Ÿ�</param>
    /// <param name="layer">Ž���� ���̾�</param>
    /// <returns>���� ����� �Ÿ��� ������Ʈ</returns>
    public Collider FindMinDistanceObject(Vector3 originPos, float searchDistance, int layer)
    {
        float minDist = searchDistance;
        minDistObject = null;
        objects = Physics.OverlapSphere(originPos, searchDistance, layer);
        foreach (Collider i in objects)
        {
            float dist = Vector3.Distance(originPos, i.gameObject.transform.position);
            if (minDist >= dist)
            {
                minDistObject = i;
                minDist = dist;
            }
        }
        return minDistObject;
    }
}
