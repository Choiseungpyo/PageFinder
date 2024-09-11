using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : SliderBar
{
    /// <summary>
    /// SliderBar�� �ִ� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxShieldValueUI(float maxHp, float currHp, float maxShield)
    {
        // ���� ü�¹� �ٷ� �����ʿ� ��ġ�ϵ��� ����
        bar.transform.position = new Vector3(-1f + 2 * currHp / (maxHp + maxShield), transform.position.y, transform.position.z);

        // shield Bar�� ������ �ִ� �� 1�� ������ �� ä���� �����Ѵ�. 
        bar.maxValue = maxShield;

        bar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100 * maxShield / (maxShield + maxHp));
    }
}
