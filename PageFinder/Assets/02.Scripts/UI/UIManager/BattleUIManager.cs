using Google.GData.AccessControl;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class BattleUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas BattleUICanvas;

    [SerializeField]
    private TMP_Text pageTypeTxt;

    [SerializeField]
    private GameObject goalContentImg;
    [SerializeField]
    private TMP_Text goalContentTxt;

    [SerializeField]
    private GameObject goalDetailContentImg;
    [SerializeField]
    private TMP_Text goalDetailContentTxt;

    [SerializeField]
    private GameObject goalFailImg;

    // Riddle Play

    [SerializeField]
    private GameObject timer;
    [SerializeField]
    private TMP_Text timer_Txt;
    float total_time;
    int timer_min;
    float timer_sec;

    bool canTimeCheck;

    PageMap pageMap;

    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();

        SetGoalContent(false);
        SetGoalDetailContent(false);
        goalFailImg.SetActive(false);
        InitTime();
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ʈ ������������ �ð��帣�� ����
        if (pageMap.CurrPageNum < 6 || pageMap.CurrPageNum > 7)
            return;
        
        SetTimer();
    }

    public void SetBattleUICanvasState(bool value, bool isSetting, bool isBattle = true)
    {
        BattleUICanvas.gameObject.SetActive(value);

        if (!value)
            return;

        // �������� �� ���� ���� ���� ȭ���� Battle�� ��ȯ�� ���� �̹� BattleUI�� ���� �����Ƿ� ���� �Լ� �������� �ʰ� ����
        if (isSetting)
            return;

        SetPageTypeTxt(isBattle ? "��Ʋ ������" : "�������� ������");
        StartCoroutine(SetGoalData());

        if (!isBattle)
            InitTime(true);
        else
            InitTime();
    }

    private void SetPageTypeTxt(string value)
    {
        pageTypeTxt.text = value;
    }

    IEnumerator SetGoalData()
    {
        SetGoalContent(true);
        SetGoalDetailContent(false);
        goalFailImg.SetActive(false);

        float currentTime = 0.0f;
        float goalContentFadeTime = 2f;

        while (currentTime / goalContentFadeTime < 1)
        {
            currentTime += Time.deltaTime;

            goalContentImg.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, currentTime / goalContentFadeTime));
            goalContentTxt.color = new Color(169/255.0f, 109/255.0f, 79/255.0f, Mathf.Lerp(1, 0, currentTime / goalContentFadeTime));

            yield return null;
        }

        //yield return new WaitForSeconds(2f);

        SetGoalContent(false);
        SetGoalDetailContent(true);
    }

    void SetGoalContent(bool value)
    {
        goalContentImg.SetActive(value);

        if (!value)
            return;
        goalContentImg.GetComponent<Image>().color = Color.white;
        goalContentTxt.color = new Color(169 / 255.0f, 109 / 255.0f, 79 / 255.0f, 1);

        //goalContentTxt.text = $"��θӸ� {EnemyManager.Instance.CurrTargetName}�� óġ�ϼ���"; // ex ) ��θӸ� ����縦 óġ�ϼ���
    }

    void SetGoalDetailContent(bool value)
    {
        goalDetailContentImg.SetActive(value);
        goalDetailContentTxt.fontStyle = FontStyles.Normal;
        if (!value)
            return;
        //goalDetailContentTxt.text = $"��θӸ� {EnemyManager.Instance.CurrTargetName}��\nóġ�ϱ�"; // ex ) ��θӸ� ����縦 óġ�ϱ�
    }


    public IEnumerator SetClearDataUI(bool value)
    {
        canTimeCheck = false;

        if (value)
            goalDetailContentTxt.fontStyle = FontStyles.Strikethrough;
        else
            goalFailImg.SetActive(true);


        yield return new WaitForSeconds(2.0f);

        if (!value)
        {
            goalFailImg.SetActive(false);
            UIManager.Instance.SetUIActiveState("PageMap");
        }
        else
            UIManager.Instance.SetUIActiveState("Reward");

    }

    /// <summary>
    /// ���ѽð��� ����Ѵ�. 
    /// </summary>
    void SetTimer()
    {
        if (!canTimeCheck)
            return;

        if (total_time < 0)
        {
            StartCoroutine(SetClearDataUI(false));
            canTimeCheck = false;
            return;
        }

        total_time -= Time.deltaTime;
        timer_min = (int)(total_time / 60f);
        timer_sec = (int)(total_time % 60f);
        timer_Txt.text = timer_min + ":" + timer_sec;
    }

    void InitTime(bool enable = false)
    {
        timer.SetActive(enable);
        canTimeCheck = enable;
        total_time = 30;
        timer_min = (int)(total_time / 60f);
        timer_sec = (int)(total_time % 60f);
    }

    public void ActivateSettingUI()
    {
        UIManager.Instance.SetUIActiveState("Setting");
    }
}
