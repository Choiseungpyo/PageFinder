using UnityEngine;

public enum BuffType
{
    BuffType_PermanentBuff,
    BuffType_PermanentMultiplier,
    BuffType_PermanentDebuff,
    BuffType_TemporaryBuff,
    BuffType_TemporaryDebuff
}

public interface IBuffCommand : ICommand
{
    public BuffType Type { get; set; }
    public float Value { get; set; }

}

// �Ͻ����� ����
public interface ITemporaryBuffCommand : IBuffCommand
{
    public float Duration { get; set; } // -1: ������ ����

    public void Tick();
}
