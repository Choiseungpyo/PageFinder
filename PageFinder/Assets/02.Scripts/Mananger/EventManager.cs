using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������ ���� �̺�Ʈ�� ��� ���.
public enum EVENT_TYPE {
    GAME_INIT, 
    GAME_END
}

public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// �̺�Ʈ�� ���� ��������Ʈ ������ �����Ѵ�.
    /// </summary>
    /// <param name="Event_Type">�߻��� �̺�Ʈ ����</param>
    /// <param name="Sender">�̺�Ʈ �߻� Ȯ�� �۽���</param>
    /// <param name="Param">���� ������ �Ķ����</param>
    public delegate void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null);

    // �̺�Ʈ ������ ������Ʈ�� ��ųʸ�(��� ������Ʈ�� �̺�Ʈ ������ ���� ��ϵǾ� ����)
    private Dictionary<EVENT_TYPE, List<OnEvent>> Listeners =
        new Dictionary<EVENT_TYPE, List<OnEvent>>();
    
    /// <summary>
    /// ������ �迭�� ������ ������ ������Ʈ�� �߰��ϱ� ���� �Լ�
    /// </summary>
    /// <param name="Event_Type">������ �̺�Ʈ</param>
    /// <param name="Listner">�̺�Ʈ�� ������ ������Ʈ</param>
    public void AddListener(EVENT_TYPE Event_Type, OnEvent Listner)
    {
        // �� �̺�Ʈ�� ������ �������� ����Ʈ
        List<OnEvent> ListenList = null;

        // �̺�Ʈ ���� Ű�� �����ϴ��� �˻��ϰ� �����ϸ� ����Ʈ�� �߰��Ѵ�.
        if(Listeners.TryGetValue(Event_Type, out ListenList))
        {
            // ����Ʈ�� �����ϸ� �� �׸��� �߰��Ѵ�
            ListenList.Add(Listner);
            return;
        }

        // ����Ʈ�� �������� ������ ���ο� ����Ʈ�� �����Ѵ�.
        ListenList = new List<OnEvent>();
        ListenList.Add(Listner);
        Listeners.Add(Event_Type, ListenList);
    }

    /// <summary>
    /// �̺�Ʈ�� �����ʿ��� �����ϱ� ���� �Լ�
    /// </summary>
    /// <param name="Event_Type">�ҷ��� �̺�Ʈ</param>
    /// <param name="Sender">�̺�Ʈ�� �θ��� ������Ʈ</param>
    /// <param name="Param">���� ������ �Ķ����</param>
    public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        // ��� �����ʿ��� �̺�Ʈ�� ���� �˸���.

        // �� �̺�Ʈ�� �����ϴ� �����ʵ��� ����Ʈ
        List<OnEvent> ListenList = null;

        // �̺�Ʈ �׸��� ������, �˸� �����ʰ� �����Ƿ� ������.
        if (!Listeners.TryGetValue(Event_Type, out ListenList))
            return;

        // �׸��� �����ϸ� ������ �����ʿ��� �˷��ش�.
        for(int i = 0; i < ListenList.Count; i++)
        {
            // ������Ʈ�� null�� �ƴϸ� �������̽��� ���� �޽����� ������.
            if (!ListenList[i].Equals(null))
                ListenList[i](Event_Type, Sender, Param);
        }
    }

    /// <summary>
    /// �̺�Ʈ ������ ������ �׸��� ��ųʸ����� �����Ѵ�.
    /// </summary>
    /// <param name="Event_Type">������ �̺�Ʈ</param>
    public void RemoveEvent(EVENT_TYPE Event_Type)
    {
        // ��ųʸ��� �׸��� �����Ѵ�.
        Listeners.Remove(Event_Type);
    }

    /// <summary>
    /// ��ųʸ����� ������� �׸���� �����Ѵ�.
    /// </summary>
    public void RemoveRedundancies()
    {
        // �� ��ųʸ� ����
        Dictionary<EVENT_TYPE, List<OnEvent>> TmpListeners
            = new Dictionary<EVENT_TYPE, List<OnEvent>>();

        // ��� ��ųʸ� �׸��� ��ȸ�Ѵ�
        foreach(KeyValuePair<EVENT_TYPE, List<OnEvent>> Item in Listeners)
        {
            // ����Ʈ�� ��� ������ ������Ʈ�� ��ȸ�ϸ� null ������Ʈ�� �����Ѵ�
            for(int i = Item.Value.Count-1; i>=0; i--)
            {
                // null�̸� �׸��� �����
                if (Item.Value[i].Equals(null))
                    Item.Value.RemoveAt(i);
            }

            // �˸��� �ޱ� ���� �׸�鸸 ����Ʈ�� ������ �� �׸���� �ӽ� ��ųʸ��� ��´�.
            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);

            // ���� ����ȭ�� ��ųʸ��� ��ü�Ѵ�.
            Listeners = TmpListeners;
        }
    }

    /// <summary>
    /// ���� ����� �� ȣ��ȴ�. ��ųʸ��� û���Ѵ�.
    /// </summary>
    private void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
}