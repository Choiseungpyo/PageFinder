using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benshee : LongRangeAttackEnemy
{

    public GameObject SoundWaveObject_Prefab;

    float maxSoundWaveReloadTime = 0.3f;
    float currentSoundWaveReloadTime = 0;

    int soundWavesIndex = 0;
    GameObject[] SoundWaves = new GameObject[3];


    public override void Start()
    {
        base.Start();

        // ����ü ����
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            SoundWaves[i] = Instantiate(SoundWaveObject_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform); //GameObject.Find("Bullet").transform
            SoundWaves[i].name = gameObject.name + " - SoundWaves" + i;
            SoundWaves[i].GetComponent<Projectile>().ParentName = gameObject.name;
            SoundWaves[i].SetActive(false);
        }
    }

    public override void Update()
    {
        SetReloadTime();
        SetSoundWaveReloadTime();
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            SetCurrentSkillCoolTime();
            ChangeCurrentStateToSkillState();

            switch (state)
            {
                case State.IDLE:
                    ani.SetBool("isIdle", true);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);
                    break;
                case State.MOVE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", true);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = true;
                    FireProjectileObject();
                    break;
                case State.STUN:
                    ani.SetFloat("stunTime", stunTime);
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", true);

                    agent.isStopped = true;
                    break;
                case State.SKILL:
                    // �ش� �� Ŭ�������� �������Ͽ� ���ϴ� ��ų�� ȣ���Ѵ�. 
                    Debug.Log("Skill ���");
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = true;
                    SoundWave();
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return null;
        }
    }

    void SoundWave()
    {
        if (currentSoundWaveReloadTime < maxSoundWaveReloadTime)
            return;

        int soundWavesIndex = FindSoundWaveThatCanBeUsed();

        if (soundWavesIndex == -1) // ����� �� �ִ� �Ѿ��� ���� ��� 
            return;
        else if(soundWavesIndex == SoundWaves.Length-1) // ������ ���ĸ� ������ �� �� ��ų ����� �� �ֵ��� �ʱ�ȭ
        {
            skillUsageStatus[0] = false;
            state = State.MOVE;
        }
           

        //Debug.Log("�Ѿ� �߻�");
        SoundWaves[soundWavesIndex].SetActive(true);
        SoundWaves[soundWavesIndex].GetComponent<Projectile>().Init();
        currentSoundWaveReloadTime = 0;
    }

    int FindSoundWaveThatCanBeUsed()
    {
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            if (SoundWaves[i].activeSelf) // ������� �Ѿ� 
                continue;
            return i;
        }
        return -1;
    }

    protected void SetSoundWaveReloadTime()
    {
        if (currentSoundWaveReloadTime >= maxSoundWaveReloadTime)
            return;

        currentSoundWaveReloadTime += Time.deltaTime;
    }
}
