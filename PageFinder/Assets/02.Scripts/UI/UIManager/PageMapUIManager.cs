using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageMapUIManager : MonoBehaviour
{
    public enum STATE
    {
        MOVE,
        READ
    }

    STATE state = STATE.MOVE;

    [SerializeField]
    private Canvas PageMapUICanvas;

    // PageMap
    [SerializeField]
    private Sprite[] pagesMoveType_Spr; // 0:clear    1:�̵��Ұ�  3:�̵����� 4: ������ �β��� ����
    [SerializeField]
    private RectTransform[] stagePage;
    [SerializeField]
    private Image playerIcon_Img;
    [SerializeField]
    private GameObject LinkedInk_Prefab;
    [SerializeField]
    private Transform linkedInks;
    string[] whitePageNames = { "", "", "" };

    // ������ ����â
    [SerializeField]
    private Sprite[] pageIcons_Spr;
    [SerializeField]
    private GameObject iconDiscriptionObj;
    [SerializeField]
    private Image iconImg;
    [SerializeField]
    private TMP_Text iconTitleTxt;
    [SerializeField]
    private TMP_Text iconContentTxt;


    // �ܰ���
    [SerializeField]
    private RectTransform clckedPageOutLine;

    // ����
    [SerializeField]
    private TMP_Text coinTxt;

    // NextPage
    [SerializeField]
    private Button nextPageBtn;
    [SerializeField]
    private Sprite[] nextPageBtn_Sprites;

    GameObject currSelectedObj = null;

    int[] clearPageNum;

    [SerializeField]
    PageMap pageMap;
    // ���ش� ����
    [SerializeField]
    PlayerState playerState;


    public void SetPageMapUICanvasState(bool value, string prvUIName)
    {
        PageMapUICanvas.gameObject.SetActive(value);

        if (!value)
            return;

        /* [NextPage]
            * - �̵��ϱ� ���� ����ȭ�ǵ��� ���� ok
            * 
            * [Discription]
            * - �÷��̾� hp, Coin �� �ֽ�ȭ
            * - Discription before�� ���� ok
            * 
            * [������ ��]
            * - �̵��� ����, �̵������� ����, �̵��Ұ��� ���� ���� ok
            * - �÷��̾� ��ġ �ֽ�ȭ ok
            */
        currSelectedObj = null;
        clckedPageOutLine.gameObject.SetActive(false);
        setState(prvUIName);
        SetNextPageBtn(state == STATE.READ ? true : false);
        MovePagePosOfplayerIcon_Img();
        SetMoveTypesOfCurrPage();
        LinkClearPage();
        SetIconDiscription();
        SetCoinTxt();
    }

    private void setState(string prvUIName)
    {
        switch(prvUIName)
        {
            case "Setting":
                state = STATE.READ;
                break;

            default:
                state = STATE.MOVE;
                break;
        }
    }

    private void SetNextPageBtn(bool isActive)
    {
        if(pageMap.CurrPageNum == -1)
        {
            nextPageBtn.interactable = true;
            nextPageBtn.image.sprite = nextPageBtn_Sprites[0];
            clckedPageOutLine.gameObject.SetActive(true);
            clckedPageOutLine.position = GameObject.Find("0_0").GetComponent<RectTransform>().position;
            currSelectedObj = GameObject.Find("0_0");
            return;
        }

        if (state == STATE.MOVE)
            nextPageBtn.image.sprite = nextPageBtn_Sprites[0];
        else if (state == STATE.READ)
            nextPageBtn.image.sprite = nextPageBtn_Sprites[1];

        nextPageBtn.interactable = isActive;
    }

    /// <summary>
    /// ���� �������� �̵��Ѵ�.
    /// </summary>
    public void MoveNextPage()
    {
        if (state == STATE.READ)
        {
            UIManager.Instance.SetUIActiveState("Battle");
            return;
        }

        string[] moveData = currSelectedObj.name.Split('_');
        Page pageToMove = pageMap.GetPageData(int.Parse(moveData[0]),  int.Parse(moveData[1]));

        switch(pageToMove.pageType)
        {
            case Page.PageType.TRANSACTION:
                break;

            case Page.PageType.RIDDLE:
                break;

            default:
                EnemyManager.Instance.SetEnemyAboutCurrPageMap(int.Parse(moveData[0]), pageToMove);
                break;
        }

        playerState.transform.position = pageToMove.GetSpawnPos();
        pageMap.CurrPageNum = int.Parse(moveData[1]);
        UIManager.Instance.SetUIActiveState(pageToMove.getPageTypeString());
    }

    /// <summary>
    /// ���������� �̵� Ÿ���� �����Ѵ�.
    /// </summary>
    private void SetMoveTypesOfCurrPage()
    {
        int currStageNum = pageMap.CurrStageNum; // ���� : [1~n - 1~n

        int[] whitePageNums = { -2, -2, -2 };
        int maxWhitePageNum = -1;
        SetwhitePageNames();

        for (int i = 0; i < whitePageNames.Length; i++)
        {
            if (whitePageNames[i].Equals(""))
                continue;
            whitePageNums[i] = int.Parse(whitePageNames[i].Split('_')[1]);
            if (whitePageNums[i] > maxWhitePageNum)
                maxWhitePageNum = whitePageNums[i];
        }

        int bluePageMaxNum = pageMap.CheckIfItIsSameColPageAbout1Stage(maxWhitePageNum);
    
        // �̵� ���ο� ���� ������ ��������Ʈ ��ü
        for (int pageNum = 0; pageNum < stagePage[currStageNum].transform.childCount; pageNum++)
        {
            stagePage[currStageNum].GetChild(pageNum).transform.localScale = Vector3.one;

            // ���� �������� �����Ͽ� �̵������� �������� ���(�Ͼ��)
            if (pageNum == whitePageNums[0] || pageNum == whitePageNums[1] || pageNum == whitePageNums[2])
                stagePage[currStageNum].GetChild(pageNum).GetComponent<Image>().sprite = pagesMoveType_Spr[0];
            // ������ �� �� ���� �������� ���(�Ķ���)
            else if (pageNum <= bluePageMaxNum)
                stagePage[currStageNum].GetChild(pageNum).GetComponent<Image>().sprite = pagesMoveType_Spr[1];
            // ���� ���� �̷��� �̵����������� ���� �̵� �Ұ����� �������� ���(������)
            else
                stagePage[currStageNum].GetChild(pageNum).GetComponent<Image>().sprite = pagesMoveType_Spr[2];
        }

        // �÷��̾� ���� ó�� ������ ��� 0-0 ĭ �Ͼ������ ����
        if (whitePageNums[0] == -1)
            stagePage[currStageNum].GetChild(0).GetComponent<Image>().sprite = pagesMoveType_Spr[0];

    }

    private void LinkClearPage()
    {
        int currStageNum = pageMap.CurrStageNum; // ���� : [1~n]

        clearPageNum = pageMap.GetClearPagesAboutStage1();

        for (int i=0; i<clearPageNum.Length; i++)
        {
            if (i == clearPageNum.Length-1)
                break;

            if (clearPageNum[i] == -1 || clearPageNum[i+1] == -1)
                break;

            Vector3 pos = (stagePage[currStageNum].GetChild(clearPageNum[i]).transform.position + stagePage[currStageNum].GetChild(clearPageNum[i + 1]).transform.position) / 2;
            Vector3 dir = (stagePage[currStageNum].GetChild(clearPageNum[i + 1]).transform.position - stagePage[currStageNum].GetChild(clearPageNum[i]).transform.position) / 100;
            Quaternion quaternion = Quaternion.Euler(0,0,-45); // �⺻ ������

            // �ϵ�
            if (dir.y > 0)
                quaternion = Quaternion.Euler(0, 0, 45);

            GameObject obj = Instantiate(LinkedInk_Prefab, pos, quaternion, linkedInks);
            obj.GetComponent<Image>().raycastTarget = false;
        }
    }

    /// <summary>
    /// Ŭ���� ���������� �̵� Ÿ���� �����Ѵ�.
    /// </summary>
    public void SetMoveTypesOfClickedPage()
    {
        currSelectedObj = EventSystem.current.currentSelectedGameObject;

        if (!clckedPageOutLine.gameObject.activeSelf)
            clckedPageOutLine.gameObject.SetActive(true);

        clckedPageOutLine.position = currSelectedObj.GetComponent<RectTransform>().position;
        SetIconDiscription();

        if (state == STATE.READ)
            return;

       
        bool canMove = false;


        if (pageMap.CurrPageNum == -1 && currSelectedObj.name.Equals("0_0"))
        {
            canMove = true;
        }
        else
        {
            for (int i = 1; i < whitePageNames.Length; i++)
            {
                // ���� ������ ���� �ϵ�, ������ �������� Ŭ���� ���
                if (currSelectedObj.name.Equals(whitePageNames[i]))
                {
                    canMove = true;
                    break;
                }
            }
        }

        if (!canMove)
        {
            SetNextPageBtn(false);
            return;
        }
        

        // ���� �������� �������� �ϵ�, �����ʿ� �÷��̾� �������� ��ġ�� ��쿡�� ���� �������� �̵��� �� �ִ�. 
        // �̵��ϱ� ��ư ����ȭ ����
        SetNextPageBtn(true);
    }

    private void MovePagePosOfplayerIcon_Img()
    {
        int currStageNum = pageMap.CurrStageNum; // ���� : [1~n]
        int currpageNum = pageMap.CurrPageNum;

        // ���������� �� ó��
        if (currpageNum == -1)
            playerIcon_Img.enabled = true;
        else
        {
            playerIcon_Img.enabled = true;
            playerIcon_Img.transform.position = stagePage[currStageNum].GetChild(currpageNum).transform.position;
        }
    }

    private void SetwhitePageNames()
    {
        int currStageNum = pageMap.CurrStageNum;  // ���� : [1~n]
        int currpageNum = pageMap.CurrPageNum;

        for (int i = 0; i < whitePageNames.Length; i++)
            whitePageNames[i] = "";

        // �� ó�� ����
        if (currpageNum == -1)
        {
            whitePageNames[0] = "0_-1";
            return;
        }

        whitePageNames[0] = currStageNum.ToString() + "_" + currpageNum.ToString();

        switch (currpageNum)
        {
            case 0:
                whitePageNames[1] = currStageNum + "_1";
                whitePageNames[2] = currStageNum + "_2";
                break;

            case 1:
                whitePageNames[1] = currStageNum + "_3";
                whitePageNames[2] = currStageNum + "_4";
                break;

            case 2:
                whitePageNames[1] = currStageNum + "_4";
                whitePageNames[2] = currStageNum + "_5";
                break;

            case 3:
                whitePageNames[1] = currStageNum + "_6";
                break;

            case 4:
                whitePageNames[1] = currStageNum + "_6";
                whitePageNames[2] = currStageNum + "_7";
                break;

            case 5:
                whitePageNames[1] = currStageNum + "_7";
                break;

            case 6:
                whitePageNames[1] = currStageNum + "_8";
                whitePageNames[2] = currStageNum + "_9";
                break;

            case 7:
                whitePageNames[1] = currStageNum + "_9";
                break;

            case 8:
                whitePageNames[1] = currStageNum + "_10";
                break;

            case 9:
                whitePageNames[1] = currStageNum + "_10";
                break;
        }


        //RectTransform currPageObj = stagePage[currStageNum].GetChild(currpageNum).GetComponent<RectTransform>();
        //RaycastHit hit;

        //// ���� �÷��̾ ��ġ���ִ� ������
        //whitePageNames[0] = currStageNum.ToString() + "_" + currpageNum.ToString();

        //// �ϵ��� üũ
        //if (Physics.Raycast(currPageObj.position, new Vector3(1, 1, 0), out hit, pageImgDist))
        //    whitePageNames[1] = hit.transform.name;

        //// ������ üũ
        //if (Physics.Raycast(currPageObj.position, new Vector3(1, -1, 0), out hit, pageImgDist))
        //    whitePageNames[2] = hit.transform.name;

        //Debug.Log(whitePageNames[1] + " " + whitePageNames[2]);
    }

    private void SetIconDiscription()
    {
        if (currSelectedObj == null)
        {
            iconDiscriptionObj.SetActive(false);
            return;
        }

        Sprite sprite = null;
        string iconName = "";
        string iconDiscription = "";

        string[] pageMapData = currSelectedObj.name.Split('_');
        int stageNum = int.Parse(pageMapData[0]);
        int pageNum = int.Parse(pageMapData[1]);
        Page page;

        iconDiscriptionObj.SetActive(true);
        page = pageMap.GetPageData(stageNum, pageNum);


        switch (page.pageType.ToString())
        {
            case "BATTLE":
                sprite = pageIcons_Spr[0];
                iconName = "����";
                iconDiscription = "������ �¸��ϸ� ���ڶ�\n��ȭ�ϴ� ��ũ��Ʈ��\nȹ���� �� �ֽ��ϴ�.";
                break;

            case "TRANSACTION":
                sprite = pageIcons_Spr[1];
                iconName = "�ŷ�";
                iconDiscription = "��带 �Ҹ��Ͽ� ���ڶ� ��ȭ�ϴ� ��ũ��Ʈ�� �����ϰų�, ���� ��ũ��Ʈ�� ���ο� ��ũ��Ʈ�� ��ȯ�� �� �ֽ��ϴ�.";
                break;

            case "RIDDLE":
                sprite = pageIcons_Spr[2];
                iconName = "��������";
                iconDiscription = "���ο� �̾߱⸦ �߰��ϰ�\n���ÿ� ���� �ٸ� �����\nȮ���մϴ�.";
                break;

            case "MIDDLEBOSS":
                sprite = pageIcons_Spr[3];
                iconName = "�߰�����";
                iconDiscription = "������ ������ ����� ��ٸ��� �ֽ��ϴ�. �� ���������� �¸��ϸ� �̹� é�͸� Ŭ������ �� �ֽ��ϴ�.";
                break;

            default:
                Debug.LogWarning(currSelectedObj.name);
                break;

        }

        iconImg.sprite = sprite;
        iconImg.SetNativeSize();
        iconTitleTxt.text = iconName;
        iconContentTxt.text = iconDiscription;
    }

    private void SetCoinTxt()
    {
        // ���ش� ���� : player -> playerState
        coinTxt.text = playerState.Coin.ToString();
    }
}
