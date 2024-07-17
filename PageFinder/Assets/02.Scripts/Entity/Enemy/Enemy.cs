using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField]
    private Slider hpBar;

    // �̵��� ��ǥ
    public Vector3 originalPos;

    protected int posType; // ������(����, ����)
    protected int moveType; // �ൿ ����(����̵�, �����̵�)
    protected int attackType; // ���� ����(����, ���� ����)

    protected MeshRenderer meshRenderer;
    // ���ʹ��� ��� ����
    protected bool isDie = false;

    public override float HP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            hpBar.value = currHP;
            if (currHP <= 0)
            {
                Die();
            }
        }
    }
    public override void Start()
    {
        base.Start();

        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}
