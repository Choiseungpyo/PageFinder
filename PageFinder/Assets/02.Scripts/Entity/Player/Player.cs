using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Diagnostics.CodeAnalysis;

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
    protected float attackRange;

    [SerializeField]
    protected Transform modelTr;
    protected Transform tr;
    protected Animator anim;
    protected Rigidbody rigid;
    protected UtilsManager utilsManager;
    protected EventManager eventManager;
    protected Palette palette;
    protected RangedEntity rangedEntity;

    [SerializeField]
    private GameObject targetObject;
    protected Transform targetObjectTr;
    private PlayerHPBar hpBar;
    private SliderBar manaBar;

    public override float HP
    {
        get
        {
            return currHP;
        }
        set
        {
            currHP = value;

            // UI ����
            hpBar.SetCurrValueUI(currHP);

            if (currHP <= 0)
            {
                Die();
                EndGame();
            }
        }
    }

    public override float MAXHP 
    {
        get
        {
            return maxHP;
        }
        set 
        { 
            maxHP = value;

            // UI ����
            hpBar.SetMaxValueUI(maxHP);
            hpBar.SetGradation();
        } 
    }

    public float Mana
    {
        get { 
            return currMana; 
        }
        set 
        { 
            currMana = value;
            
            if(currMana <=0)
            {
                currMana = 0;
            }
            manaBar.SetCurrValueUI(currMana);
        }
    }


    public float AttackSpeed
    {
        get { return attackSpeed; }
        set
        {
            attackSpeed = value;
            anim.SetFloat("AttackSpeed", attackSpeed);
        }
    }

    public float AttackRange
    {
        get { return attackRange; }
        set
        {
            attackSpeed = value;
        }
    }
    public GameObject TargetObject{ get { return targetObject; } }
    public virtual void Awake()
    {
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        Hasing();
        SetBasicStatus();
        DontDestroyOnLoad(this.gameObject);

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
        eventManager = EventManager.Instance;
        targetObjectTr = targetObject.GetComponent<Transform>();
        targetObject.SetActive(false);

        rangedEntity = GetComponent<RangedEntity>();
    }

    // �÷��̾� �⺻ �ɷ�ġ ����
    public void SetBasicStatus()
    {
        maxHP = 1000.0f;
        atk = 60.0f;
        currHP = maxHP;
        moveSpeed = 10.0f;
        attackSpeed = 2.5f;
        maxMana = 500.0f;
        currMana = maxMana;
        anim.SetFloat("AttackSpeed", attackSpeed);
        attackRange = 2.6f;

        // HP Bar
        hpBar = GetComponentInChildren<PlayerHPBar>();
        hpBar.SetMaxValueUI(maxHP);
        hpBar.SetCurrValueUI(currHP);

        // Mana Bar
        manaBar = GameObject.Find("ManaBar").GetComponent<SliderBar>();
        manaBar.SetMaxValueUI(maxMana);
        manaBar.SetCurrValueUI(currMana);
    }

    /// <summary>
    /// Ÿ���� ��ü �����̱�
    /// </summary>
    /// <param name="targetingRange">���� ����</param>
    public virtual void OnTargeting(Vector3 attackDir, float targetingRange)
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
    }

    public void SetTargetObject(bool isActive)
    {
        targetObjectTr.position = tr.position;
        targetObject.SetActive(isActive);
    }
    
    public void EndGame()
    {
        eventManager.PostNotification(EVENT_TYPE.GAME_END, this);
    }
}
