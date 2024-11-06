using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RiddleUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas RiddleUICanavs;

    [SerializeField]
    private Image bookImg;
    [SerializeField]
    private Sprite[] bookSprites;

    [SerializeField]
    private TMP_Text problemTitleTxt;
    [SerializeField]
    private TMP_Text problemContentTxt;
    [SerializeField]
    private GameObject answerSet;

    private int currPageNum = 1;
    private int lastPageNum = 4;

    private int selectedContentNum = -1;

    PageMap pageMap;

    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();
    }

    public void SetRiddleUICanvasState(bool value)
    {
        RiddleUICanavs.gameObject.SetActive(value);

        if (!value)
            return;

        selectedContentNum = -1;
        SetBookImg();
        SetAnswerSetState(false);
    }

    private void SetBookImg()
    {
        if(currPageNum == lastPageNum)
        {
            if (selectedContentNum == 0 || selectedContentNum == 1)
                bookImg.sprite = bookSprites[3];
            else
                bookImg.sprite = bookSprites[4];
        }
        else
            bookImg.sprite = bookSprites[currPageNum - 1];
    }


    private void SetAnswerSetState(bool value)
    {
        answerSet.SetActive(value);
    }

    public void MoveNextPage()
    {
        Debug.Log("���� ������ : "+ currPageNum);
        // 1,2 ������
        if (currPageNum < lastPageNum - 1)
        {
            currPageNum++;
            SetBookImg();
            Debug.Log("1,2������");
            if(currPageNum == lastPageNum - 1)
                SetAnswerSetState(true);
        }
        // 3�������� ��
        else if(currPageNum == lastPageNum -1)
        {
            Debug.Log("3������");
            if (selectedContentNum == -1)
                return;

            currPageNum++;
            SetBookImg();
            SetAnswerSetState(false);
            Debug.Log("3->4 �������� �̵��ϵ��� ��ư ����");
        }
        // ������ ������
        else
        {
            Debug.Log("������ ���������� ���� �Ϸ�");

            int index = 0;
            Page pageToMove = pageMap.GetPageData(pageMap.CurrStageNum, pageMap.CurrPageNum-1);
            Debug.Log($"���� �������� : {pageToMove.PageDataName}");

            if (pageMap.CurrPageNum == 7)
                index = 0;
            else
                index = 1;

            switch (selectedContentNum)
            {
                // ���� ���� �̵� �ӵ� ����
                case 0:
                    pageMap.riddlePage1[index].target_moveSpeed = 1.5f;
                    EnemyManager.Instance.SetEnemyAboutCurrPageMap(pageMap.CurrStageNum, pageToMove);
                    UIManager.Instance.SetUIActiveState("RiddlePlay");
                    break;

                // ���� ���� Hp ����
                case 1:
                    pageMap.riddlePage1[index].target_hp = 100;
                    EnemyManager.Instance.SetEnemyAboutCurrPageMap(pageMap.CurrStageNum, pageToMove);
                    UIManager.Instance.SetUIActiveState("RiddlePlay");
                    break;

                // ����
                case 2:
                    pageMap.SetPageClearData(false);
                    break;

                default:
                    Debug.LogWarning(selectedContentNum);
                    break;
            }
        }
    }

    public void ClickAnswer()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        string cilcekdAnswerName = EventSystem.current.currentSelectedGameObject.name;

        for(int i=0; i< answerSet.transform.childCount; i++)
        {
            if(cilcekdAnswerName.Contains((i+1).ToString()))
            {
                selectedContentNum = i;
                break;
            }
        }
    }

}
