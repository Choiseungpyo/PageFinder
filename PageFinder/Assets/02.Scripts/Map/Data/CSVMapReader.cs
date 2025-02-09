using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVMapReader : Singleton<CSVMapReader>
{
    public TextAsset textAssetData;
    public int columnCounts;

    private List<BattlePageData> battlePageDataList;
    private List<RiddlePageData> riddlePageDataList;
    //private List<ShopPageData> shopPageDataList;

    //private PageMap pageMap;

    private void Start()
    {
        //pageMap = DebugUtils.GetComponentWithErrorLogging<PageMap>(GameObject.Find("Maps"), "PageMap");

        //pageMap.Datas = shopPageDataList;
    }

    void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int tableSize = data.Length / columnCounts - 1;
        battlePageDataList = new List<BattlePageData>();

        //for (int i = 0; i < tableSize; i++)
        //{
        //    scriptDataList.Add(ScriptableObject.CreateInstance<ScriptData>());
        //    scriptDataList[i].scriptId = int.Parse(data[columnCounts * (i + 1)]);
        //    scriptDataList[i].scriptName = data[columnCounts * (i + 1) + 1];
        //    scriptDataList[i].scriptDesc = data[columnCounts * (i + 1) + 2];
        //    SetInkType(ref scriptDataList[i].inkType, data[columnCounts * (i + 1) + 3]);
        //    SetScitptIconAndBackground(
        //        ref scriptDataList[i].scriptIcon,
        //        ref scriptDataList[i].scriptBG,
        //        data[columnCounts * (i + 1) + 3],
        //        data[columnCounts * (i + 1) + 4],
        //        scriptDataList[i].scriptId
        //    );
        //    SetScriptType(ref scriptDataList[i].scriptType, data[columnCounts * (i + 1) + 4]);
        //    scriptDataList[i].price = int.Parse(data[columnCounts * (i + 1) + 5]);
        //    scriptDataList[i].percentages = new float[3];
        //    for (int j = 0; j < 3; j++)
        //    {
        //        scriptDataList[i].percentages[j] = float.Parse(data[columnCounts * (i + 1) + 6 + j]);
        //    }
        //    SetLevelData(ref scriptDataList[i].level, scriptDataList[i].percentages[0], scriptDataList[i].percentages[1]);
        //    allScriptIdList.Add(scriptDataList[i].scriptId);
        //    // ���� ���� ��ũ��Ʈ �������� ID�� 16, �� ������ �Ҳ��� ��� �÷��̾� ��ũ ������ �⺻ ��ũ��Ʈ�� �߰�.
        //    if (scriptDataList[i].scriptId == 16)
        //    {
        //        Debug.Log("��ũ��Ʈ id�� 16�� ������Ʈ ã��");
        //        playerBasicInkMagicScript = scriptDataList[i];
        //    }
        //}
    }
}
