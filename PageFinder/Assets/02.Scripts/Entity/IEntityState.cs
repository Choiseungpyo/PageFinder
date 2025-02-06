using UnityEngine;

/// <summary>
/// ���� ������Ƽ ���� �ʿ�
/// </summary>
public interface IEntityState
{
    public float MaxHp { get; set; }
    public float CurHp { get; set; }
    public float Atk { get; set; }
    public float CurMoveSpeed { get; set; }
    public float CurAttackSpeed { get; set; }
}
