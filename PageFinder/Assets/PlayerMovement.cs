using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // �÷��̾� �ִϸ�����
    private Animator anim;
    // �̵� ���� ����
    [SerializeField]
    private Vector3 moveDir;

    // �̵��ӵ� 
    public float moveSpeed = 10.0f;
    // ���� ��Ÿ� ǥ�ø� ���� ������Ʈ
    public GameObject rangeObject;
    // ������ �� ��ü�� ������ �ݶ��̴� �迭
    Collider[] enemies;
    // ������ �� ��ü
    Collider attackEnemy;
    // ���� ��Ÿ�
    private float attackDist = 2.6f;
    bool isAttack;
    Transform tr;
    Rigidbody rigid;
    RaycastHit rayHit;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
        rangeObject.SetActive(false);
        attackEnemy = null;
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
   
    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }

    public IEnumerator OnAttack(InputValue value)
    {
        rangeObject.SetActive(true);
        anim.SetTrigger("Attack");
        FindMinDistanceEnemy(tr.position);
        if (attackEnemy != null)
        {
            Vector3 enemyDir = attackEnemy.gameObject.transform.position - tr.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(enemyDir.x, 0, enemyDir.z));
        }
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        rangeObject.SetActive(false);
        isAttack = false;
    }

    public void FindMinDistanceEnemy(Vector3 position)
    {
        attackEnemy = null;
        float minDist = attackDist;
        enemies = Physics.OverlapSphere(position, attackDist, 1 << 6);
        for(int i = 0; i < enemies.Length; i++)
        {
            float dist = Vector3.Distance(position, enemies[i].gameObject.transform.position);
            if(minDist >= dist)
            {
                Debug.Log("enemy Found");
                attackEnemy = enemies[i];
                minDist = dist;
                isAttack = true;
            }
        }
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
