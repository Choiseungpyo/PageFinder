using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataLeaderBase : ScriptableObject
{
    public abstract class DataReaderBase : ScriptableObject
    {
        [Header("��Ʈ�� �ּ�")][SerializeField] public string associatedSheet = "";
        [Header("�������� ��Ʈ�� ��Ʈ �̸�")][SerializeField] public string associatedWorksheet = "";
        [Header("�б� ������ �� ��ȣ")][SerializeField] public int START_ROW_LENGTH = 2;
        [Header("���� ������ �� ��ȣ")][SerializeField] public int END_ROW_LENGTH = -1;
    }
}
