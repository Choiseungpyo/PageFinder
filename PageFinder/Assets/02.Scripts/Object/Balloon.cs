using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public GameObject Particle_Prefab;

    GameObject particle;

    bool didColl = false;

    Palette palette;
    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();

        StartCoroutine(MoveUpDown());
    }

    IEnumerator MoveUpDown()
    {
        Vector3 moveDir = Vector3.up;
        float speed = 1 + Random.Range(0, 10) / 10.0f; // 1.0 ~ 1.9

        while (!didColl)
        {
            if (transform.position.y <= 1.5f)
                moveDir = Vector3.up;    
            else if(transform.position.y > 6)
                moveDir = Vector3.down;

            transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

            yield return null;
        }
    }

    public void ChangeColor()
    {
        if (material.color == Color.red) // ��ǥ�� ������ �̹� �� ��� �������� �ʵ��� ��
            return;

        material.color = palette.ReturnCurrentColor(); // ǳ�� ���� ���� �÷��̾� �ȷ�Ʈ ����� ����

        if (material.color == Color.red) // ��ǥ�� ������ ���������� ����ÿ��� MoveUpDown �ڷ�ƾ�� ����ǵ��� �� 
        {
            particle = Instantiate(Particle_Prefab, transform.position, Quaternion.identity);
            Destroy(particle, 2.0f);
            didColl = true;
        }

    }
}
