using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : Player
{
    private SkillManager skillManager;

    private GameObject skillObject;
    private SkillData skillData;
    // ��ų ��ȯ ����
    private Vector3 spawnVector;

    // ������ �� ��ü
    Collider attackEnemy;

    private new void Awake()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        skillManager = SkillManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    /// <summary>
    /// ���� ����� ������ ��ų�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="skillName">��ȯ�� ��ų</param>
    /// <return>��ų ��ȯ ���� ����</return>
    public bool InstantiateSkill(string skillName)
    {
        skillObject = skillManager.GetSkillPrefab(skillName);
        if (skillObject == null)
        {
            Debug.LogError("��ȯ�� ��ų ������Ʈ�� �����ϴ�.");
            return false;
        }

        skillData = skillManager.GetSkillData(skillName);
        if (skillData == null)
        {
            Debug.LogError("��ų ������ ���� x");
            return false; 
        }
        
        switch (skillData.skillType)
        {
            case SkillTypes.PAINT:
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, skillData.skillDist, 1 << 6);

                if (attackEnemy == null)
                {
                    Debug.Log("������ �� ��ü�� �����ϴ�.");
                    return false;
                }
                spawnVector = new Vector3(attackEnemy.transform.position.x, tr.position.y + 0.1f, attackEnemy.transform.position.z);
                TurnToDirection(spawnVector);
                anim.SetTrigger("SpawnSkill");
                break;
            case SkillTypes.STROKE:
                anim.SetTrigger("TurningSkill");
                break;
            default:
                spawnVector = new Vector3(tr.position.x, tr.position.y + 0.1f, tr.position.z);
                break;
        }
        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayAudioClip("SkillAttack");
        Instantiate(skillObject, spawnVector, Quaternion.identity);
        return true;
    }

    // ������ ��ġ�� ��ų ��ȯ�ϴ� �Լ�
    public bool InstantiateSkill(string skillName, Vector3 pos)
    {
        skillObject = skillManager.GetSkillPrefab(skillName);
        if (skillObject == null) 
        { 
            Debug.LogError("��ȯ�� ��ų ������Ʈ�� �����ϴ�.");
            return false;
        }
        skillData = skillManager.GetSkillData(skillName);

        if (skillData == null)
        {
            Debug.LogError("��ų ������ ���� x");
            return false;
        }

        switch (skillData.skillType)
        {
            case SkillTypes.PAINT:
                TurnToDirection(pos);
                anim.SetTrigger("SpawnSkill");
                break;
            case SkillTypes.STROKE:
                anim.SetTrigger("TurningSkill");
                Debug.Log("STROKE ��ų �ִϸ��̼� ���");
                break;
            default:
                break;
        }

        Debug.Log("��ų ��ȯ");
        Instantiate(skillObject, targetObjectTr.position, Quaternion.identity);
        return true;
    }
}
