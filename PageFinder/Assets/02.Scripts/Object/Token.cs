using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    // ��ũ��Ʈ ����
    private void Awake()
    {
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("PLAYER"))
            return;

        // �÷��̾�� �浹���� ���
        SetActiveState(false);
    }

    /// <summary>
    /// ��ū ������Ʈ Ȱ��ȭ ���¸� �����Ѵ�. 
    /// </summary>
    /// <param name="value">������ ���� ��</param>
    void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }
}
