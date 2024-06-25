using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stingray : EnemyController
{
    // Start is called before the first frame update
    public override void Start()
    {
        // �⺻ �ɷ�ġ ����
        // �̼�, �ִ� ü��, ���� ü��, ���ݷ�, ����
        moveSpeed = 0.8f;
        maxHP = 40.0f;
        atk = 10.0f;

        // Ÿ�� ����
        posType = 1; // ����
        moveType = 1; // ���� �̵�
        attackType = 0;

        cognitiveDist = 5f;

        base.Start();
    }

    /// <summary>
    /// ���� �̵�
    /// </summary>
    public override void MoveRandom()
    {
        
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        Debug.Log(distance);
        if (distance <= 1.5f)
        {
            SetCurrentPosIndexToMove();

            // ���� �̵��� ��ǥ�� �����ϰ� ����
            while (distance < cognitiveDist) // ���� ��ǥ�� ���� ���� ������ ���� ������ ��ǥ�� �Ÿ��� �ּ� 3�̻� �� �� �ְ� ����
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);

            }
        }


        if (!CheckCognitiveDist())
            return;

        distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
        }
        else if (distance <= traceDist)
        {
            state = State.TRACE;
        }

        Debug.Log(state);
    }

    // ����ü �߻� ���� 

    /* <������ ���� ��ƾ>
     * 1. ���� �����Ÿ� �̳�
     * 2. ����ü 1ȸ �߻� (������) 
     * 3. 1�� �� 2������ 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
}
