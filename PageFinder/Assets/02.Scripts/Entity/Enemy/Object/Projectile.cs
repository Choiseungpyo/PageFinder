using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private float speed;
    private string parentName;
    private Vector3 dir;

    private float moveTime = 0;

    private Player playerScr;
    private EnemyAction enemyActionScr;

    private Transform posToCreate;

    private void Awake()
    {
        rb = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(gameObject, "Rigidbody");
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
    }

    private void Update()
    {
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
        ManageMoveTime();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER")) // ���� �տ� �ش� �κ��� �����ų�� �� ��ü�� �����ų�� ����غ���
        {
            //Debug.Log("�÷��̾�� �Ѿ� �浹");
            playerScr.HP -= enemyActionScr.ATK * (enemyActionScr.DefaultAtkPercent / 100);
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

    public void Init(GameObject enemy, string Objectname, float speed, Transform posToCreate)
    {
        this.enemyActionScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(enemy, "EnemyAction");
        gameObject.name = Objectname;

        this.speed = speed;

        this.posToCreate = posToCreate;
        this.dir = Vector3.zero;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// ����ü�� �⺻ ���� ���� �ʱ�ȭ�Ѵ�.
    /// </summary>
    public void Init(Vector3 dir)
    {
        transform.position = new Vector3(posToCreate.position.x, posToCreate.position.y, posToCreate.position.z);
        rb.position = transform.position;

        this.dir = dir;
        this.dir.y =0;

        moveTime = 0;
    }
}
