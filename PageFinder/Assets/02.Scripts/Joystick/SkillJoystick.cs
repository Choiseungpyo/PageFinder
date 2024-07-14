using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public string skillName;
    private PlayerSkillController playerSkillControllerScr;
    private SkillManager<GameObject> skillObjectManager;
    private SkillManager<SkillData> skillDataManager;

    private void Awake()
    {
        imageBackground = transform.GetChild(0).GetComponent<Image>();
        imageController = transform.GetChild(1).GetComponent<Image>();
        skillObjectManager = SkillManager<GameObject>.Instance;
        skillDataManager = SkillManager<SkillData>.Instance;
    }

    private void Start()
    {
        imageBackground.enabled = false;
        imageController.enabled = false;
        shortSkillTouchDuration = 0.1f;
        playerSkillControllerScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerSkillController>();

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
        if (skillObjectManager[skillName] == null 
            || skillDataManager[skillName].skillType == SkillTypes.STROKE) 
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


            playerSkillControllerScr.OnTargeting(attackDir, skillDataManager[skillName].skillDist);
        }
    }

    /// <summary>
    /// ���̽�ƽ ��ġ ����� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // ��ġ ���� �� �̹����� ��ġ�� �߾����� �ٽ� �ű��.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // �ٸ� ������Ʈ���� �̵� �������� ����ϱ� ������ �̵� ���⵵ �ʱ�ȭ
        touchPosition = Vector2.zero;

        // ��ġ �ð� ����
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortSkillTouchDuration)
        {
            playerSkillControllerScr.InstantiateSkill(skillName);
        }
        else
        {
            playerSkillControllerScr.InstantiateSkill(skillName, attackDir);
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
        this.skillName = skillName;
    }
}
