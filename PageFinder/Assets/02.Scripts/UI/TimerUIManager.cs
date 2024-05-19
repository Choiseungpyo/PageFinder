using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUIManager : MonoBehaviour
{
    public TMP_Text Timer_Txt;

    float total_time = 300;
    int timer_min = 5;
    float timer_sec = 0;

    private void Update()
    {
        CalculateTime();
        SetTimer_Txt();
    }

    /// <summary>
    /// ���ѽð� �ؽ�Ʈ�� �����Ѵ�. 
    /// </summary>
    void SetTimer_Txt()
    {
        Timer_Txt.text = timer_min + ":" + timer_sec;
    }


    /// <summary>
    /// ���ѽð��� ����Ѵ�. 
    /// </summary>
    void CalculateTime()
    {
        if (total_time > 0)
            total_time -= Time.deltaTime;
        timer_min = (int)(total_time / 60f);
        timer_sec = (int)(total_time % 60f);

        //  Debug.Log(timer_min + ":"+ timer_sec);
    }
}