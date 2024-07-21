using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider hpBar;
    // Start is called before the first frame update
    public void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// HP���� �ִ� HP�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="maxHP"></param>
    public void SetMaxHPUI(float maxHP)
    {
        hpBar.maxValue = maxHP;
    }

    /// <summary>
    /// HP���� HP�� �����ϴ� �Լ�
    /// </summary>
    public void SetHPUI(float currHP)
    {
        hpBar.value = currHP;
    }
}
