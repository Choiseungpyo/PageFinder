using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : SliderBar
{
    /// <summary>
    /// SliderBar�� �ִ� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxShieldValueUI(float maxHp, float currHp, float maxShield)
    {
        if (bar == null)
            bar = GetComponent<Slider>();

        bar.maxValue = maxShield;

        // ���� ü�¹� �ٷ� �����ʿ� ��ġ�ϵ��� ����
        bar.transform.localPosition = new Vector3(-1f + 2 * currHp / (maxHp + maxShield), transform.localPosition.y, transform.localPosition.z);
        bar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100 * maxShield / (maxShield + maxHp));
    }
}
