using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : Player
{
    #region Attack
    public GameObject targetObject;     // Ÿ���� ǥ��
    private Transform targetObjectTr;

    [SerializeField]
    Vector3 attackDir;
    // ������ �� ��ü
    Collider attackEnemy;

    float attackRange = 2.6f;
    float attackPlus = 2.2f;
    float touchStartTime;
    bool targeting;


    #endregion

    #region Skills
    public GameObject[] skillPrefabs;
    RaycastHit skillRayHit;

    bool skillTargeting = false;
    float skillDist = 5.0f;
    #endregion
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        targeting = false;
        attackEnemy = null;
        targetObjectTr = targetObject.GetComponent<Transform>();
        targetObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (targeting)
        {
            targetObject.SetActive(true);
            if (Vector3.Distance(tr.position, targetObjectTr.position) >= attackRange)
            {
                targetObjectTr.position = targetObjectTr.position;
            }
            else
            {
                targetObjectTr.position = tr.position + (attackDir) * attackPlus;
                Debug.Log(attackDir);
            }
        }
        else if (skillTargeting)
        {
            targetObject.SetActive(true);
            if (Vector3.Distance(tr.position, targetObjectTr.position) >= skillDist)
            {
                targetObjectTr.position = targetObjectTr.position;
            }
            else
            {
                targetObjectTr.position = tr.position + (attackDir) * (skillDist-0.1f);
                Debug.Log(attackDir);
            }
        }
        else
        {
            targetObject.SetActive(false);
        }

    }

    // ª�� ���� �ÿ� ����
    public void ButtonAttack(InputAction.CallbackContext context)
    {
        if(context.started)
            touchStartTime = Time.time;
        // ��ư�� ������ ���� �ÿ��� �۵��ϵ���
        if (context.canceled )
        {
            float touchDuration = Time.time - touchStartTime;
            if (touchDuration >= 0.3f) return;
            // ���� �������� �ִϸ��̼��� ���� �ִϸ��̼��� �ƴ� ���� ���� ����
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Warrior_Attack"))
            {
                Debug.Log("button Attack");
                anim.SetTrigger("Attack");

                // ���� ����� �Ÿ��� �� ã��
                attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
                if (attackEnemy == null) return;

                TurnToDirection(CaculateDirection(attackEnemy));
                Damage(attackEnemy);
                //portal.ChangeColor(palette.ReturnCurrentColor());
            }
        }
    }

    // ��� ���� �ÿ� ����
    public void JoystickAttack(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();

        // �̹� Ÿ���� ���� ���
        if (targeting)
        {
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        else
        {
            if (context.started)
            {
                targetObjectTr.position = tr.position;
                targeting = true;
            }
            
        }
        if (context.canceled)
        {
            targeting = false;
            attackEnemy = targetObject.GetComponent<attackTarget>().GetClosestEnemy();
            if (attackEnemy == null) return;

            Debug.Log("Targeting Attack");
            anim.SetTrigger("Attack");

            TurnToDirection(CaculateDirection(attackEnemy));
            Damage(attackEnemy);
            targetObject.SetActive(false);
        }
    }
    public void ButtonSkill(InputAction.CallbackContext context)
    {
        if (context.started)
            touchStartTime = Time.time;
        // ��ư�� ������ ���� �ÿ��� �۵��ϵ���
        if (context.canceled)
        {
            float touchDuration = Time.time - touchStartTime;
            if (touchDuration >= 0.3f) return;

            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, skillDist, 1 << 6);
            if (attackEnemy == null) return;
            SetSkillPos(attackEnemy.transform.position, skillPrefabs[0]);
        }
    }

    public void JoystickSkill(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();
        // �̹� Ÿ���� ���� ���
        if (skillTargeting)
        {
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        else
        {
            if (context.started)
            {
                targetObjectTr.position = tr.position;
                skillTargeting = true;
            }

        }
        if (context.canceled)
        {
            skillTargeting = false;

            SetSkillPos(targetObject.transform.position, skillPrefabs[0]);

        }
    }
    public void Damage(Collider attackEnemy)
    {
        attackEnemy.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        attackEnemy.GetComponent<EnemyController>().Die();
    }

    public void SetSkillPos(Vector3 pos, GameObject skillObject)
    {
        if (skillObject == null) Debug.LogError("��ų ������Ʈ�� �������� �ʽ��ϴ�.");
        Instantiate(skillObject, pos, Quaternion.identity);
    }
}
