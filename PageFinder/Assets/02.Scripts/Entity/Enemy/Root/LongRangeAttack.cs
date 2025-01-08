using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack : MonoBehaviour
{
    [Header("Proectile")]
    [SerializeField]
    private GameObject Projectile_Prefab;
    [SerializeField]
    private Transform projectilePos;
    [SerializeField]
    private int speed;

    //List �� �����Ͽ� ���� �ɵ������� ������ �� �ְ� �غ���
    GameObject[] projectile = new GameObject[6];

    private void Start()
    {
        // ����ü ����
        for (int i = 0; i < projectile.Length; i++)
        {
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(gameObject.transform.position.x, -10, gameObject.transform.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform);
            projectile[i].GetComponent<Projectile>().Init(gameObject, gameObject.name + " - Projectile" + i, speed, projectilePos);
        }
    }

    private void FireProjectileObject()
    {
        int projectileIndex = FindBulletThatCanBeUsed();
        if (projectileIndex == -1) // ����� �� �ִ� �Ѿ��� ���� ��� 
        {
            Debug.Log("����� �Ѿ� ����");
            return;
        }
        projectile[projectileIndex].SetActive(true);
        projectile[projectileIndex].GetComponent<Projectile>().Init(projectilePos.position - transform.position);
    }

    /// <summary>
    /// ����� �� �ִ� ����ü�� ã�´�.
    /// </summary>
    /// <returns>-1 : ����� �� �ִ� ����ü ���� / 0~Bullet.Length-1 : ����� �� �ִ� ����ü �ε���</returns>
    int FindBulletThatCanBeUsed()
    {
        for (int i = 0; i < projectile.Length; i++)
        {
            if (projectile[i].activeSelf) // ������� �Ѿ� 
                continue;
            return i;
        }
        return -1;
    }
}
