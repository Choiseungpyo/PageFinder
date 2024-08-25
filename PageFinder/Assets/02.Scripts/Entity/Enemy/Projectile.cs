using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    int speed = 10;

    string parentName; // �� �Ѿ��� �����س� Stingray ��ü�� ��ȣ 

    Vector3 targetDir = Vector3.zero;

    Player playerScr;
    LongRangeAttackEnemy enemy;
    

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

    /// <summary>
    /// ����ü�� �⺻ ���� ���� �ʱ�ȭ�Ѵ�.
    /// </summary>
    public void Init()
    {
        enemy = GameObject.Find(parentName).GetComponent<LongRangeAttackEnemy>();
        transform.position = new Vector3(enemy.transform.position.x, 2, enemy.transform.position.z);
        Vector3 bulletDir = (playerScr.transform.position - enemy.transform.position).normalized;
        bulletDir.y = 0;
        targetDir = bulletDir;
    }

    public string ParentName
    {
        get
        {
            return parentName;
        }
        set
        {
            parentName = value;
        }
    }
}
