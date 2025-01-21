using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    public GameObject bridgeSegment;  // �ٸ� ���� ������
    public Transform pointA;          // ���� ����
    public Transform pointB;          // �� ����
    public float segmentLength = 1.0f; // �� �ٸ� ������ ����

    void Start()
    {
        CreateBridge(pointA.position, pointB.position);
    }

    void CreateBridge(Vector3 start, Vector3 end)
    {
        // �� ���� ������ ���� ���
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction.Normalize();

        // �ٸ� ������ ���� ���
        int segmentCount = Mathf.CeilToInt(distance / segmentLength);

        for (int i = 0; i <= segmentCount; i++)
        {
            // �ٸ� ������ ��ġ ���
            Vector3 segmentPosition = start + direction * segmentLength * i;

            // �ٸ� ���� ����
            GameObject segment = Instantiate(bridgeSegment, segmentPosition, Quaternion.LookRotation(direction));
            segment.GetComponent<Transform>().Rotate(new Vector3(90, 0, 0));
        }
    }
}
