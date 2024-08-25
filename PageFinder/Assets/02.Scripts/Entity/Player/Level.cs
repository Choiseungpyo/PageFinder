using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int currentLevel = 0;

    public GameObject[] levelUIObject;

    private void Start()
    {
        IncreaseCurrentLevel(1);
    }

    /// <summary>
    /// ���� ������ �����Ѵ�. 
    /// </summary>
    /// <returns></returns>
    public int ReturnCurrentLevel()
    {
        return currentLevel;
    }

    /// <summary>
    /// ���� ������ ������Ų��.
    /// </summary>
    /// <param name="value">������ų ��</param>
    public void IncreaseCurrentLevel(int value)
    {
        currentLevel += value;

        for (int i=0; i< levelUIObject.Length; i++)
        {
            levelUIObject[i].GetComponent<LevelUIManager>().SetLevel_Txt(currentLevel);
        }
    }
}
