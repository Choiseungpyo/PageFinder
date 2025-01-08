using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ��ũ��ũ ������Ʈ Ǯ���� ����ϴ� Ŭ����
/// </summary>
public class InkMarkPooler : Singleton<InkMarkPooler> 
{
    public GameObject inkMarkPrefab;
    private ObjectPool<InkMark> pool;
    public int maxPoolSize = 40;
    public int defaultPoolCapacity = 10;

    public override void Awake()
    {
        base.Awake();
        pool = new ObjectPool<InkMark>(CreatedPooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolCapacity, maxPoolSize);
    }

    // ������Ʈ Ǯ ������ �ʱ� ��ü �Ҵ� �̺�Ʈ �Լ�
    private InkMark CreatedPooledItem()
    {
        var inkMarkObj = Instantiate(inkMarkPrefab);

        InkMark inkMark = inkMarkObj.AddComponent<InkMark>();
        inkMarkObj.name = "InkMark";

        return inkMark;
    }

    // ������Ʈ Ǯ�� ��� ��ü ��ȯ
    private void OnReturnedToPool(InkMark inkMark)
    {
        inkMark.gameObject.SetActive(false);
    }

    // ������Ʈ Ǯ���� ��ü ������
    private void OnTakeFromPool(InkMark inkMark)
    {
        inkMark.gameObject.SetActive(true);
    }

    // ������Ʈ Ǯ ��ü ����
    private void OnDestroyPoolObject(InkMark inkMark)
    {
        Destroy(inkMark.gameObject);
    }
}
