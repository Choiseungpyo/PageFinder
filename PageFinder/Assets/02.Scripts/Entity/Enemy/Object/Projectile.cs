using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private float speed;
    private string parentName; // �� �Ѿ��� �����س� Stingray ��ü�� ��ȣ 
    private GameObject target;
    private Vector3 targetDir = Vector3.zero;


    private Player playerScr;
    private EnemyAction enemy;



    private Transform posToCreate;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerScr = GameObject.FindWithTag("PLAYER").GetComponent<Player>();
    }

    private void Update()
    {
        rb.MovePosition(rb.position + targetDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER")) // ���� �տ� �ش� �κ��� �����ų�� �� ��ü�� �����ų�� ����غ���
        {
            Debug.Log("Projectile �÷��̾�� �ε���" + (playerScr.HP - enemy.ATK * (enemy.DefaultAtkPercent / 100)));
            playerScr.HP -= enemy.ATK * (enemy.DefaultAtkPercent / 100);
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }
        else if (coll.CompareTag("MAP") || coll.CompareTag("OBJECT"))
        {
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }
    }

    public void Init(string parentName, string Objectname, float speed, Transform posToCreate, GameObject target)
    {
        this.parentName = parentName;
        this.name = name;

        this.speed = speed;

        this.posToCreate = posToCreate;
        this.target = target;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ����ü�� �⺻ ���� ���� �ʱ�ȭ�Ѵ�.
    /// </summary>
    public void SetDirToMove()
    {
        enemy = GameObject.Find(parentName).GetComponent<EnemyAction>();
        transform.position = new Vector3(posToCreate.position.x, posToCreate.position.y, posToCreate.position.z);
        Vector3 bulletDir = (target.transform.position - posToCreate.transform.position).normalized;
        bulletDir.y = 0;
        targetDir = bulletDir;
    }
}
