using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ShieldManager : MonoBehaviour
{
    private List<Shield> temporaryShields = new List<Shield>();
    private List<Shield> permanentShields = new List<Shield>();
    private float curShield;
    private float maxShield;
    public float CurShield { get => curShield; set => curShield = value; }
    public float MaxShield { get => maxShield; set => maxShield = value; }

    public WaitForSeconds shieldDelayDuration;
    private bool isAbleCreateShield = true;
    public class Shield
    {
        public float maxValue;
        public float curValue;
        public float duration;
        public float timeRemaning;

        public Shield(float maxValue, float duration)
        {
            this.maxValue = maxValue;
            this.curValue = maxValue;
            this.duration = duration;
            this.timeRemaning = duration;
        }

        public void UpdateShield(float deltaTime)
        {
            // ���ӽð��� ���� �ǵ��ϰ�� ���ӽð� ��� x
            if (duration == -1) return;

            timeRemaning -= deltaTime;
            if (timeRemaning <= 0)
            {
                curValue = 0;
            }
        }
    }

    private void Start()
    {
        shieldDelayDuration = new WaitForSeconds(1.0f);
    }

    void Update()
    {
        // ���ӽð��� �����ϴ� �ǵ尡 �����Ǿ��� ��� �ǵ� ���ӽð� ���
        if(temporaryShields.Count >= 1)
        {
            UpdateShieldRemaningTime();
        }
    }

    private void UpdateShieldRemaningTime()
    {
        for(int i = temporaryShields.Count-1; i >= 0; i--)
        {
            temporaryShields[i].UpdateShield(Time.deltaTime);
            if (temporaryShields[i].timeRemaning <= 0)
            {
                temporaryShields.Remove(temporaryShields[i]);
                UpdateShieldValues();
            }
        }
    }

    public void GenerateShield(float value, float duration)
    {
        if (!isAbleCreateShield) return; // �ǵ���Ÿ�� �ÿ� return

        Shield shield = new Shield(value, duration);
        // �ǵ� �߰��ÿ� maxShield���� Ŀ���� �ʵ���
        if (curShield + shield.curValue > maxShield) shield.curValue = maxShield - curShield;

        if (duration == -1) permanentShields.Insert(0, shield);
        else temporaryShields.Insert(0, shield);
        UpdateShieldValues();

        StartCoroutine(ShieldDelay());
    }

    public void UpdateShieldValues()
    {
        curShield = 0f;
        foreach(var shield in temporaryShields)
        {
            curShield += shield.curValue;
        }
        foreach(var shield in permanentShields)
        {
            curShield += shield.curValue;
        }

        curShield = Mathf.Min(curShield, maxShield);
    }

    public float CalculateDamageWithDecreasingShield(float damage)
    {
        //  ���� ���ӽð��� �����ϴ� �ǵ忡�� ������ ����(������ �ǵ� ���� ����)
        for(int i = temporaryShields.Count-1; i >= 0; i--)
        {
            if(temporaryShields[i].curValue > 0)
            {
                float absorbed = Mathf.Min(damage, temporaryShields[i].curValue);
                temporaryShields[i].curValue -= absorbed;
                damage -= absorbed;

                if (temporaryShields[i].curValue <= 0)
                {
                    temporaryShields.Remove(temporaryShields[i]);
                }

                if (damage <= 0) return 0;  // ��� �������� �����Ǿ��� ��� return
            }
        }

        // ���ӽð��� �������� �ʴ� �ǵ忡�� ������ ����(������ �ǵ� ���� ����)
        for (int i = permanentShields.Count - 1; i >= 0; i--)
        {
            if (permanentShields[i].curValue > 0)
            {
                float absorbed = Mathf.Min(damage, permanentShields[i].curValue);
                permanentShields[i].curValue -= absorbed;
                damage -= absorbed;

                if (permanentShields[i].curValue <= 0)
                {
                    permanentShields.Remove(permanentShields[i]);
                }

                if (damage <= 0) return 0;  // ��� �������� �����Ǿ��� ��� return
            }
        }

        UpdateShieldValues();
        // ���� �������� �����Ұ�� ������ return
        return damage;
    }

    public IEnumerator ShieldDelay()
    {
        isAbleCreateShield = false;

        yield return shieldDelayDuration;

        isAbleCreateShield = true;
    }
}
