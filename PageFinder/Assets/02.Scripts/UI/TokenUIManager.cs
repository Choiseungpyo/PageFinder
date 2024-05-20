using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

public class TokenUIManager : MonoBehaviour
{
    
    public TMP_Text CurrentTokenCnt_Txt;
    public TMP_Text StoragedTokenCnt_Txt;

    // ��ũ��Ʈ ����
    TokenManager tokenManager;

    private void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }

    private void Start()
    {
        SetCurrentTokenCnt_Txt(0);
        SetStoragedTokenCnt_Txt(tokenManager.ReturnTokenCntAboutNextScene());
    }

    /// <summary>
    /// ���� ������ �ִ� ��ū�� ���� �ؽ�Ʈ�� �����Ѵ�.
    /// </summary>
    /// <param name="currentTokenCnt">��ū ����</param>
    public void SetCurrentTokenCnt_Txt(int currentTokenCnt)
    {
        CurrentTokenCnt_Txt.text = currentTokenCnt.ToString();
    }

    /// <summary>
    /// ��Ȧ�� ������ ��ū�� ���� �ؽ�Ʈ�� �����Ѵ�.
    /// </summary>
    /// <param name="storagedTokenCnt"></param>
    public void SetStoragedTokenCnt_Txt(int storagedTokenCnt)
    {
        StoragedTokenCnt_Txt.text = storagedTokenCnt.ToString();
    }
}
