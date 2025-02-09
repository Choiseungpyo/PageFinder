using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider bar;

    /// <summary>
    /// SliderBar�� �ִ� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxValue)
    {
        bar.maxValue = maxValue;
        //Debug.Log(maxValue);
    }

    /// <summary>
    /// SliderBar�� ���� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float currValue)
    {
        if (currValue > bar.maxValue)
            Debug.LogError($"max:{bar.maxValue}    curr:{currValue}");

        bar.value = currValue;
        //Debug.Log(currValue);
    }
}
