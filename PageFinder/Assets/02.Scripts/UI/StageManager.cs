using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{
    int currentStage = 1;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void ChangeCurrentStage(int value)
    {
        currentStage = value;
    }

    // ��� �� ���������� �Ѿ���� ����
    public void MoveNextStage()
    {
        int nextStage = currentStage + 1;
        ChangeCurrentStage(nextStage);
        SceneManager.LoadScene(nextStage.ToString());
    }

    public void MoveTitleScene()
    {
        SceneManager.LoadScene("Title");
    }
}
