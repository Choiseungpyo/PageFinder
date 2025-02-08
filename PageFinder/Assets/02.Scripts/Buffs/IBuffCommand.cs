using UnityEngine;

public enum BuffType
{
    BuffType_PermanentBuff,
    BuffType_PermanentMultiplier,
    BuffType_PermanentDebuff,
    BuffType_TemporaryBuff,
    BuffType_TemporaryDebuff
}

public abstract class IBuffCommand : ICommand
{
    public BuffType Type { get; set; }
    public float Value { get; set; }
}

// �Ͻ����� ����
public abstract class ITemporaryBuffCommand : IBuffCommand
{
    private float elapsedTime;
    private float duration;

    public float ElapsedTime { get; set; }
    public float Duration { get; set; } // -1: ������ ����

    public abstract void Tick();
    public abstract void EndBuff();
}
