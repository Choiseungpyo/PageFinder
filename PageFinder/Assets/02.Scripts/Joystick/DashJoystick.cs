using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DashJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;
    private Vector3 dashDir;
    private float touchStartTime;
    private float touchEndTime;
    private float touchDuration;
    private float shortSkillTouchDuration;

    private PlayerTarget playerTargetScr;
    private PlayerController playerControllerScr;
    private CoolTimeComponent coolTimeComponent;
    private bool isDraged;
    private void Awake()
    {
        imageBackground = transform.GetChild(0).GetComponent<Image>();
        imageController = transform.GetChild(1).GetComponent<Image>();
        coolTimeComponent = GetComponent<CoolTimeComponent>();
    }

    private void Start()
    {
        imageBackground.enabled = false;
        imageController.enabled = false;
        isDraged = false;
        shortSkillTouchDuration = 0.1f;
        playerControllerScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerController>();
        if (playerControllerScr)
        {
            coolTimeComponent.CurrSkillCoolTime = playerControllerScr.DashCooltime;
            Debug.Log(playerControllerScr.DashCooltime);
        }
        playerTargetScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerTarget>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill) return;
        dashDir = Vector3.zero;
        touchStartTime = Time.time;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill)
            return;
        isDraged = true;
        imageBackground.enabled = true;
        imageController.enabled = true;
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


            dashDir = new Vector3(touchPosition.x, 0.1f, touchPosition.y);

            if (playerTargetScr)
            {
                playerTargetScr.FixedLineTargeting(dashDir, playerControllerScr.DashPower, playerControllerScr.DashWidth);
            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill) return;
        // ��ġ ���� �� �̹����� ��ġ�� �߾����� �ٽ� �ű��.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // �ٸ� ������Ʈ���� �̵� �������� ����ϱ� ������ �̵� ���⵵ �ʱ�ȭ
        touchPosition = Vector2.zero;

        // ��ġ �ð� ����
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortSkillTouchDuration || isDraged == false)
        {
            playerControllerScr.Dash();
            isDraged = false;
        }
        else
        {
            playerControllerScr.Dash(new Vector3(dashDir.x, 0.0f, dashDir.z));
            isDraged = false;
        }
        if (coolTimeComponent)
        {
            StartCoroutine(coolTimeComponent.SkillCoolTime());
            Debug.Log("��ų ��Ÿ��");
        }

        playerTargetScr.OffAllTargetObjects();
        imageBackground.enabled = false;
        imageController.enabled = false;
    }
}
