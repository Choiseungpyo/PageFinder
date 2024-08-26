using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : StateMachineBehaviour
{
    bool isAttack = false;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // EnemyController�� �ƴ϶� ��ӹ޴� �ٸ� Ŭ�����ֱ� ������ �׿� ��ó�ϴ� �ڵ� �ʿ���.
        EnemyController enemyController = animator.gameObject.GetComponent<EnemyController>();

        // ���߿� ���� �ִϸ��̼� �ϴ� �߿� �÷��̾�� �������� ���� Ÿ�ֿ̹� ���� �����Ͱ��� ���� �����ϵ��� �����ϱ�

        // �ִϸ��̼��� ���� �̻� �������� ���
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && !isAttack)
        {
            if (!enemyController.CheckIfPlayerIsWithInAttackRange())
                return;

            // ���� ���� �ȿ� �÷��̾ ���� ���
            isAttack = true;
            enemyController.playerScr.HP -= enemyController.ATK * (enemyController.DefaultAtkPercent / 100);
            Debug.Log("�÷��̾� HP : " + enemyController.playerScr.HP);
            enemyController.state = EnemyController.State.IDLE;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemyController = animator.gameObject.GetComponent<EnemyController>();

        enemyController.CurrDefaultAtkCoolTime = enemyController.MaxDefaultAtkCoolTime;
        //enemyController.state = EnemyController.State.MOVE;
        isAttack = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
