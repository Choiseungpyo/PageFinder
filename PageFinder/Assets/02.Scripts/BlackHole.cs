using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    TokenManager tokenManager;

    public void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }
    /// <summary>
    /// ���� ���¸� �����Ѵ�. 
    /// </summary>
    /// <param name="value">������ Ȱ��ȭ ���� ��</param>
    public void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            tokenManager.StorageCurrentToken();
        }
    }
}
