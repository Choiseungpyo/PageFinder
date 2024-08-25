using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public enum Category
    {
        ITEM,
        EQUIPMENT
    }

    public Canvas ShopCanvas;
    public GameObject[] Product = new GameObject[4];
    public GameObject ClickedProduct;
    public Image[] ProductTypeBtn = new Image[2];

    ProductData.Type type=  ProductData.Type.ITEM;
    string[] currentItems = new string[4];
    string[] currentEquipments = new string[4];

    ProductData productData;

    private void Start()
    {
        //stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();

        productData = ProductData.Instance;
        Init();
    }

    public void ClickItem()
    {
        type = ProductData.Type.ITEM;
        SetProducts();
        ChangeProductTypeBtnColor(type);
    }
    public void ClickEquipment()
    {
        type = ProductData.Type.EQUIPMENT;
        SetProducts();
        ChangeProductTypeBtnColor(type);
    }

    public void ClickProduct()
    {
        string name = EventSystem.current.currentSelectedGameObject.transform.GetChild(2).GetComponent<TMP_Text>().text;
        int productIndex = productData.FindProductIndex(type, name);

        SetClickedProduct(productIndex);
    }

    public void ResetShop()
    {
        // �� ��ǰ���� ���
        SetProducts();
    }

    public void Exit()
    {
        ShopCanvas.gameObject.SetActive(false);
    }

    public void Activate()
    {
        ShopCanvas.gameObject.SetActive(true);
        type = ProductData.Type.ITEM;
        SetProducts();
        ChangeProductTypeBtnColor(type);
    }

    public void Buy()
    {

    }

    void ChangeProductTypeBtnColor(ProductData.Type type)
    {
        int useBtnIndex = -1;
        int unusedBtnIndex = -1;

        if (type == ProductData.Type.ITEM)
        {
            useBtnIndex = 0;
            unusedBtnIndex = 1;
        }
        else if (type == ProductData.Type.EQUIPMENT)
        {
            useBtnIndex = 1;
            unusedBtnIndex = 0;
        }
        else
            Debug.LogWarning(type);

        // ���õ� ��ǰ Ÿ�� ����
        ProductTypeBtn[useBtnIndex].color = Color.gray;

        // ���õ��� ���� ��ǰ Ÿ�� ����
        ProductTypeBtn[unusedBtnIndex].color = Color.white;

        Debug.Log("Change");
    }

    void Init()
    {
        type = ProductData.Type.ITEM;
        productData.InitProducts();
        InitLongDescription();
        SetProducts();
    }

    void SetProducts()
    {

        for (int i = 0; i < Product.Length; i++)
        {
            int currentStage = 0;
            // ���� ���������� ���� ��ǰ���� �����Ѵ�.
            Product[i].transform.GetChild(1).GetComponent<Image>().sprite = productData.GetProduct(type, currentStage, i).sprite;
            Product[i].transform.GetChild(2).GetComponent<TMP_Text>().text = productData.GetProduct(type, currentStage, i).name;
            Product[i].transform.GetChild(3).GetComponent<TMP_Text>().text = productData.GetProduct(type, currentStage, i).shortDescription;
            Product[i].transform.GetChild(4).GetComponent<TMP_Text>().text = productData.GetProduct(type, currentStage, i).price + "G";

            if (type == ProductData.Type.ITEM)
                currentItems[i] = productData.GetProduct(type, currentStage, i).name;
            else if (type == ProductData.Type.EQUIPMENT)
                currentEquipments[i] = productData.GetProduct(type, currentStage, i).name;
            else
                Debug.LogWarning(type);
        }
    }

    void SetClickedProduct(int productIndex)
    {

        if (ClickedProduct.transform.GetChild(5).gameObject.activeSelf) // ó�� ��ǰ�� Ŭ���� ���
        {
            for (int i = 0; i < 5; i++)
                ClickedProduct.transform.GetChild(i).gameObject.SetActive(true);

            ClickedProduct.transform.GetChild(5).gameObject.SetActive(false);
        }
        int currentStage = 0;

        ClickedProduct.transform.GetChild(1).GetComponent<Image>().sprite = productData.GetProduct(type, currentStage, productIndex).sprite;
        ClickedProduct.transform.GetChild(2).GetComponent<TMP_Text>().text = productData.GetProduct(type, currentStage, productIndex).name;
        ClickedProduct.transform.GetChild(3).GetComponent<TMP_Text>().text = productData.GetProduct(type, currentStage, productIndex).longDescription;
        ClickedProduct.transform.GetChild(4).GetComponent<TMP_Text>().text = productData.GetProduct(type, currentStage, productIndex).price + "G";
    }

    void InitLongDescription()
    {
        for(int i=0; i<5; i++)
            ClickedProduct.transform.GetChild(i).gameObject.SetActive(false);

        // ���� �������� �������ּ��� ��� �ؽ�Ʈ�� ���̰� �ϱ�
        ClickedProduct.transform.GetChild(5).gameObject.SetActive(true);
    }

    /// <summary>
    /// ������ ���� �ִ��� Ȯ���Ѵ�.
    /// </summary>
    /// <returns></returns>
    bool CheckIfPlayerHaveMoneyToPay()
    {
        // �÷��̾��� �� ���� ��������
        return true;
    }
}
