using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillTypes
{
    BASICATTACK, STROKE, PAINT
}

public class Skill : MonoBehaviour, IType
{
    protected GameObject playerObj;
    protected Transform tr;

    protected SkillTypes skillType;
    protected float skillCoolTime;
    protected float skillBasicDamage;
    protected float skillDuration;
    protected float skillRange;
    protected float skillDist;
    protected IType.TYPE currType = IType.TYPE.PURPLE;


    public SkillData skillData;
    public SkillTypes SkillType { get; set; }
    public float SkillCoolTime { get; set; }
    public float SkillBasicDamage { get; set; }
    public float SkillDuration { get; set; }
    public float SkillRange { get; set; }
    public float SkillDist { get; set; }
    public IType.TYPE CurrType { get; set; }
    // Start is called before the first frame update
    public virtual void Start()
    {
        Hashing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void SetSkillData()
    {
        if (skillData == null)
        {
            Debug.LogError("��ų ������ ����");
            return;
        }
        skillType = skillData.skillType;
        skillCoolTime = skillData.skillCoolTime;
        skillBasicDamage = skillData.skillBasicDamage;
        skillDuration = skillData.skillDuration;
        skillRange = skillData.skillRange;
        skillDist = skillData.skillDist;
    }

    /// <summary>
    /// ��ų �� ����
    /// </summary>
    /// <param name="skillType">��ų Ÿ��</param>
    /// <param name="skillCoolTime">��ų ��Ÿ��</param>
    /// <param name="skillBasicDamage">��ų �⺻ ������</param>
    /// <param name="skillDuration">��ų ���� �ð�</param>
    /// <param name="skillRange">��ų ����</param>
    /// <param name="skillDist">��ų ��Ÿ�</param>
    protected void SetSkillStatus(SkillTypes skillType, float skillCoolTime, float skillBasicDamage, 
        float skillDuration, float skillRange, float skillDist)
    {
        this.skillType = skillType;
        this.skillCoolTime = skillCoolTime;
        this.skillBasicDamage = skillBasicDamage;
        this.skillDuration = skillDuration;
        this.skillRange = skillRange;
        this.skillDist = skillDist;
    }

    public void Hashing()
    {
        playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        tr = GetComponent<Transform>();
    }

    protected bool CheckDistanceToDestroy(Vector3 originPos, Vector3 currPos)
    {
        float distance = Vector3.Distance(originPos, tr.position);
        return distance >= skillDist ? true : false;
    }
}
