using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AttackJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;
    private Vector3 attackDir;
    private float touchStartTime;
    private float touchEndTime;
    private float touchDuration;
    private float shortAttackTouchDuration;
    
    private PlayerAttackController playerAttackScr;
    private void Awake()
    {
        imageBackground = GetComponent<Image>();
        imageController = transform.GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        shortAttackTouchDuration = 0.1f;
        playerAttackScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerAttackController>();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartTime = Time.time;
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

            if(playerAttackScr == null)
            {
                Debug.LogError("playerAttackScr ��ü�� �����ϴ�.");
                return;
            }
            attackDir = new Vector3(touchPosition.x, 0, touchPosition.y);
            
            playerAttackScr.OnTargeting(attackDir, playerAttackScr.AttackRange);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // ��ġ ���� �� �̹����� ��ġ�� �߾����� �ٽ� �ű��.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // �ٸ� ������Ʈ���� �̵� �������� ����ϱ� ������ �̵� ���⵵ �ʱ�ȭ
        touchPosition = Vector2.zero;

        // ��ġ �ð� ����
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortAttackTouchDuration)
        {
            Debug.Log("ª�� ����");
            StartCoroutine(playerAttackScr.OnAttack(AttackType.SHORTATTCK));
        }
        else
        {
            Debug.Log("Ÿ�� ����");
            StartCoroutine(playerAttackScr.OnAttack(AttackType.LONGATTACK));
        }
    }
}
