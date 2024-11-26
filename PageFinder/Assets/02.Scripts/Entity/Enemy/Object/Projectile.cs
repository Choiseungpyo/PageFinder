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

    private float moveTime = 0;

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
        ManageMoveTime();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER")) // ���� �տ� �ش� �κ��� �����ų�� �� ��ü�� �����ų�� ����غ���
        {
            //Debug.Log("�÷��̾�� �Ѿ� �浹");
            playerScr.HP -= enemy.ATK * (enemy.DefaultAtkPercent / 100);
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }
        else if (coll.CompareTag("MAP") || coll.CompareTag("OBJECT") || coll.includeLayers.Equals("MAP"))
        {
            //Debug.Log("Map or Object�� �Ѿ� �浹");
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }
    }

    private void ManageMoveTime()
    {
        if (moveTime > 5.0f)
        {
            moveTime = 0;
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
            return;
        }

        moveTime += Time.deltaTime;
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
        rb.position = transform.position;
        Vector3 bulletDir = (target.transform.position - posToCreate.transform.position).normalized;
        bulletDir.y = 0;
        targetDir = bulletDir;
        moveTime = 0;
    }
}
