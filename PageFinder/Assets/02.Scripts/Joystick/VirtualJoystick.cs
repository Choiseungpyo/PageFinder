using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface VirtualJoystick : IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // ��ġ ���۽� 1ȸ
    public new void OnPointerDown(PointerEventData eventData);

    // ��ġ ������ �� �� ������
    public new void OnDrag(PointerEventData eventData);

    // ��ġ ���� �� 1ȸ
    public new void OnPointerUp(PointerEventData eventData);
}
