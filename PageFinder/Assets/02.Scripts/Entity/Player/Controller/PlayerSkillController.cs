using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : Player
{

    private string currSkillName;
    private SkillManager skillManager;
    private GameObject currSkillObject;
    private SkillData currSkillData;
    public INKMARK skillInkMark;
    // ��ų ��ȯ ����
    private Vector3 spawnVector;
    // ��ų ���������
    private bool isUsingSkill;
    // Ÿ���� ������
    private bool isOnTargeting;

    // ������ �� ��ü
    Collider attackEnemy;

    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }
    public bool IsOnTargeting { get => isOnTargeting; set => isOnTargeting = value; }
    public string CurrSkillName { get => currSkillName; set => currSkillName = value; }
    public SkillData CurrSkillData { get => currSkillData; set => currSkillData = value; }
    public INKMARK SkillInkMark { get => skillInkMark; set => skillInkMark = value; }

    public override void Awake()
    {
        base.Awake();
        skillInkMark = INKMARK.BLUE;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        isUsingSkill = false; 
        skillManager = SkillManager.Instance;
        DebugUtils.CheckIsNullWithErrorLogging<SkillManager>(skillManager, this.gameObject);
        currSkillName = "SkillBulletFan";
        ChangeSkill(currSkillName);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsingSkill)
        {
            CheckAnimProgress(currSkillData.skillState, currSkillData.skillAnimEndTime, ref isUsingSkill);
        }
    }

    /// <summary>
    /// ���� ����� ������ ��ų�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <return>��ų ��ȯ ���� ����</return>
    public bool InstantiateSkill()
    {
        if (!isUsingSkill)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                { 
                    switch (currSkillData.skillType)
                    {
                        case SkillTypes.FAN:
                            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, currSkillData.skillDist, 1 << 6);
                            if (!DebugUtils.CheckIsNullWithErrorLogging(attackEnemy, "������ �� ��ü�� �����ϴ�."))
                            {
                                GameObject instantiatedSkill = Instantiate(currSkillObject, tr.position, Quaternion.identity);
                                if (!DebugUtils.CheckIsNullWithErrorLogging(instantiatedSkill, this.gameObject))
                                {
                                    anim.SetTrigger("TurningSkill");
                                    spawnVector = attackEnemy.transform.position - tr.position;
                                    TurnToDirection(spawnVector);
                                    Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                    if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                    {
                                        skill.SkillInkMark = this.skillInkMark;
                                        skill.ActiveSkill(spawnVector.normalized);
                                        isUsingSkill = true;
                                        return true;
                                    }
                                }
                            }
                            break;
                        default:
                            spawnVector = new Vector3(tr.position.x, tr.position.y + 0.1f, tr.position.z);
                            break;
                    }
                }
             }
        }
        return false;
    }

    // ������ ��ġ�� ��ų ��ȯ�ϴ� �Լ�
    public bool InstantiateSkill(Vector3 pos)
    {
        if (!isUsingSkill)
        {
            //rangedEntity.DisableCircleRenderer();
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                {
                    GameObject instantiatedSkill = Instantiate(currSkillObject, tr.position, Quaternion.identity);
                    if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedSkill, this.gameObject))
                    {
                        switch (currSkillData.skillType)
                        {
                            case SkillTypes.FAN:
                                TurnToDirection(pos);
                                anim.SetTrigger("TurningSkill");
                                Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                {
                                    skill.SkillInkMark = this.skillInkMark;
                                    skill.ActiveSkill(pos.normalized);
                                    isUsingSkill = true;
                                    return true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// ������ ��ų�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="skillName">������ ��ų �̸�</param>
    public bool ChangeSkill(string skillName)
    {
        if(!DebugUtils.CheckIsNullWithErrorLogging<SkillManager>(skillManager, this.gameObject))
        {
            this.currSkillObject = skillManager.GetSkillPrefab(skillName);
            if(DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                return false;
            }
            this.currSkillData = skillManager.GetSkillData(skillName);
            if (DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
            {
                return false;
            }
            this.currSkillData.skillInkMark = this.skillInkMark;
        }
        return true;
    }
}
