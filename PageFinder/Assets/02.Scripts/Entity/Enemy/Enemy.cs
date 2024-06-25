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

    protected int posType = -1; // ������(����, ����)
    protected int moveType = -1; // �ൿ ����(����̵�, �����̵�)
    protected int attackType = -1; // ���� ����(����, ���� ����)

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

        // ������ ���� Ŭ�������� ���� �Ҵ��ߴٸ� ���⼭ �ٽ� �ʱ�ȭ���� �ʰ�,
        // �Ҵ����� �ʾҴٸ� 0���� �ʱ�ȭ
        if (posType == -1)
            posType = 0;

        if (moveType == -1)
            moveType = 0;

        if (attackType == -1)
            attackType = 0;


        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

}
