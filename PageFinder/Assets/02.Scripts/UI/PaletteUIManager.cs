using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PaletteUIManager : MonoBehaviour
{
    public Canvas PaletteCanvas;
    public GameObject PaletteLine_Prefab;
    public GameObject[] PaletteColors_Prefab = new GameObject[5];
    // ���� �� ���� ���� ��ä�� ��� �̹��� �ʿ�

    public int colorCnt;
    List<GameObject> PaletteLines = new List<GameObject>();
    List<GameObject> PaletteColors = new List<GameObject>();

    Palette palette;
    private void Start()
    {
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();
        SetPaletteObjects();
        ChangePaletteObjectsActiveState(false);
    }

    /// <summary>
    /// �ȷ�Ʈ ������ ���� �ȷ�Ʈ �� ����ŭ �����Ѵ�.
    /// </summary>
    public void SetPaletteObjects()
    {
        Transform tr = PaletteCanvas.transform.GetChild(0);
        Vector3 standardPos = PaletteCanvas.transform.GetChild(0).transform.position;
        int totalColorCount = palette.GetTotalColorCount(); //

        PaletteLines.Clear();
        PaletteColors.Clear();

        // �����ϰ� �ִ� �ȷ�Ʈ �� ����ŭ ������ �����Ѵ�. 
        for (int i = 0; i < totalColorCount; i++) 
        {
            // �ð� �������� ����

            // �ȷ�Ʈ ����
            PaletteLines.Add(Instantiate(PaletteLine_Prefab,
                                        standardPos,
                                        Quaternion.Euler(new Vector3(0, 0, -(360 / totalColorCount) * i)), 
                                        tr.GetChild(1).transform)); 

            // �ȷ�Ʈ ����
            PaletteColors.Add(Instantiate(PaletteColors_Prefab[totalColorCount-2],
                                        standardPos,
                                        Quaternion.Euler(new Vector3(0, 0, -(360 / totalColorCount) * i)), 
                                        tr.GetChild(0).transform));

            PaletteColors[i].GetComponent<Image>().color = palette.GetColorToUse(i);
        }
    }

    /// <summary>
    /// �ȷ�Ʈ ���� ������Ʈ���� Ȱ��ȭ ���θ� �����Ѵ�.
    /// </summary>
    /// <param name="value"></param>
    public void ChangePaletteObjectsActiveState(bool value)
    {
        for (int i = 0; i < PaletteLines.Count; i++)
        {
            PaletteLines[i].SetActive(value);
            PaletteColors[i].SetActive(value);
        }
    }


    public void ChangeCurrentColor(double rot)
    {
        int totalColorCount = palette.GetTotalColorCount();
        Color colorToChange;

        for (int i=0; i< totalColorCount; i++)
        {
            // ���� ������(�ð���� ����)�� ���ϴ� ������ ���
            if (i == totalColorCount - 1 || rot <= (360 / totalColorCount) * (i + 1))
            {
                colorToChange = PaletteColors[i].GetComponent<Image>().color;
                palette.ChangeCurrentColor(colorToChange);

                if (colorToChange == Color.red)
                    Debug.Log("Red");
                else if (colorToChange == Color.green)
                    Debug.Log("Green");
                else if (colorToChange == Color.blue)
                    Debug.Log("Blue");
                else if (colorToChange == Color.magenta)
                    Debug.Log("Magenta");
                else if (colorToChange == Color.yellow)
                    Debug.Log("Yellow");
                else if (colorToChange == Color.cyan)
                    Debug.Log("Cyan");

                break;
            }
        }
    }

}
