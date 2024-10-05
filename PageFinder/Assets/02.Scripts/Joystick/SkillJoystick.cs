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

    public string currSkill;

    private PlayerSkillController playerSkillControllerScr;
    private SkillManager skillManager;
    private CoolTimeComponent coolTimeComponent;

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
        shortSkillTouchDuration = 0.1f;
        playerSkillControllerScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerSkillController>();
        skillManager = SkillManager.Instance;
        if (coolTimeComponent)
        {
            coolTimeComponent.CurrSkillCoolTime = skillManager.GetSkillData(currSkill).skillCoolTime;
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
        if (skillManager.GetSkillData(currSkill) == null 
            || skillManager.GetSkillData(currSkill).skillType == SkillTypes.STROKE || !coolTimeComponent.IsAbleSkill) 
            return;

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

            if (playerSkillControllerScr)
            {
                playerSkillControllerScr.OnTargeting(attackDir, skillManager.GetSkillData(currSkill).skillDist, skillManager.GetSkillData(currSkill).skillRange);
            }
                
        }
    }

    /// <summary>
    /// ���̽�ƽ ��ġ ����� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
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

        if (touchDuration <= shortSkillTouchDuration)
        {
            if (playerSkillControllerScr.InstantiateSkill(currSkill))
                StartCoroutine(coolTimeComponent.SkillCoolTime());
        }
        else
        {
            if (playerSkillControllerScr.InstantiateSkill(currSkill, attackDir))
                StartCoroutine(coolTimeComponent.SkillCoolTime());
        }
        playerSkillControllerScr.SetTargetObject(false);
        imageBackground.enabled = false;
        imageController.enabled = false;
    }

    /// <summary>
    /// ������ ��ų�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="skillName">������ ��ų �̸�</param>
    public void ChangeSkill(string skillName)
    {
        this.currSkill = skillName;
        if(coolTimeComponent)
            coolTimeComponent.CurrSkillCoolTime = skillManager.GetSkillData(currSkill).skillCoolTime;
    }

}
