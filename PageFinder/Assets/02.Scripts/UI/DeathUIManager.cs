using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUIManager : MonoBehaviour
{
    public Canvas Death_Canvas;

    StageManager stageManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    private void Start()
    {
        ChangeDeathCanvasState(false);
    }

    void ChangeDeathCanvasState(bool value)
    {
        Death_Canvas.gameObject.SetActive(value);
    }
    
    // �÷��̾� �״� �����ʿ��� ȣ���ϱ�
    public IEnumerator ActivateDeathUI()
    {
        ChangeDeathCanvasState(true);

        yield return new WaitForSeconds(1);

        ChangeDeathCanvasState(false);
        stageManager.MoveTitleScene();
    }
}
