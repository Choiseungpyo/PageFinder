using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RiddlePage : Page
{
    public string[] enemyTypes = { "" };
    public Vector3[] enemySpawnPos = { Vector3.zero };

    public string targetEnemyType;
    public Vector3 targetEnemySpawnPos;


    public float[] playerCognitiveDist;// �÷��̾� ���� �Ÿ�
    public float[] fugitiveCognitiveDist; // ������ ���� �Ÿ�
    public float[] moveDistance;
    public float[] moveSpeed;

    public float target_playerCognitiveDist;
    public float target_fugitiveCognitiveDist;
    public float target_moveDistance;
    public float target_moveSpeed;

    public float target_hp;

    public Vector3[] rallyPoints;
}
