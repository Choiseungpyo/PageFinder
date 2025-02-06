using UnityEngine;

public class Portal : MonoBehaviour
{
    private Vector3 targetPosition;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // �÷��̾ ��Ż�� �������� ���� �̵�
        {
            Debug.Log($"��Ż�� ���� {targetPosition}���� �̵��մϴ�.");
            other.transform.position = targetPosition;
        }
    }
}