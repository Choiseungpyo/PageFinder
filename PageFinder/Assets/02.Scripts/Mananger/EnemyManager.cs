using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;


public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private GameObject[] Enemies_Prefab;

    private List<GameObject> Enemies = new List<GameObject>();

    [SerializeField]
    private GameObject enemyHPCanvas_Prefab;
    [SerializeField]
    private GameObject targetEnemyHPCanvas_Prefab;


    PageMap pageMap;
    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();
        CreateRooms(5);
    }


    /// <summary>
    /// �ִ� ���� ������ŭ EnemyManager�� �ڽ� ��ü�� �����Ѵ�.
    /// </summary>
    /// <param name="maxMapNum">���� �ִ� ����</param>
    public void CreateRooms(int maxMapNum)
    {
        GameObject map;
        for (int i=0; i< maxMapNum; i++)
        {
            map = new GameObject((i+1).ToString());
            map.transform.parent = transform;
        }
    }

    /// <summary>
    /// ���� �����Ѵ�.
    /// </summary>
    /// <param name="mapNum">���� �����Ǵ� �� ��ȣ</param>
    /// <param name="type">���� ����</param>
    /// <param name="pos">�� ���� ��ġ</param>
    /// <returns>���� ���� ���� �̸�</returns>
    public string CreateEnemy(int mapNum, string type, Vector3 spawnPos, Vector3 dir, int moveDist = 5, bool isTargetEnemy = false)
    {
        int index = 0;
        GameObject obj = null;
        mapNum--;

        switch (type)
        {
            case "Jiruru":
                index = 0;
                break;
            case "Bansha":
                index = 1;
                break;
            case "Witched":
                index = 2;
                break;

            default:
                Debug.LogWarning(type);
                break;
        }

        //if (transform.childCount == 0)
        //    ClassifyMap(mapNum);

        if(isTargetEnemy)
        {
            obj = Instantiate(Enemies_Prefab[index], spawnPos, Quaternion.Euler(dir), transform.GetChild(mapNum));
            Instantiate(targetEnemyHPCanvas_Prefab, Vector3.zero, Quaternion.identity, obj.transform);
            obj.transform.localScale = Vector3.one * 1.2f;
            obj.name = "Target-" + type;
        }
        else
        {
            obj = Instantiate(Enemies_Prefab[index], spawnPos, Quaternion.Euler(dir), transform.GetChild(mapNum));
            Instantiate(enemyHPCanvas_Prefab, obj.transform.position + Vector3.up * 2, Quaternion.identity, obj.transform);
            obj.name = type;

            index = 0;
            for (int i = 0; i < transform.GetChild(mapNum).childCount; i++)
            {
                if (transform.GetChild(mapNum).GetChild(i).name.Contains(type))
                    index++;
            }
            obj.name = type + index;
        }
        obj.GetComponent<Enemy>().MoveDist = moveDist;


        Enemies.Add(obj);

        return obj.name;
    }

    public void DestroyEnemy(string name)
    {
        GameObject obj;
        if (!name.Contains("Target"))
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].name.Equals(name))
                {
                    obj = Enemies[i];
                    Enemies.RemoveAt(i);
                    Destroy(obj);
                    break;
                }
            }
        }
        else
        {
            // ��� �� �ı�
            for (int i = 0; i < Enemies.Count; i++)
            {
                obj = Enemies[i];
                Enemies.RemoveAt(i);
                Destroy(obj);
            }
            pageMap.SetPageClearData();
        }
    }

    public void DeactivateEnemy(string name)
    {
        for(int i=0; i< Enemies.Count; i++)
        {
            if (Enemies[i].name.Equals(name))
            {
                Enemies[i].GetComponent<Enemy>().StopAllCoroutines();
                Enemies[i].SetActive(false);
                    
                break;
            }
        }
    }

    public void ActivateEnemy(string EnemyType)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].name.Contains(EnemyType) && !Enemies[i].activeSelf)
            {
                Enemies[i].SetActive(true);
                break;
            }
        }
    }

    public void SetEnemyAboutCurrPageMap(int mapNum, Page page)
    {
        for (int i = 0; i < page.enemyTypes.Length; i++)
            CreateEnemy(mapNum, page.enemyTypes[i], page.enemySpawnPos[i], page.enemyDir[i], page.enemyMoveDist[i]);

        CreateEnemy(mapNum, page.targetEnemyType, page.targetEnemySpawnPos, page.targetEnemyDir, page.targetEnemyMoveDist, true);
    }
}
