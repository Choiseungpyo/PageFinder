using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas shopUICanvas;
    private PlayerScriptController playerScriptControllerScr;
    private Dictionary<int, bool> stackedScriptDataInfo;
    [SerializeField]
    private GameObject[] scripts;
    [SerializeField]
    private List<ScriptData> scriptDatas;
    List<int> scriptIdList;
    private ScriptData selectData;

    [SerializeField]
    private TMP_Text coinText;

    [SerializeField]
    // ���ش� ����: player -> playerState
    //Player player;
    PlayerState playerState;
    [SerializeField]
    PageMap pageMap;

    public int coinToMinus = 0;

    private bool isAbled;
    public Dictionary<int, bool> StackedScriptDataInfo { get => stackedScriptDataInfo; set => stackedScriptDataInfo = value; }
    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public List<ScriptData> ScriptDatas { get => scriptDatas; set => scriptDatas = value; }

    private void Awake()
    {
        isAbled = false;
        stackedScriptDataInfo = new Dictionary<int, bool>();
        scriptIdList = new List<int>();
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
    }


    public void SetShopUICanvasState(bool value, bool changeScripts = true)
    {
        shopUICanvas.gameObject.SetActive(value);

        if (!value)
            return;

        scriptIdList.Clear();
        if (changeScripts)
        {
            SetScripts();
            SetCoinText();
        }

    }


    public int RandomChoice()
    {
        return Random.Range(0, CSVReader.Instance.AllScriptIdList.Count);
    }
    public void SetScripts()
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            ShopScript scriptScr = DebugUtils.GetComponentWithErrorLogging<ShopScript>(scripts[i], "Script");
            if (!DebugUtils.CheckIsNullWithErrorLogging<ShopScript>(scriptScr, this.gameObject))
            {
                StartCoroutine(MakeDinstinctScripts(scriptScr));
            }
        }
    }
    public IEnumerator MakeDinstinctScripts(ShopScript scriptScr)
    {
        // ��ø�� �ȵɶ� ����
        while (true)
        {
            int index = RandomChoice();
            // ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���
            if (scriptIdList.Contains(ScriptDatas[index].scriptId))
            {
                if (scriptIdList.Count == ScriptDatas.Count)
                {
                    yield break;
                }

                yield return null;
            }
            else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(ScriptDatas[index].scriptId) != null)
            {
                yield return null;
            }
            /*// �ش� ��ũ��Ʈ�� �÷��̾����� ���� ���
            else if (playerScriptControllerScr.CheckScriptDataAndReturnIndex(scriptId) != null)
            {
                ScriptData playerScript = playerScriptControllerScr.CheckScriptDataAndReturnIndex(scriptId);
                if(playerScript.level == -1 || playerScript.level >= 2)
                {
                    yield return null;
                }
                else
                {
                    scriptIdList.Add(scriptId);
                    playerScript.level += 1;
                    scriptScr.ScriptData = playerScriptControllerScr.CheckScriptDataAndReturnIndex(scriptId);
                }

            }*/
            // �ش� ��ũ��Ʈ�� �÷��̾����� ����, ��ũ��Ʈ 3���� �߿� �Ѱ����� ���ԵǾ� ���� ���� ���
            else
            {
                scriptIdList.Add(ScriptDatas[index].scriptId);
                scriptScr.ScriptData = ScriptDatas[index];
                yield break;
            }
        }

    }

    public void SendPlayerToScriptData()
    {
        playerScriptControllerScr.ScriptData = selectData;
        playerState.Coin -= selectData.price;
        pageMap.SetPageClearData();

    }

    private void SetCoinText()
    {
        coinText.text = playerState.Coin.ToString();
    }
}
