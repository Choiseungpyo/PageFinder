using UnityEngine;

public class TestButton : MonoBehaviour
{
    public void ButtonAction()
    {
        Debug.Log("�ǵ� ���� �̺�Ʈ �߻�");
        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, this, new System.Tuple<float, float>(10f, 10f));
    }
}
