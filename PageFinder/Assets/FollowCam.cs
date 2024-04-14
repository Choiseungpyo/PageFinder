using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    private Transform camTr;

    // ���� ������κ��� ������ �Ÿ�
    [Range(1.0f, 20.0f)]
    public float distance = 1.0f;

    // Y������ �̵��� ����
    public float height = 20.0f;

    // ���� �ӵ�
    public float damping = 10.0f;

    // SmoothDamp���� ����� ����
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = targetTr.position + new Vector3(0, height, -distance);

        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);

    }
}
