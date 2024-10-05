using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: Player
{
    #region Variables
    // Move
    private Vector3 moveDir;
    [SerializeField]
    private MoveJoystick moveJoystick;
    private PlayerInk playerInkScr;
    // Dash
    private float dashPower;
    private float dashDuration;
    private float dashWidth;
    private float dashCooltime;
    private bool isDashing;
    private INKMARK dashInkMark;
    #endregion

    #region Properties
    public float DashPower { get => dashPower; set => dashPower = value; }
    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    public float DashCooltime { get => dashCooltime; set => dashCooltime = value; }
    public float DashWidth { get => dashWidth; set => dashWidth = value; }

    #endregion

    public override void Awake()
    {
        base.Awake();

        dashCooltime = 1.0f;
        dashPower = 5.0f;
        dashDuration = 0.28f;
        DashWidth = 1.5f;
        isDashing = false;
        dashInkMark = INKMARK.RED;
    }
    public override void Start()
    {
        base.Start();
        TargetObject.SetActive(true);
        playerInkScr = GetComponent<PlayerInk>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            // Ű���� �̵�
            KeyboardControl();
            // ���̽�ƽ �̵�
            JoystickControl();

            anim.SetFloat("Movement", moveDir.magnitude);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            dashInkMark = INKMARK.RED;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            dashInkMark = INKMARK.BLUE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            dashInkMark = INKMARK.GREEN;
        }
    }

    private void KeyboardControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = new Vector3(h, 0, v).normalized;
        if (h != 0 || v != 0)
        {
           Move(moveDir);
        }
    }

    private void JoystickControl()
    {
        // �̵� ���̽�ƽ�� x, y �� �о����
        float x = moveJoystick.Horizontal();
        float y = moveJoystick.Vertical();

        if(x!= 0 || y != 0)
        {
            moveDir = new Vector3(x, 0, y).normalized;
            Move(moveDir);
        }
    }

    private void Move(Vector3 moveDir)
    {
        tr.Translate(modelTr.forward * moveSpeed * Time.deltaTime);
        //tr.position += moveDir * moveSpeed * Time.deltaTime;
        TurnToDirection(moveDir);
    }

    public void Dash(Vector3? dir = null)
    {
        StartCoroutine(DashCouroutine(dir));
        Debug.Log("Dash");
    }
    public IEnumerator DashCouroutine(Vector3? dashDir)
    {
        isDashing = true;
        float leftDuration = dashDuration;
        Vector3 dest;
        if(dashDir == null)
        {
            dest = tr.position + modelTr.forward * dashPower;
            Debug.Log("Short Dash");
        }
        else
        {
            TurnToDirection(((Vector3)dashDir).normalized);
            dest = tr.position + ((Vector3)dashDir).normalized * dashPower;
            Debug.Log("Long Dash");
        }

        float dashSpeed = dashPower / leftDuration; // ������ �ӵ� ���
        float currentDuration = 0f;
        Transform inkObjTransform = null;
        if (playerInkScr)
        {
            Vector3 direction = (dest - tr.position).normalized;
            Vector3 position = tr.position + direction * (dashPower / 2);
            position.y += 0.1f;

            inkObjTransform = playerInkScr.CreateInk(INKTYPE.LINE, position);
            if (inkObjTransform.TryGetComponent<InkMark>(out InkMark inkMarkScr))
            {
                inkMarkScr.CurrMark = dashInkMark;
                inkMarkScr.SetMaterials();
            }
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            inkObjTransform.rotation = Quaternion.Euler(90, angle, 0);
        }
        float size = 0;
        Vector3 originPos = tr.position;
        while (currentDuration < leftDuration)
        {
            // ���� ��ġ���� ��ǥ ��ġ���� ������ �ӵ��� �̵�
            tr.position = Vector3.MoveTowards(tr.position, dest, dashSpeed * Time.deltaTime);
            size = Vector3.Distance(originPos, tr.position);
            if (inkObjTransform)
            {
                inkObjTransform.localScale = new Vector3(1.5f, size, 0);
            }

            yield return null; // ���� �����ӱ��� ���

            currentDuration += Time.deltaTime;
        }

        isDashing = false;

    }
}
