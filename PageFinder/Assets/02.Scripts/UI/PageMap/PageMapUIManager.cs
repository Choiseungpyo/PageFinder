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
    [SerializeField]
    private Canvas PageMapUICanvas;

    // PageMap
    [SerializeField]
    private Sprite[] pagesMoveType_Spr; // 0:clear    1:�̵��Ұ�  3:�̵����� 4: ������ �β��� ����
    [SerializeField]
    private Transform[] stagePage;
    [SerializeField]
    private Image playerIcon_Img;
    [SerializeField]
    private GameObject LinkedInk_Prefab;
    [SerializeField]
    private Transform linkedInks;
    string[] whitePageNames = { "", "", "" };

    // Discription
    [SerializeField]
    private GameObject additionalDiscriptions;

    // Player Discription
    [SerializeField]
    private SliderBar playerHpBar;
    [SerializeField]
    private TMP_Text coinValue_Txt;


    // NextPage
    [SerializeField]
    private GameObject[] nextPageObjects;

    // Additional Discription Btn -2
    bool isClick = false;
    float clickTime = 0;

    // Additional Discription Btn -1
    [SerializeField]
    private GameObject additionalDiscritionsObj; 
    [SerializeField]
    private SVGImage additionalDiscritionIcon_SvgImg;
    [SerializeField]
    private TMP_Text additionalDiscritionIconName_Txt;
    [SerializeField]
    private TMP_Text additionalDiscritionIconDiscrition_Txt;
    [SerializeField]
    private Sprite[] additionalDiscritionIcons_Spr = new Sprite[4];
    string clickedPageIconName = "";


    GameObject currSelectedObj = null;

    int[] clearPageNum;

    PageMap pageMap;
    Player player;
    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();
        player = GameObject.FindWithTag("PLAYER").GetComponent<Player>();

    }

    private void Update()
    {
        //SetAddtionalDiscription2();
        SetAddtionalDiscriptionClickTime();
    }

    public void SetPageMapUICanvasState(bool value)
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
        SetNextPageBtnTransparentState(true);
        MovePagePosOfplayerIcon_Img();
        SetMoveTypesOfCurrPage();
        //additionalDiscriptions.SetActive(false);
        additionalDiscritionsObj.SetActive(false);
        SetPlayerDiscription();
        LinkClearPage();
    }

    /// <summary>
    /// ���� ������ �̵� ���� ����ȭ ������ �Ѵ�.
    /// </summary>
    public void SetNextPageBtnTransparentState(bool value)
    {
        Color currImgColor = nextPageObjects[0].GetComponent<Image>().color;

        if (value)
            currImgColor = new Color(currImgColor.r, currImgColor.g, currImgColor.b, 0.5f);
        else
            currImgColor = new Color(currImgColor.r, currImgColor.g, currImgColor.b, 1f);

        nextPageObjects[0].GetComponent<Image>().color = currImgColor;
        nextPageObjects[1].GetComponent<Button>().interactable = !value;
    }

    /// <summary>
    /// ���� �������� �̵��Ѵ�.
    /// </summary>
    public void MoveNextPage()
    {
        string[] moveData = currSelectedObj.name.Split('-');
        Page pageToMove = pageMap.GetPageData(int.Parse(moveData[0]),  int.Parse(moveData[1]) - 1);
        EnemyManager.Instance.SetEnemyAboutCurrPageMap(int.Parse(moveData[0]), pageToMove);
        player.transform.position = pageToMove.GetSpawnPos();

        pageMap.CurrPageNum = int.Parse(moveData[1]);

        SetPageMapUICanvasState(false);
    }

    /// <summary>
    /// ���������� �̵� Ÿ���� �����Ѵ�.
    /// </summary>
    private void SetMoveTypesOfCurrPage()
    {
        int currStageNum = pageMap.CurrStageNum - 1; // ���� : [1~n - 1~n
        int currPageNum = pageMap.CurrPageNum - 1;
        int[] whitePageNums = { -1, -1, -1 };
        int bluePageMaxNum = pageMap.CheckIfItIsSameColPageAbout1Stage(currPageNum);
        

        SetwhitePageNames();

        for (int i = 0; i < whitePageNames.Length; i++)
        {
            if (whitePageNames[i].Equals(""))
                continue;
            whitePageNums[i] = int.Parse(whitePageNames[i].Split('-')[1]) - 1;
        }

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
    }

    private void LinkClearPage()
    {
        int currStageNum = pageMap.CurrStageNum - 1; // ���� : [1~n]

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
        bool canMove = false;
        string[] tmpPageData;


        if (!pageMap.isClearPageAboutStage1(0) && currSelectedObj.name.Equals(whitePageNames[0]))
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
            return;

        // Ŭ���� �������� �̵������� �������� ���
        currSelectedObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        if (currSelectedObj.name.Equals(whitePageNames[1]))
        {
            tmpPageData = whitePageNames[2].Split('-');
            if (tmpPageData.Length == 2)
                stagePage[int.Parse(tmpPageData[0]) - 1].GetChild(int.Parse(tmpPageData[1]) - 1).transform.localScale = Vector3.one;
        }
        else
        {
            tmpPageData = whitePageNames[1].Split('-');
            if (tmpPageData.Length == 2)
                stagePage[int.Parse(tmpPageData[0]) - 1].GetChild(int.Parse(tmpPageData[1]) - 1).transform.localScale = Vector3.one;
        }

        // ���� �������� �������� �ϵ�, �����ʿ� �÷��̾� �������� ��ġ�� ��쿡�� ���� �������� �̵��� �� �ִ�. 
        // �̵��ϱ� ��ư ����ȭ ����
        SetNextPageBtnTransparentState(false);

    }

    private void MovePagePosOfplayerIcon_Img()
    {
        int currStageNum = pageMap.CurrStageNum - 1; // ���� : [1~n]
        int currpageNum = pageMap.CurrPageNum - 1;

        playerIcon_Img.transform.position = stagePage[currStageNum].GetChild(currpageNum).transform.position;
    }

    private void SetwhitePageNames()
    {
        int currStageNum = pageMap.CurrStageNum - 1;  // ���� : [1~n]
        int currpageNum = pageMap.CurrPageNum - 1;
        Transform currPageObj = stagePage[currStageNum].GetChild(currpageNum);
        RaycastHit hit;

        for (int i = 0; i < whitePageNames.Length; i++)
            whitePageNames[i] = "";

     
        // ���� �÷��̾ ��ġ���ִ� ������
        whitePageNames[0] = (currStageNum + 1).ToString() + "-" + (currpageNum + 1).ToString();
      
        // 1-1�� ������ ��
        if (!pageMap.isClearPageAboutStage1(0))
            return;

        // �ϵ��� üũ
        if (Physics.Raycast(currPageObj.position, new Vector3(1, 1, 0), out hit, 150))
            whitePageNames[1] = hit.transform.name;

        // ������ üũ
        if (Physics.Raycast(currPageObj.position, new Vector3(1, -1, 0), out hit, 150))
            whitePageNames[2] = hit.transform.name;
    }

    public void AdditionalDiscriptionBtnDown()
    {
        isClick = true;
        clickedPageIconName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<SVGImage>().sprite.name.Split("IconSprite")[0];
    }

    public void AdditionalDiscriptionBtnUp()
    {
        clickTime = 0;
        isClick = false;
        //additionalDiscriptions.SetActive(false);
    }

    private void SetAddtionalDiscriptionClickTime()
    {
        if (!isClick)
            return;

        clickTime += Time.deltaTime;
        if (clickTime > 1f)
        {
            isClick = false; // �Ʒ� �Լ� �ѹ��� ����
            additionalDiscritionsObj.SetActive(true);
            SetAddtionalDiscription1();
            Debug.Log("�� Ŭ��");
            clickTime = 0;
        }
    }


    private void SetAddtionalDiscription1()
    {
        Sprite sprite = null;
        string iconName = "";
        string iconDiscription = "";

        switch (clickedPageIconName)
        {
            case "Battle":
                sprite = additionalDiscritionIcons_Spr[0];
                iconName = "����";
                iconDiscription = "������ �¸��ϸ� ���ڶ� ��ȭ�ϴ� ��ũ��Ʈ�� ȹ���� �� �ֽ��ϴ�.";
                break;

            case "Transaction":
                sprite = additionalDiscritionIcons_Spr[1];
                iconName = "�ŷ�";
                iconDiscription = "��带 �Ҹ��Ͽ� ���ڶ� ��ȭ�ϴ� ��ũ��Ʈ�� �����ϰų�, ���� ��ũ��Ʈ�� ���ο� ��ũ��Ʈ�� ��ȯ�� �� �ֽ��ϴ�.";
                break;

            case "Riddle":
                sprite = additionalDiscritionIcons_Spr[2];
                iconName = "��������";
                iconDiscription = "�����ο� �̾߱⸦ �߰��ϰ� ���ÿ� ���� �ٸ� ����� Ȯ���մϴ�.";
                break;

            case "MiddleBoss":
                sprite = additionalDiscritionIcons_Spr[3];
                iconName = "�߰�����";
                iconDiscription = "������ ������ ����� ��ٸ��� �ֽ��ϴ�. �� ���������� �¸��ϸ� �̹� é�͸� Ŭ������ �� �ֽ��ϴ�.";
                break;

            default:
                Debug.LogWarning(clickedPageIconName);
                break;

        }
        additionalDiscritionIcon_SvgImg.sprite = sprite;
        additionalDiscritionIconName_Txt.text = iconName;
        additionalDiscritionIconDiscrition_Txt.text = iconDiscription;
    }

    public void CloseAdditionalDiscrition()
    {
        additionalDiscritionsObj.SetActive(false);
    }

    private void SetPlayerDiscription()
    {
        // Hp Bar
        playerHpBar.SetMaxValueUI(player.MAXHP);
        playerHpBar.SetCurrValueUI(player.HP);

        // Coin
        coinValue_Txt.text = "100";
    }

}
