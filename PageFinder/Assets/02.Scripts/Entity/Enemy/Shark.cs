using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : BossController
{
    Vector3 rushPos = Vector3.zero;



    // Start is called before the first frame update
    public override void Start()
    {
        // �⺻ �ɷ�ġ ����
        // �̼�, �ִ� ü��, ���� ü��, ���ݷ�, ����
        moveSpeed = 0.7f;
        maxHP = 40.0f;
        atk = 10.0f;

        // Ÿ�� ����
        posType = 0;
        moveType = 2; // ���� �̵�
        attackType = 0;

        // ��ġ ����
        originalPos = new Vector3(-70, 0.86f, -82);

        base.Start();

        //Debug.Log((new Vector3(2, 0 , 0) - new Vector3(5, 0 , 0)).normalized);
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
                    Debug.Log("Trace");
                    meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    Debug.Log("Attack");
                    meshRenderer.material.color = Color.red;
                    break;
                case State.SKILL:
                    meshRenderer.material.color = Color.yellow;
                    agent.SetDestination(rushPos);
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
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);


            if (!CheckSkillCoolTimeIsEnded() && !usingSkill) // ��ų ��Ÿ���� ������ ���� ���
            {
                if (distance <= attackDist)
                {
                    state = State.ATTACK;
                }
                else if (distance <= traceDist)
                {
                    state = State.TRACE;
                }
            }
            else // ��ų ��Ÿ���� ���� ���
            {
                if (CheckIfSkillIsUsing()) // ��ų�� ������� ���
                    continue;

                // ó�� ��ų ����ϴ� ���
                usingSkill = true;

                //Debug.Log((playerTr.position - monsterTr.position).Normalize());
                rushPos = (playerTr.position - monsterTr.position).normalized * 45; // ����ȭ�� ���� * �� �ִ� ũ��(���� ���� �̻�) -> �� �÷��̾ �ִ� ������ �� �κ�
                Debug.Log("���� ��ġ : " + (playerTr.position - monsterTr.position).normalized);
                state = State.SKILL;

                // ������ �÷��̾ ����Ͽ� �� �� ���� �ƴϸ� ���� ������ �����غ���
            }
        }
    }
}
