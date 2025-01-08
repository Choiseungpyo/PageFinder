using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum INKMARKTYPE
{
    BASICATTACK,
    DASH,
    INKSKILL,
    INTERACTIVEOBJECT
}

/// <summary>
/// ��ũ ��ũ�� �����ϴ� Ŭ����
/// ��ũ ��ũ �������� ������ ���� ������.
/// ��ũ ��ũ�� �ǵ����� ���� �� �� ������Ʈ�� �ش��ϴ� Ÿ���� ������Ʈ�� �ƴ� �� ����
/// ex. ��ũ �뽬�� ��� �뽬 ��ũ �������� ������ �ڿ� ������Ʈ Ǯ����ȯ
/// -> �׷��� �ش� �������� ��ũ �뽬�� ������ ��� ����
/// -> ���� ��ũ ��ų�� ����� ��ũ ��ũ�� �ʿ��� �� ���� ������Ʈ�� ��ũ �뽬 ��ũ�� ������ȭ �ߴ� ��ü���
/// �ʱ�ȭ�� �ʿ���
/// ��������� Pool�� ����ϴ°� �̵��ϱ�??
/// 
/// ->���� ��ũ��ũ ���Ϳ��� ���� �����Ѵٰ� ġ��, ��ũ ��ũ ������(Ȱ��ȭ�ҽ�)
/// �����Ϳ� ���� ��ũ ��ũ�� �����ϰ�, �ռ��ÿ��� ��ũ ���Ϳ��� �����͸� �ҷ��ͼ� �ؾ��ϳ�..?
/// ����ؾߵ� �κ�.
/// </summary>
public class InkMarkSetter : MonoBehaviour
{
    public InkMarkData[] inkMarksDatas; // 0: BA, 1: Dash, 2: Skill, 3: InteractiveObject

    public void SetInkMarkScale(INKMARKTYPE inkType, Transform inkMarkTransform)
    {
        switch (inkType)
        {
            case INKMARKTYPE.BASICATTACK:
                inkMarkTransform.localScale = inkMarksDatas[0].scale;
                break;
            case INKMARKTYPE.DASH:
                inkMarkTransform.localScale = inkMarksDatas[1].scale;
                break;
            case INKMARKTYPE.INKSKILL:
                inkMarkTransform.localScale = inkMarksDatas[2].scale;
                break;
            case INKMARKTYPE.INTERACTIVEOBJECT:
                inkMarkTransform.localScale = inkMarksDatas[3].scale;
                break;
        }
    }

    public void SetInkMarkSprite(InkType inkType, Sprite inkMarkSprite)
    {
        switch (inkType)
        {
            case InkType.RED:
                break;
            case InkType.GREEN:
                break;
            case InkType.BLUE:
                break;
        }
    }
}
