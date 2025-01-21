using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiddlePageData : ScriptableObject
{
    public enum PageType
    {
        BATTLE,
        TRANSACTION,
        RIDDLE,
        MIDDLEBOSS
    }

    public PageType pageType;

    public string pageDataName;

    public bool isClear;

    public Vector3 playerSpawnPos;

    // 0��°�� Target, �������� �Ϲ� ��
    public string[] types = { "" };
    public Vector3[] spawnPos = { Vector3.zero };
    public float[] maxHp;
    public float[] playerCognitiveDist;// �÷��̾� ���� �Ÿ�
    public float[] fugitiveCognitiveDist; // ������ ���� �Ÿ�
    public float[] moveDist;
    public float[] moveSpeed;

    public Vector3[] rallyPoints;
}
