using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

/*
 * �����ؾ��� ���� 
 * 1. Ŭ���� ���� ���
 *
 */
public class PlayerController: MonoBehaviour, IPlayerController
{
    #region Move
    private Vector3 moveDir;
    public float moveSpeed = 10.0f;
    #endregion

    #region Attack
    public GameObject targetObject;     // Ÿ���� ǥ��
    public Transform targetObjectTr;
    public Vector3 targetObjectPosition;

    [SerializeField]
    Vector3 attackDir;
    // ������ �� ��ü�� ������ �ݶ��̴� �迭
    Collider[] enemies;
    // ������ �� ��ü
    Collider attackEnemy;
    // �� ������ �ÿ� �޾ƿ� ���ʹ�
    List<Collider> lEnimes;
    float attackRange = 2.6f;
    bool isAttack;
    bool targeting;
    bool isEnableCor;
    #endregion

    #region Component
    private Animator anim;
    Transform tr;
    Rigidbody rigid;
    #endregion

    UtilsManager utilsManager;
    RaycastHit rayHit;

    // Start is called before the first frame update
    void Start()
    {
        Hasing();

        targeting = false;
        attackEnemy = null;
        utilsManager = UtilsManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveDir != Vector3.zero)
        {
            if (!isAttack)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
            }
        }
        if (targeting)
        {
            targetObject.SetActive(true);
            if (Vector3.Distance(tr.position, targetObject.transform.position) >= attackRange)
            {
                targetObject.transform.position = targetObject.transform.position;
            }
            else
            {
                targetObject.transform.position = tr.position + (attackDir) * 2.0f;
            }
        }
    }
   
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    // ª�� ���� �ÿ� ����
    public void ButtonAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("button Attack");
            anim.SetTrigger("Attack");

            // ���� ����� �Ÿ��� �� ã��
            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackRange, 1 << 6);
            if (attackEnemy == null) return;

            TurnToEnemy(attackEnemy);
            Damage(attackEnemy);
        }
    }

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
                targetObject.transform.position = tr.position;
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
            TurnToEnemy(attackEnemy);
            Damage(attackEnemy);
            targetObject.SetActive(false);
        }
    }

    public void TurnToEnemy(Collider attackEnemy)
    {
        // ����� �Ÿ��� �� �������� ĳ���� ȸ��
        Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));
    }

    public void Damage(Collider attackEnemy)
    {
        attackEnemy.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tr.position, attackRange);
        }
    }

    public void Hasing()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }
}
