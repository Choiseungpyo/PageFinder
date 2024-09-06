using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack : MonoBehaviour
{
    [Header("Proectile")]
    public GameObject Projectile_Prefab;
    public Transform projectilePos;

    //List �� �����Ͽ� ���� �ɵ������� ������ �� �ְ� �غ���
    GameObject[] projectile = new GameObject[3];
    GameObject playerObj;

    private void Start()
    {
        playerObj = GameObject.FindWithTag("PLAYER");
        // ����ü ����
        for (int i = 0; i < projectile.Length; i++)
        {
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(gameObject.transform.position.x, -10, gameObject.transform.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform);
            projectile[i].GetComponent<Projectile>().Init(gameObject.name, gameObject.name + " - Projectile" + i, 10, projectilePos, playerObj);
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
        projectile[projectileIndex].GetComponent<Projectile>().SetDirToMove();
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
