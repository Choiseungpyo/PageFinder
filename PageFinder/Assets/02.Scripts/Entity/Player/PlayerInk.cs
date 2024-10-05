using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INKTYPE
{
    LINE,
    CIRCLE
}
public class PlayerInk : MonoBehaviour
{
    public GameObject lineObj;
    
    //TODO
    // ������Ʈ Ǯ�� ���� �ʿ�.
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO
    // ������Ʈ Ǯ�� Ȱ���� �ڵ�� ���� �ʿ�
    public Transform CreateInk(INKTYPE inkType, Vector3 createPos)
    {
        GameObject inkObj = null;
        switch (inkType) 
        {
            case INKTYPE.LINE:
                inkObj = Instantiate(lineObj, createPos, Quaternion.identity);
                break;
            case INKTYPE.CIRCLE:
                
                break;
        }

        if(inkObj == null)
        {
            Debug.LogError("InkObj is null");
            return null;
        }
        return inkObj.GetComponent<Transform>();
    }
}
