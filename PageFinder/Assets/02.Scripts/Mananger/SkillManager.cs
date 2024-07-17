using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ų �Ŵ���
/// ��ų �����Ϳ� ��ų �������� ������.
/// ��ų �����Ϳ� ��ų �������� ���� ���� ���۽� ����.
/// �� ���� Singleton���� �����Ͽ� ��� �ı����� �ʰ� ����.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{

    private GameObject[] skillObjects;
    private ScriptableObject[] skillDatas;
    // ��ų ������ ��ųʸ� 
    private Dictionary<string, GameObject> skillPrefabDic;
    // ��ų ������ ��ųʸ�
    private Dictionary<string, SkillData> skillDataDic;

    public override void Awake()
    {
        base.Awake();

        skillPrefabDic = new Dictionary<string, GameObject>();
        skillDataDic = new Dictionary<string, SkillData>();

        // ��ų ������ �ε�
        LoadSkillPrefabs();
        // ��ų ������ �ε�
        LoadSkillDatas();
    }

    public void Start()
    {

    }

    /// <summary>
    /// ��ų �������� �ε��ϴ� �Լ�
    /// ���� 1���� ����
    /// </summary>
    private void LoadSkillPrefabs()
    {
        skillObjects = Resources.LoadAll<GameObject>("SkillPrefabs");
        if (skillObjects == null) return;   // ������ ��ų �������� ������ return
        for (int i = 0; i < skillObjects.Length; i++)
        {
            skillPrefabDic.Add(skillObjects[i].name, skillObjects[i]);
        }
    }

    /// <summary>
    /// ��ų �����͸� �ε��ϴ� �Լ�
    /// ���� 1���� ����
    /// </summary>
    private void LoadSkillDatas()
    {
        skillDatas = Resources.LoadAll<ScriptableObject>("SkillDatas");
        if (skillDatas == null) return; // ������ ��ų �����Ͱ� ������ return
        SkillData skillData;
        for (int i = 0; i < skillDatas.Length; i++)
        {
            skillData = skillDatas[i] as SkillData;
            if (skillData == null) continue; // skillData�� ����ȯ�� �����ϸ� continue
            skillDataDic.Add(skillData.name, skillData);
        }
    }

    /// <summary>
    /// ������ �̸��� ��ų �������� �޾ƿ��� �Լ�
    /// </summary>
    /// <param name="skillPrefabName">������ ��ų ������ �̸�</param>
    /// <returns>������ �̸��� ��ų �������� ��ųʸ��� ������ ��� �ش� ��ų ������ ��ȯ, �׷��� ���� ��� null��ȯ</returns>
    public GameObject GetSkillPrefab(string skillPrefabName)
    {
        if (skillPrefabDic.ContainsKey(skillPrefabName))
        {
            return skillPrefabDic[skillPrefabName];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// ������ �̸��� ��ų �����͸� �޾ƿ��� �Լ�
    /// </summary>
    /// <param name="skillDataName">������ ��ų ������ �̸�</param>
    /// <returns>������ �̸��� ��ų �����Ͱ� ��ųʸ��� ������ ��� �ش� ��ų ������ ��ȯ, �׷��� ���� ��� null��ȯ</returns>
    public SkillData GetSkillData(string skillDataName)
    {
        if (skillDataDic.ContainsKey(skillDataName))
        {
            return skillDataDic[skillDataName];
        }
        else
        {
            return null;
        }
    }
}
