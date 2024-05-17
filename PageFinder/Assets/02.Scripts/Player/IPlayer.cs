using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{

    // ������ ���� ���ϱ�
    public Vector3 CaculateDirection(Collider goalObj);
    // ���� �������� ȸ��
    public void TurnToDirection(Vector3 dir);
}
