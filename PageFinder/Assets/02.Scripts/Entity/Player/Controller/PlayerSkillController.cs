using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{

    private string currSkillName;
    private SkillManager skillManager;
    private GameObject currSkillObject;
    private SkillData currSkillData;
    private PlayerAttackController playerAttackControllerScr;

    // ��ų ��ȯ ����
    private Vector3 spawnVector;
    // ��ų ���������
    private bool isUsingSkill;
    // Ÿ���� ������
    private bool isOnTargeting;

    // ������ �� ��ü
    private Collider attackEnemy;

    private Player playerScr;
    private UtilsManager utilsManager;

    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }
    public bool IsOnTargeting { get => isOnTargeting; set => isOnTargeting = value; }
    public string CurrSkillName { get => currSkillName; set => currSkillName = value; }
    public SkillData CurrSkillData { get => currSkillData; set => currSkillData = value; }


    public void Awake()
    {
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        isUsingSkill = false;
    }
    // Start is called before the first frame update
    public void Start()
    {
        skillManager = SkillManager.Instance;
        utilsManager = UtilsManager.Instance;
        currSkillName = "SkillBulletFan";
        ChangeSkill(currSkillName);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsingSkill)
        {
            playerScr.CheckAnimProgress(currSkillData.skillState, currSkillData.skillAnimEndTime, ref isUsingSkill);
        }

    }

    /// <summary>
    /// ���� ����� ������ ��ų�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <return>��ų ��ȯ ���� ����</return>
    public bool InstantiateSkill()
    {
        if (!isUsingSkill && playerScr.CurrInk >= currSkillData.skillCost && !playerAttackControllerScr.IsAttacking)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                { 
                    switch (currSkillData.skillType)
                    {
                        case SkillTypes.FAN:
                            attackEnemy = utilsManager.FindMinDistanceObject(playerScr.Tr.position, currSkillData.skillDist, 1 << 6);
                            if (!DebugUtils.CheckIsNullWithErrorLogging(attackEnemy, "������ �� ��ü�� �����ϴ�."))
                            {
                                isUsingSkill = true;


                                GameObject instantiatedSkill = Instantiate(currSkillObject, playerScr.Tr.position, Quaternion.identity);
                                if (!DebugUtils.CheckIsNullWithErrorLogging(instantiatedSkill, this.gameObject))
                                {
                                    playerScr.Anim.SetTrigger("TurningSkill");
                                    if (attackEnemy.transform.position == null) return false;
                                    spawnVector = attackEnemy.transform.position - playerScr.Tr.position;
                                    playerScr.TurnToDirection(spawnVector);
                                    Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                    if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                    {
                                        skill.SkillInkType = playerScr.SkillInkType;
                                        skill.ActiveSkill(spawnVector.normalized);
                                        playerScr.CurrInk -= skill.SkillCost;
                                        playerScr.RecoverInk();
                                        return true;
                                    }
                                }
                            }
                            break;
                        default:
                            spawnVector = new Vector3(playerScr.Tr.position.x, playerScr.Tr.position.y + 0.1f, playerScr.Tr.position.z);
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
        if (!isUsingSkill && playerScr.CurrInk >= currSkillData.skillCost && !playerAttackControllerScr.IsAttacking)
        {
            //rangedEntity.DisableCircleRenderer();
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(currSkillObject, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<SkillData>(currSkillData, this.gameObject))
                {
                    GameObject instantiatedSkill = Instantiate(currSkillObject, playerScr.Tr.position, Quaternion.identity);
                    if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedSkill, this.gameObject))
                    {
                        switch (currSkillData.skillType)
                        {
                            case SkillTypes.FAN:
                                isUsingSkill = true;
                                playerScr.TurnToDirection(pos);
                                playerScr.Anim.SetTrigger("TurningSkill");
                                Skill skill = DebugUtils.GetComponentWithErrorLogging<Skill>(instantiatedSkill, "Skill");
                                if (!DebugUtils.CheckIsNullWithErrorLogging(skill, this.gameObject))
                                {
                                    skill.SkillInkType = playerScr.SkillInkType;
                                    skill.ActiveSkill(pos.normalized);
                                    playerScr.CurrInk -= skill.SkillCost;
                                    playerScr.RecoverInk();
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
            this.currSkillData.skillInkType = playerScr.SkillInkType;
        }
        return true;
    }
}
