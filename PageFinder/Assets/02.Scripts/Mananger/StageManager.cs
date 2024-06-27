using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour
{
    public Door[] DoorSrc = new Door[3];
    public Vector3[] stageStartPos = new Vector3[3];
    int currentStage = 0;

    GameObject Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("PLAYER");
        Player.transform.position = stageStartPos[0]; // �÷��̾� �������� 1 ���� ��ġ���� ����
    }

    private void Update()
    {
        if (Player.transform.position.z <= -36) // �ش� �������� Ŭ���� �� �� �ٱ����� ���� ���
        {
            if (currentStage >= 3) // ������ ��������
                currentStage = 0; // �� ó�� ���������� �̵��ϰ� ����

            Debug.Log("�̵��� �������� : "+stageStartPos[currentStage]);
            Player.transform.position = stageStartPos[currentStage];
        }
            
    }


    public void ClearStage(int i)
    {
        currentStage = i+1;
        DoorSrc[i].StartCoroutine(DoorSrc[i].Open());
    }
}
