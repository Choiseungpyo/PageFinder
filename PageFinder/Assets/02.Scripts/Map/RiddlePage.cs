using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RiddlePage : Page
{
    // 0��°�� Target, �������� �Ϲ� ��
    public string[] types = { "" };
    public Vector3[] spawnPos = { Vector3.zero };
    public float[] maxHp;
    public float[] playerCognitiveDist;// �÷��̾� ���� �Ÿ�
    public float[] fugitiveCognitiveDist; // ������ ���� �Ÿ�
    public float[] moveDistance;
    public float[] moveSpeed;

    public Vector3[] rallyPoints;
}
