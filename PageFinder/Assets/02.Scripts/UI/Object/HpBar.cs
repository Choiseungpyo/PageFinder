using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : SliderBar
{
    [SerializeField]
    protected Slider shieldBar;

    // Hp Bar ���� ���

    // 
    /* ��ü Bar = ���� Hp + ���差 
     * 
     * Max Hp�� �״�� ����, ���� Hp�κ��� �߰��Ǵ� �ǵ差 ��ŭ �����ʺ��� ä���
     *
     * currHp + maxShieldValue > Max Hp : ������ ������ ���� �������� ���� ä���
     * currHp + maxShieldValue <= Max Hp : curr Hp ������ ������ ������ �������� ���� ä���
     * 
     * entity Ŭ�������� ���差�� �����ؾ� ��
     * maxShieldValue
     * currShieldValue
     * 
     * bar.maxValue�� bar.currValue ���� �̿��Ͽ� shieldBar ũ��� ��ġ�� �����Ͽ� ���带 �������� 
     */

    /// <summary>
    /// SliderBar�� �ִ� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxHp, float currHp, float maxShieldValue, float currShieldValue)
    {
        //SetShieldBarData(maxHp, currHp, maxShieldValue, currShieldValue);
        bar.maxValue = maxHp;
    }

    /// <summary>
    /// SliderBar�� ���� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float maxHp, float currHp, float maxShield, float currShield)
    {
        SetShieldBarData(maxHp, currHp, maxShield, currShield);
        bar.value = currHp;
    }


    private void SetShieldBarData(float maxHp, float currHp, float maxShield, float currShield)
    {
        // �� ä������ ���� ���ϱ� 

        if(maxShield + currHp > maxHp) // Hp ���� ���κп������� ä�����ϴ� ���
        {
            shieldBar.direction = Slider.Direction.LeftToRight;
            shieldBar.transform.position = new Vector3(1, shieldBar.transform.position.y, 0);
            shieldBar.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);

            Debug.Log("currHp + ���� ���� �ִ밪 ���� ŭ");
        }
        else
        {
            shieldBar.direction = Slider.Direction.RightToLeft;
            shieldBar.transform.position = new Vector3(-1f + 2 * currHp/maxHp, transform.position.y, transform.position.z);
            shieldBar.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            Debug.Log("currHp + ���� ���� �ִ밪 ���� ����");
        }


        // shield Bar�� ������ �ִ� �� 1�� ������ �� ä���� �����Ѵ�. 

        shieldBar.maxValue = maxShield;
        shieldBar.value = currShield;
        shieldBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100 * maxShield / maxHp);
    }
}
