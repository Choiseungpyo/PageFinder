using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameData : Singleton<GameData>
{
    private List<List<List<List<EnemyData>>>> stageData
        = new List<List<List<List<EnemyData>>>>();

    [SerializeField]
    private int currStageNum;
    [SerializeField]
    private int currPageNum;
    [SerializeField]
    private int currWaveNum;

    public int CurrStageNum // ���� ���� ����
    {
        get { return currStageNum; }
        set { currStageNum = value; }
    }

    public int CurrPageNum // ��Ż Ÿ�� ����
    {
        get { return currPageNum; }
        set { currPageNum = value; }
    }

    public int CurrWaveNum // ������ ������ ����
    {
        get { return currWaveNum; }
        set 
        { 
            currWaveNum = value;
            EnemySetter.Instance.SpawnEnemy();
            Debug.Log($"Wave {currWaveNum}�� ���� : Enemy Spawn");
        }
    }

    private void Start()
    {
        EnemyCSVReader.Instance.ReadCSV();
    }

    public void SetStageData(ref EnemyData enemyData, int stageNum, int pageNum, int waveNum)
    {
        List<List<List<EnemyData>>> pageData;

        // ���������� �ִ� ���
        if (stageNum > 0 && stageNum <= stageData.Count())
            pageData = stageData[stageNum-1];
        else
        {
            pageData = new List<List<List<EnemyData>>>();
            stageData.Add(pageData);
        }
        SetPageDatas(ref enemyData, ref pageData, pageNum, waveNum);
    }

    public void SetPageDatas(ref EnemyData enemyData, ref List<List<List<EnemyData>>> pageData, int pageNum, int waveNum)
    {
        List<List<EnemyData>> waveData;

        // �������� �ִ� ���
        if (pageNum > 0 && pageNum <= pageData.Count())
            waveData = pageData[pageNum-1];
        else
        {
            waveData = new List<List<EnemyData>>();
            pageData.Add(waveData);
        }

        SetWaveDatas(ref enemyData, ref waveData, waveNum);
    }

    public void SetWaveDatas(ref EnemyData enemyData, ref List<List<EnemyData>> waveData, int waveNum)
    {
        List<EnemyData> enemyDatas;

        // ���̺갡 �ִ� ���
        if (waveNum > 0 && waveNum <= waveData.Count())
            enemyDatas = waveData[waveNum-1];
        else
        {
            enemyDatas = new List<EnemyData>();
            waveData.Add(enemyDatas);
        }

        enemyDatas.Add(enemyData);
    }

    /// <summary>
    /// ���� ��������, ������, ���̺��� ���� ������ ��´�.
    /// </summary>
    /// <param name="enemyNum">�� ��ȣ</param>
    /// <returns></returns>
    public void GetCurrEnemyDatas(ref List<EnemyData> enemyDatas)
    {
        var pageData = stageData[currStageNum-1];
        var waveData = pageData[currPageNum-1];
        
        enemyDatas = waveData[currWaveNum-1];
    }
}