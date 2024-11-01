using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattlePage : Page
{
    // �ϴ� �̿��ϱ� ���� public���� ����
    public string[] enemyTypes = { "" };

    public Vector3[] enemySpawnPos = { Vector3.zero };

    public Vector3[] enemyDir = { Vector3.zero };

    public int[] enemyMoveDist = { 0 };


    public string targetEnemyType;

    public Vector3 targetEnemySpawnPos;

    public Vector3 targetEnemyDir = Vector3.zero;

    public int targetEnemyMoveDist = 0;

}
