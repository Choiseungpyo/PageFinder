using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : BossController
{
    Vector3 rushPos = Vector3.zero;


    /*
     * <�ɷ�ġ>
     * �̼� : 0.7
     * �ִ�ü�� : 200
     * ���ݷ� : 10
     * 
     * <Ÿ�� ����>
     * posType : x -> 0���� �ڵ����õ�
     * moveType : x -> 0���� �ڵ����õ�
     * attackType : x -> 0���� �ڵ����õ�
     * 
     * <��ų ��Ÿ��>
     * ��ų : ���� 
     * �ִ� ��ų ��Ÿ�� : 10��
     * 
     * <���� ��ƾ>
     * �÷��̾ ���� ������ ������ ���� ��� -> �������� ����
     * �÷��̾ ���� ������ ������ ��� -> ���� ����
     * 
     * ��ų ��Ÿ���� ���� ���� ��� -> �÷��̾ ��� �����Ͽ� ����
     * ��ų ��Ÿ���� �Ǿ��� ��� -> ���� : �÷��̾ �ִ� �������� �ʿ� ���������� 1ȸ �����Ѵ�. ���� �� �ٽ� ���� �̵�
     * 
     *
     */


    // Start is called before the first frame update
    public override void Start()
    {
        // �⺻ �ɷ�ġ ����
        // �̼�, �ִ� ü��, ���� ü��, ���ݷ�, ����
        moveSpeed = 0.7f;
        maxHP = 200.0f;
        atk = 10.0f;

        // Ÿ�� ����
        posType = 0;
        moveType = 2; // ���� �̵�
        attackType = 0;

        // ��ġ ����
        mapCenterPos = new Vector3(-25, 1, -25);

        // ��ų ��Ÿ��
        maxSkillCoolTime = 5;

        base.Start();
    }


    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    break;
                case State.MOVE:
                    meshRenderer.material.color = Color.green;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    //Debug.Log("Trace");
                    meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    //Debug.Log("Attack");
                    meshRenderer.material.color = Color.red;
                    break;
                case State.SKILL:
                    //Debug.Log("SKILL");
                    //Debug.Log(rushPos);
                    meshRenderer.material.color = Color.yellow;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;

                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }


    protected override IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            //meshRenderer.material.color = Color.green;
            yield return null;

            float distance = Vector3.Distance(playerTr.transform.position, mapCenterPos);

            // �÷��̾ ���� ������ ������ ���� ��� �������� �ʵ��� �Ѵ�.
            if (distance > 50) // �� ���� : 50
            {
                Debug.Log("�÷��̾�� ���� ������ ���� �ʴ�.");
                continue;
            }

            distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);

            if (!CheckSkillCoolTimeIsEnded()) // ��ų ��Ÿ���� ������ ���� ���
            {
                //Debug.Log("��ų ��Ÿ���� ������ ���� ���");
                state = State.TRACE;
                if (distance <= attackDist)
                {
                    state = State.ATTACK;
                }
            }
            else // ��ų ��Ÿ���� ���� ���
            {
                if (CheckIfSkillIsUsing()) // ��ų�� ������� ���
                {
                    if(CheckIfItCollWithMap()) // ���� ���� ���
                    {
                        Debug.Log("���� ����");
                        currentSkillCoolTime = 0;
                        usingSkill = false;
                        moveSpeed = 3.5f;
                        agent.speed = moveSpeed;
                        state = State.TRACE;
                    }
                    //Debug.Log("��ų �����");
                    continue;
                }
                    
                Debug.Log("��ų ��� ����");
                // ó�� ��ų ����ϴ� ���
                usingSkill = true;
                moveSpeed = 15;
                agent.speed = moveSpeed;
                state = State.SKILL;
            }
        }
    }

    bool CheckIfItCollWithMap()
    {
        return Physics.Raycast(monsterTr.position, monsterTr.forward, 5, LayerMask.GetMask("MAP")) ? true : false;
    }
}
