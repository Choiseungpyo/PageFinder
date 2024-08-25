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
    }

    /// <summary>
    /// SliderBar�� ���� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float currValue)
    {
        bar.value = currValue;
    }
}
