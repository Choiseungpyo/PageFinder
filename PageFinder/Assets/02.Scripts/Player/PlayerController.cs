using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

public class PlayerController: MonoBehaviour, IPlayerController
{
    #region Move
    private Vector3 moveDir;
    public float moveSpeed = 10.0f;
    #endregion

    #region Attack
    public GameObject rangeObject;      // ���� ��Ÿ� ǥ��
    public GameObject targetObject;     // Ÿ���� ǥ��

    [SerializeField]
    Vector3 attackDir;
    // ������ �� ��ü�� ������ �ݶ��̴� �迭
    Collider[] enemies;
    // ������ �� ��ü
    Collider attackEnemy;
    float attackDist = 2.6f;
    bool isAttack;
    bool targeting;
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
        rangeObject.SetActive(false);
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
            StartCoroutine(SetTarget());
        }
    }
   
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    public void ButtonAttack(InputAction.CallbackContext context)
    {

        rangeObject.SetActive(true);
        if (context.performed)
        {
            Debug.Log("button Attack");
            anim.SetTrigger("Attack");

            // ���� ����� �Ÿ��� �� ã��
            attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackDist, 1 << 6);
            if (attackEnemy == null) return;

            // ����� �Ÿ��� �� �������� ĳ���� ȸ��
            Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));

        }
        if (context.canceled)
        {
            rangeObject.SetActive(false);
        }
    }
    public void JoystickAttack(InputAction.CallbackContext context)
    {
        rangeObject.SetActive(true);

        Vector2 inputVec = context.ReadValue<Vector2>();
        if(context.action.phase == InputActionPhase.Started)
        {
            targetObject.transform.position = tr.position;
            targeting = true;
            attackDir = new Vector3(inputVec.x, 0, inputVec.y);
        }
        if (context.action.phase == InputActionPhase.Canceled)
        {
            targeting = false;
        }
    }

    public IEnumerator SetTarget()
    {
        targetObject.SetActive(true);
        if(Vector3.Distance(tr.position, targetObject.transform.position) >= attackDist)
        {
            targetObject.transform.position = targetObject.transform.position;
        }
        else
        {
            targetObject.transform.rotation = Quaternion.LookRotation(attackDir);
            targetObject.transform.Translate(Vector3.forward * 0.2f);
        }
        
        yield return new WaitUntil(() => targeting == false);

        targetObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tr.position, attackDist);
        }
       
    }

    public void Hasing()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }
}
