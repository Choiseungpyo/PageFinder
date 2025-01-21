using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*// ���� �칰 ��ũ��Ʈ
public class DeepWell : IStatModifier
{
    Player playerScr;

    public void AddDecorator()
    {
        Debug.Log("���� �칰 �߰�");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerScr.InkGainModifiers.Add(this);
    }
    public float ModifyStat(float inkGain)
    {
        return inkGain * 1.04f;
    }
}

// �ʸ��� ��� ��ũ��Ʈ
public class EnergyOfVegetation : IStatModifier
{
    private PlayerScriptController playerScriptControllerScr;
    private Player playerScr;
    public void AddDecorator()
    {
        Debug.Log("�ʸ��� ��� �߰�");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerScr.MaxHpModifiers.Add(this);
    }
    public float ModifyStat(float maxHP)
    {
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerScriptController>(playerScriptControllerScr))
        {
            return (playerScriptControllerScr.GreenScriptCounts * 0.04f + 1) * maxHP; 
            
        }
        // �÷��̾� �� ã�� ��� maxHP�� ��ȯ(��ǻ� ����)
        return maxHP;
    }
}

// ü�� �µ� ��ũ��Ʈ
public class PerceivedTemperature : IStatModifier
{
    private PlayerScriptController playerScriptControllerScr;
    private Player playerScr;

    public void AddDecorator()
    {
        Debug.Log("ü�� �µ� �߰�");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerScr.AtkModifiers.Add(this);

    }
    public float ModifyStat(float atk)
    {
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerScriptController>(playerScriptControllerScr))
        {
            return (playerScriptControllerScr.RedScriptCounts * 0.03f + 1) * atk;
        }
        // �÷��̾� �� ã�� ��� atk�� ��ȯ(��ǻ� ����)
        return atk;
    }
}*/
