using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController: MonoBehaviour
{
    #region Move
    private Vector3 moveDir;
    public float moveSpeed = 10.0f;
    #endregion

    #region Attack
    public GameObject rangeObject;      // ���� ��Ÿ� ǥ��
    public GameObject targetObject;     // Ÿ���� ǥ��

    // ������ �� ��ü�� ������ �ݶ��̴� �迭
    Collider[] enemies;
    // ������ �� ��ü
    Collider attackEnemy;
    float attackDist = 2.6f;
    float clickTime = 1.0f;
    bool isAttack;
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
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();

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
    }
   
    public void Move(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    public IEnumerator Attack(InputAction.CallbackContext context)
    {
        if(context.time <= 0.4f)
        {
            Debug.Log("Tap");
        }
        else
        {
            Debug.Log("Hold");
        }
        yield return new WaitForSeconds(0.1f);
        /*rangeObject.SetActive(true);
        anim.SetTrigger("Attack");

        // ���� ����� �Ÿ��� �� ã��
        attackEnemy = utilsManager.FindMinDistanceObject(tr.position, attackDist, 1 << 6);
        if (attackEnemy == null) yield break;

        // ����� �Ÿ��� �� �������� ĳ���� ȸ��
        Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));
        isAttack = true;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        rangeObject.SetActive(false);
        isAttack = false;*/
    }

    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tr.position, attackDist);
        }
       
    }
}
