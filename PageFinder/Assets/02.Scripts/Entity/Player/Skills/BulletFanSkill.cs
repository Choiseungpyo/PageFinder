using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFanSkill : Skill
{

    public GameObject bulletPrefab;
    protected GameObject[] bullets;
    protected GameObject instatiatedBullet;
    public int bulletCounts;
    private float fanDegree;
    public float bulletSpeed;

    private Vector3 fireDirection;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SetSkillData();
    }

    protected override void SetSkillData()
    {
        FanSkillData fanSkillData = skillData as FanSkillData;
        if(!DebugUtils.CheckIsNullWithErrorLogging<FanSkillData>(fanSkillData, this.gameObject)){
            skillType = fanSkillData.skillType;
            skillCoolTime = fanSkillData.skillCoolTime;
            skillBasicDamage = fanSkillData.skillBasicDamage;
            skillDuration = fanSkillData.skillDuration;
            skillRange = fanSkillData.skillRange;
            skillDist = fanSkillData.skillDist;
            skillState = fanSkillData.skillState;
            skillAnimEndTime = fanSkillData.skillAnimEndTime;
            fanDegree = fanSkillData.fanDegree;
            skillCost = fanSkillData.skillCost;
        }
    }


    public override void ActiveSkill()
    {
        Start();
        CreateBulletsArray();
        FireEachBullet();
        Destroy(this.gameObject, skillDuration);
    }
    public override void ActiveSkill(Vector3 direction)
    {
        Start();
        CreateBulletsArray();
        SetFireDirection(direction);
        FireEachBullet();
        Destroy(this.gameObject, 3.0f);
    }
    public void SetFireDirection(Vector3 direction)
    {
        fireDirection = direction.normalized;
        fireDirection.y = 0;
    }
    public virtual void CreateBulletsArray()
    {
        bullets = new GameObject[bulletCounts];
        if(bullets != null)
        {
            for(int i = 0; i < bullets.Length; i++)
            {
                if(!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(bulletPrefab, this.gameObject))
                {
                    bullets[i] = bulletPrefab;
                }
            }
        }
    }

    public virtual void FireEachBullet()
    {
        // segments ������ ���� ���
        float angleStep = fanDegree / (bulletCounts-1);
        // ��ü������ �ݸ�ŭ �������� �̵��Ͽ� ��ä���� �߾ӿ��� �����ϵ��� ��.
        float currentAngle = -fanDegree / 2;

        if (bullets != null)
        {
            Quaternion rotationToFireDirection = Quaternion.LookRotation(fireDirection.normalized, Vector3.up);
            for (int i = 0; i < bullets.Length; i++)
            {
                instatiatedBullet = Instantiate(bullets[i], tr.position, Quaternion.identity);

                Bullet bullet = DebugUtils.GetComponentWithErrorLogging<Bullet>(instatiatedBullet, "Bullet");
                if(!DebugUtils.CheckIsNullWithErrorLogging<Bullet>(bullet, this.gameObject))
                {
                    bullet.bulletSpeed = bulletSpeed;
                    bullet.Damage = skillBasicDamage;
                    bullet.BulletInkMark = skillInkMark;

                    float radian = Mathf.Deg2Rad * currentAngle;

                    Vector3 postion = new Vector3(Mathf.Cos(radian) * skillRange, 0f, Mathf.Sin(radian) * skillRange);

                    Vector3 rotatedPosition = rotationToFireDirection * postion;

                    // y������ -90�� ȸ���ϴ� ���ʹϾ� ����
                    // �������: Quaternion.LookRotation�� z���� �������� �ϱ� ������ 90�� ȸ���Ǿ� �ִ�.
                    // �׷��⿡ �ٽ� -90���� ����� ���ϴ� ������ ���´�.
                    Quaternion rotationY = Quaternion.Euler(0f, -90.0f, 0f);

                    // ���� ���Ϳ� ȸ�� ����
                    rotatedPosition = tr.position + rotationY * rotatedPosition;

                    bullet.Fire(rotatedPosition);
                }
                currentAngle += angleStep;
            }
        }
    }
}
