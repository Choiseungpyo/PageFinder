using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour
{
    public Door[] DoorSrc = new Door[3];
    public Vector3[] stageStartPos = new Vector3[3];

    [SerializeField]
    private int currentStage = 0;

    GameObject Player;
    FollowCam followCam; 

    private void Start()
    {
        Player = GameObject.FindWithTag("PLAYER");
        followCam = GameObject.Find("Camera").GetComponent<FollowCam>();
        Player.transform.position = stageStartPos[currentStage]; // �÷��̾� �������� 1 ���� ��ġ���� ����
    }

    private void Update()
    {
        if (Player.transform.position.z <= -36) // �ش� �������� Ŭ���� �� �� �ٱ����� ���� ���
        {
            if (currentStage >= 3) // ������ ��������
            {
                currentStage = 0; // �� ó�� ���������� �̵��ϰ� ����
                followCam.distance = 7;
                followCam.height = 8;
            }
            else if(currentStage == 2)
            {
                followCam.distance = 20;
                followCam.height = 15;
            }
            else
            {
                followCam.distance = 7;
                followCam.height = 8;
            }
            

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
