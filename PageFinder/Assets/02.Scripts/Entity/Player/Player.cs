using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : Entity
{
    /*
     * �� �� �ʿ��� ������ ����
     */

    protected float img;
    protected float maxMana;
    protected float currMana;
    protected float manaGain;
    protected float attackSpeed;

    [SerializeField]
    protected Transform modelTr;
    protected Transform tr;
    protected Animator anim;
    protected Rigidbody rigid;
    protected UtilsManager utilsManager;
    protected Palette palette;

    [SerializeField]
    private GameObject targetObject;
    protected Transform targetObjectTr;

    public GameObject TargetObject{ get { return targetObject; } }
    public virtual void Awake()
    {
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        SetBasicStatus();
        DontDestroyOnLoad(this.gameObject);
        Hasing();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CaculateDirection(Collider goalObj)
    {
        Vector3 dir = goalObj.gameObject.transform.position - tr.position;
        return dir;
    }
    public void TurnToDirection(Vector3 dir)
    {
        modelTr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
    }

    public virtual void Hasing()
    {
        // ������Ʈ ����
        anim = GetComponentInChildren<Animator>();
        tr = GetComponentInChildren<Transform>();
        rigid = GetComponentInChildren<Rigidbody>();

        utilsManager = UtilsManager.Instance;
        targetObjectTr = targetObject.GetComponent<Transform>();
        targetObject.SetActive(false);
    }

    // �÷��̾� �⺻ �ɷ�ġ ����
    public void SetBasicStatus()
    {
        maxHP = 100.0f;
        atk = 20.0f;
        currHP = maxHP;
        moveSpeed = 10.0f;
    }

    /// <summary>
    /// Ÿ���� ��ü �����̱�
    /// </summary>
    /// <param name="targetingRange">���� ����</param>
    public Vector3 OnTargeting(Vector3 attackDir, float targetingRange)
    {
        SetTargetObject(true);

        // ��Ÿ��� ��� ��� ���ڸ� ����
        if (Vector3.Distance(tr.position, targetObjectTr.position) >= targetingRange)
        {
            targetObjectTr.position = (tr.position - targetObjectTr.position).normalized * targetingRange;
        }
        // Ÿ���� ������Ʈ �����̱�
        else
        {
            targetObjectTr.position = (tr.position + (attackDir) * (targetingRange - 0.1f));
            targetObjectTr.position = new Vector3(targetObjectTr.position.x, 0.1f, targetObjectTr.position.z);
        }
        return targetObjectTr.position;
    }

    public void SetTargetObject(bool isActive)
    {
        targetObjectTr.position = tr.position;
        targetObject.SetActive(isActive);
    }
}
