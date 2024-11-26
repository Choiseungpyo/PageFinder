using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DiaryElement : MonoBehaviour
{
    protected ScriptData scriptData;
    [SerializeField]
    protected GameObject scriptDescriptionObject;
    [SerializeField]
    protected Image backgroundImage;

    protected Image[] scriptDescriptionImages;
    protected TMP_Text[] scriptDescriptionTexts;
    protected Toggle toggle;
    [SerializeField]
    protected Image icon;

    [SerializeField]
    protected Sprite[] backGroundImages;
    public virtual ScriptData ScriptData { 
        get => scriptData; 
        set{
            scriptData = value;
            if(value == null)
            {
                toggle.interactable = false;
            }
            else
            {
                toggle.interactable = true;
                SetScriptPanels();
            }
        }  
    }

    // Start is called before the first frame update
    public virtual void Awake()
    {
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(this.gameObject, "Toggle");
    }

    protected void OnEnable()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            if (scriptData == null) toggle.interactable = false;
            else toggle.interactable = true;
        }
    }

    protected void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            // ���� ��ũ��Ʈ �� ���� ������Ʈ ��Ȱ��ȭ
            scriptDescriptionObject.SetActive(false);
            // ��ü ����� ���� �ȵ� ������� ����
            backgroundImage.sprite = backGroundImages[0];
        }
    }

    public virtual void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            if (scriptData == null) return;
            scriptDescriptionObject.SetActive(true);
            backgroundImage.sprite = backGroundImages[1];
            SetScriptDescription();

        }
        else
        {
            if (scriptData == null) return;

            backgroundImage.sprite = backGroundImages[0];
            scriptDescriptionObject.SetActive(false);
        }
    }


    public virtual void SetScriptPanels()
    {
        icon.sprite = scriptData.scriptIcon;
    }

    public virtual void SetScriptDescription()
    {
        if(!DebugUtils.CheckIsNullWithErrorLogging<ScriptData>(scriptData))
        {
            scriptDescriptionImages = scriptDescriptionObject.GetComponentsInChildren<Image>();
            scriptDescriptionImages[0].sprite = scriptData.scriptBG;
            scriptDescriptionImages[1].sprite = scriptData.scriptIcon;

            scriptDescriptionTexts = scriptDescriptionObject.GetComponentsInChildren<TMP_Text>();
            scriptDescriptionTexts[0].text = scriptData.scriptName;
            string tempText = null;
            switch (scriptData.scriptType)
            {
                case ScriptData.ScriptType.BASICATTACK:
                    tempText = "�⺻����";
                    break;
                case ScriptData.ScriptType.DASH:
                    tempText = "��ũ���";
                    break;
                case ScriptData.ScriptType.SKILL:
                    tempText = "��ũ��ų";
                    break;
                case ScriptData.ScriptType.PASSIVE:
                    tempText = "�нú�";
                    break;
                case ScriptData.ScriptType.MAGIC:
                    tempText = "��ũ����";
                    break;
            }
            scriptDescriptionTexts[1].text = tempText;
            /*        if(level == - 1) {
                        tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[0] * 100}%</color>");
                    }
                    else
                    {
                        tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[level] * 100}%</color>");
                    }*/
            tempText = scriptData.scriptDesc.Replace("LevelData%", $"<color=red>{scriptData.percentages[1] * 100}%</color>");
            scriptDescriptionTexts[2].text = tempText;
        }
        
    }
}
