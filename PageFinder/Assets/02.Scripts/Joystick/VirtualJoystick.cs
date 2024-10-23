using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IVirtualJoystick
{
    protected Image imageBackground;
    protected Image imageController;
    protected Vector2 touchPosition;
    protected float touchStartTime;
    protected float touchEndTime;
    protected float touchDuration;
    protected float shortTouchDuration;

    public virtual void SetImages()
    {
        imageBackground = DebugUtils.GetComponentWithErrorLogging<Image>(transform, "Image");
        imageController = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(0), "Image");
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        SetImages();
    }

    public virtual void OnPointerDown(PointerEventData eventData) 
    {
        ResetImageAndPostion();
    }

    public virtual void OnDrag(PointerEventData eventData) 
    {
        MoveImage(eventData, ref touchPosition);
    }

    public virtual void OnPointerUp(PointerEventData eventData) 
    {
        ResetImageAndPostion();
    }

    public virtual void MoveImage(PointerEventData eventData, ref Vector2 position)
    {
        // ���̽�ƽ�� ��ġ�� ��� �ֵ� ������ ���� �����ϱ� ����
        // touchPosition�� ��ġ ���� �̹����� ���� ��ġ�� ��������
        // �󸶳� ������ �ִ����� ���� �ٸ��� ���´�.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackground.rectTransform, eventData.position, eventData.pressEventCamera, out position))
        {
            // touchPosition�� ���� ����ȭ[0 ~ 1]
            // touchPosition�� �̹��� ũ��� ����
            position.x = (position.x / imageBackground.rectTransform.sizeDelta.x);
            position.y = (position.y / imageBackground.rectTransform.sizeDelta.y);

            // touchPosition ���� ����ȭ [-1 ~ 1]
            // ���� ���̽�ƽ ��� �̹��� ������ ��ġ�� ������ �Ǹ� -1 ~ 1���� ū ���� ���� �� �ִ�.
            // �� �� normalized�� �̿��� -1 ~ 1 ������ ������ ����ȭ
            position = (position.magnitude > 1) ? position.normalized : position;

            // ���� ���̽�ƽ ��Ʈ�ѷ� �̹��� �̵� 
            imageController.rectTransform.anchoredPosition = new Vector2(
                position.x * imageBackground.rectTransform.sizeDelta.x / 2,
                position.y * imageBackground.rectTransform.sizeDelta.y / 2);
        }
    }

    public virtual void ResetImageAndPostion()
    {
        // ��ġ ���� �� �̹����� ��ġ�� �߾����� �ٽ� �ű��.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // �ٸ� ������Ʈ���� �̵� �������� ����ϱ� ������ �̵� ���⵵ �ʱ�ȭ
        touchPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        return touchPosition.x;
    }

    public float Vertical()
    {
        return touchPosition.y;
    }
}
