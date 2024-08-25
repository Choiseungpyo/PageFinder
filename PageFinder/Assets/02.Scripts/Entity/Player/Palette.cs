using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class Palette : MonoBehaviour
{
    Color currentColor = Color.green;
    
    // ��ü ���� ����Ʈ�� ������ ���Ҵ� ������ ���� ������ ��ġ�ϵ��� �Ѵ�.
    List<Color> totalColors = new List<Color>() { Color.red, Color.blue, Color.yellow, Color.green}; // ����Ʈ ��� ���� : ���� �� ��� �߰��ϰų� ������ �� �ֱ⿡ 

    // ��ũ��Ʈ ����
    PaletteUIManager paletteManager;

    private void Start()
    {
        paletteManager = GameObject.Find("UIManager").GetComponent<PaletteUIManager>();
    }

    /// <summary>
    /// ���� ������ �����Ѵ�.
    /// </summary>
    /// <param name="color"></param>
    public void ChangeCurrentColor(Color color)
    {
        totalColors.Remove(currentColor);

        if (color.Equals(Color.red))
            currentColor = Color.red;
        else if (color.Equals(Color.green))
            currentColor = Color.green;
        else if (color.Equals(Color.blue))
            currentColor = Color.blue;
        else if (color.Equals(Color.magenta))
            currentColor = Color.magenta;
        else if (color.Equals(Color.yellow))
            currentColor = Color.yellow;
        else if (color.Equals(Color.cyan))
            currentColor = Color.yellow;
        else
        {
            Debug.LogWarning(color);
            currentColor = Color.clear;
        }

        // ���� �������� ������ ����
        totalColors.Add(currentColor);
    }

    /// <summary>
    /// ����� ������ ��´�. 
    /// </summary>
    /// <param name="colorIndex"></param>
    /// <returns></returns>
    public Color GetColorToUse(int colorIndex) // ���� ������ ����, ���� ������ ����, �Ʒ� ������ ����
    {
        if (colorIndex >= totalColors.Count || colorIndex <= -1) // totalColors�� �ε����� �ּ�, �ִ븦 �Ѿ�� ���
        {
            Debug.LogWarning("�ε��� �ʰ�");
            return Color.clear;
        }
       
        return totalColors[colorIndex];
    }

    public int GetTotalColorCount()
    {
        return totalColors.Count;
    }

    /// <summary>
    /// ���� ������ ��´�. 
    /// </summary>
    /// <returns></returns>
    public Color GetCurrentColor()
    {
        return currentColor;
    }

    /// <summary>
    /// ���ο� ������ �߰��Ѵ�.
    /// </summary>
    /// <param name="color"></param>
    public void AddNewColor(Color color)
    {
        // ���� ������ ���� �������� �ֱ� ������ ������ ����
        totalColors.Insert(totalColors.Count-1, color);
        paletteManager.SetPaletteObjects();
    }
}
