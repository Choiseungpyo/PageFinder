using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpUIManager : MonoBehaviour
{
    public Canvas Exp_Canvas;
    public Slider Exp_Slid;

    static bool tmp = true; 

    private void Start()
    {
        if(tmp)
        {
            // �� ó������ ����ġ ���� + ���� ������ �Ѿ�� ����ġ�� ���µ��� �ʰ���
            tmp = false;
            ResetExpBar();
        }
    }

    /// <summary>
    /// Exp Bar�� value ���� �����Ѵ�.
    /// </summary>
    public void ResetExpBar()
    {
        Exp_Slid.value = 0;
    }

    /// <summary>
    /// Exp Bar�� ���� �����Ѵ�. 
    /// </summary>
    /// <param name="currentExp">���� ����ġ</param>
    /// <param name="totalExp">�� ����ġ</param>
    public IEnumerator ChangeExpBarValue(float currentExp, float totalExp)
    {
        float barSpeed = 1.5f;
        float goalValue = currentExp / totalExp;

        while(Exp_Slid.value < goalValue)
        {
            Exp_Slid.value += Time.deltaTime * barSpeed;
            yield return null;
        }
        if(currentExp == totalExp)
        {
            yield return new WaitForSeconds(0.2f);
            ResetExpBar();
        }   
    }
}
