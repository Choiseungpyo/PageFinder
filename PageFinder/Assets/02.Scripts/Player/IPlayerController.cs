using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// �÷��̾� �⺻ �̵��� ���� �������̽�
public interface IPlayerController
{
    // Move
    void OnMove(InputAction.CallbackContext context);

    // Attack
    void ButtonAttack(InputAction.CallbackContext context);

}
