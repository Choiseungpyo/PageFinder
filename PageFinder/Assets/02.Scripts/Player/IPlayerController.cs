using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// �÷��̾� �⺻ �̵��� ���� �������̽�
public interface IPlayerController
{
    // Move
    void OnMove(InputValue value);

    // Attack
    void OnAttack(InputValue value);

}
