using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PaletteJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;

    //private PaletteUIManager paletteUIManager;
    private void Awake()
    {
        imageBackground = GetComponent<Image>();
        imageController = transform.GetChild(2).GetComponent<Image>();
       // paletteUIManager = GameObject.Find("UIManager").GetComponent<PaletteUIManager>();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // �̹��� ũ�� ����
        imageBackground.transform.localScale = new Vector3(4f, 4f, 1);
        imageController.transform.localScale = new Vector3(0.3f, 0.3f, 1);

        Debug.Log("PointerDown");
        //paletteUIManager.ChangePaletteObjectsActiveState(true);
    }


    public void OnDrag(PointerEventData eventData)
    {
        touchPosition = Vector2.zero;

        // ���̽�ƽ�� ��ġ�� ��� �ֵ� ������ ���� �����ϱ� ����
        // touchPosition�� ��ġ ���� �̹����� ���� ��ġ�� ��������
        // �󸶳� ������ �ִ����� ���� �ٸ��� ���´�.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackground.rectTransform, eventData.position, eventData.pressEventCamera, out touchPosition))
        {
            // touchPosition�� ���� ����ȭ[0 ~ 1]
            // touchPosition�� �̹��� ũ��� ����
            touchPosition.x = (touchPosition.x / imageBackground.rectTransform.sizeDelta.x);
            touchPosition.y = (touchPosition.y / imageBackground.rectTransform.sizeDelta.y);

            // touchPosition ���� ����ȭ [-1 ~ 1]
            // ���� ���̽�ƽ ��� �̹��� ������ ��ġ�� ������ �Ǹ� -1 ~ 1���� ū ���� ���� �� �ִ�.
            // �� �� normalized�� �̿��� -1 ~ 1 ������ ������ ����ȭ
            touchPosition = (touchPosition.magnitude > 1) ? touchPosition.normalized : touchPosition;

            // ���� ���̽�ƽ ��Ʈ�ѷ� �̹��� �̵� 
            imageController.rectTransform.anchoredPosition = new Vector2(
                touchPosition.x * imageBackground.rectTransform.sizeDelta.x / 2,
                touchPosition.y * imageBackground.rectTransform.sizeDelta.y / 2);

            //paletteUIManager.ChangePaletteObjectsColorTransparency(VectorToRadian(touchPosition));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        double rot = VectorToRadian(touchPosition);

        // ���̽�ƽ�� ���� ��ġ�� ����� ���� ���� ����
        //paletteUIManager.ChangeCurrentColor(rot);

        // ���� �ʱ�ȭ
        //paletteUIManager.ChangePaletteObjectsColorTransparency();

        // ��ġ ���� �� �̹����� ��ġ�� �߾����� �ٽ� �ű��.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // �ٸ� ������Ʈ���� �̵� �������� ����ϱ� ������ �̵� ���⵵ �ʱ�ȭ
        touchPosition = Vector2.zero;

        // �̹��� ũ�� ����
        imageBackground.transform.localScale = new Vector3(1.4f, 1.4f, 1);
        imageController.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        //paletteUIManager.ChangePaletteObjectsActiveState(false);
    }

    /// <summary>
    /// ���� (0,1)�� �������� �Է��� ���� ������ ���� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    double VectorToRadian(Vector2 to)
    {
        return Quaternion.FromToRotation(to, Vector3.up).eulerAngles.z;
    }
}
