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
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = targetTr.position + new Vector3(0, height, -distance);
        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);

        HideObject();
    }

    /// <summary>
    /// �÷��̾�� ī�޶� ������ �ִ� ������Ʈ�� ������ �Լ�
    /// </summary>
    void HideObject()
    {
        Vector3 direction = (targetTr.position - transform.position).normalized;
        // �÷��̾�� ī�޶� ������ ��� ȯ�� ��ֹ��� ã�´�.
        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, 
            direction, 
            Mathf.Infinity, 
            1 << LayerMask.NameToLayer("EnvironmentObject")
        );

        // ã�� ��ֹ����� ����ȭ�Ѵ�.
        for(int i = 0; i <hits.Length; i++)
        {
            TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();
            for(int j = 0; j < obj.Length; j++)
            {
                obj[j].BecomeTransParent();
            }
        }
    }
}
