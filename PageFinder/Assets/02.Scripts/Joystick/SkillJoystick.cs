using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class SkillJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;
    private Vector3 attackDir;
    private float touchStartTime;
    private float touchEndTime;
    private float touchDuration;
    private float shortSkillTouchDuration;

    private PlayerTarget playerTargetScr;
    private PlayerSkillController playerSkillControllerScr;
    private CoolTimeComponent coolTimeComponent;

    private void Awake()
    {
        imageBackground = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(0), "Image");
        imageController = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(1), "Image");
        coolTimeComponent = DebugUtils.GetComponentWithErrorLogging<CoolTimeComponent>(transform, "CoolTimeComponent");
    }

    private void Start()
    {
        imageBackground.enabled = false;
        imageController.enabled = false;
        shortSkillTouchDuration = 0.1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(playerObj, this.gameObject))
        {
            playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(playerObj, "PlayerSkillController");

        }

        playerTargetScr = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(playerObj, "PlayerTarget");

        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent))
        {
            if (playerSkillControllerScr == null)
            {
                Debug.Log("null!");
            }
            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
            {
                coolTimeComponent.CurrSkillCoolTime = playerSkillControllerScr.CurrSkillData.skillCoolTime;
            }
        }

    }

    /// <summary>
    /// ���̽�ƽ �Է� ���� �ÿ� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        attackDir = Vector3.zero;
        touchStartTime = Time.time;
    }

    /// <summary>
    /// ���̽�ƽ �巡�׽ÿ� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent, this.gameObject))
        {
            if (!coolTimeComponent.IsAbleSkill)
                return;
        }
        else
        {
            return;
        }

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


            attackDir = new Vector3(touchPosition.x, 0.1f, touchPosition.y);

            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
                {
                    if (playerSkillControllerScr.CurrSkillData.skillType == SkillTypes.FAN)
                    {
                        FanSkillData fanSkillData = playerSkillControllerScr.CurrSkillData as FanSkillData;
                        playerTargetScr.FanTargeting(attackDir, fanSkillData.skillRange, fanSkillData.fanDegree);
                    }
                }
            }

        }
    }

    /// <summary>
    /// ���̽�ƽ ��ġ ����� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent))
        {
            if (!coolTimeComponent.IsAbleSkill)
                return;
        }
        else
        {
            return;
        }

        // ��ġ ���� �� �̹����� ��ġ�� �߾����� �ٽ� �ű��.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // �ٸ� ������Ʈ���� �̵� �������� ����ϱ� ������ �̵� ���⵵ �ʱ�ȭ
        touchPosition = Vector2.zero;

        // ��ġ �ð� ����
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject))
        {
            playerTargetScr.OffAllTargetObjects();       
        }

        if(!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
        {
            if (touchDuration <= shortSkillTouchDuration)
            {
                if (playerSkillControllerScr.InstantiateSkill())
                    coolTimeComponent.StartCoolDown();
            }
            else
            {
                if (playerSkillControllerScr.InstantiateSkill(attackDir))
                    coolTimeComponent.StartCoolDown();
            }
        }
        
        imageBackground.enabled = false;
        imageController.enabled = false;
    }


}
