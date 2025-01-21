using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattlePage : Page
{
    // �ϴ� �̿��ϱ� ���� public���� ����
    public string[] types = { "" };

    public Vector3[] spawnPos = { Vector3.zero };

    public Vector3[] dir = { Vector3.zero };

    public int[] moveDist = { 0 };

    public int[] maxHP;
    public int[] atk;
    public float[] atkSpeed;
}
