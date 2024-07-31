using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public GameObject mapObj;
    public GameObject TestObj;

    [Range(10, 40)]
    public int radius;
    [Range(1, 7)]
    public int placeCount;
    [Range(1, 10)]
    public int startPosRandomVal;
    [Range(1, 6)]
    public int roomCount;

    private int sideLength;
    private int StartPosWidth;
    private int StartPosHeight;

    private Vector3 topLeft;
    private Vector3 topRight;
    private Vector3 bottomLeft;
    private Vector3 bottomRight;

    private LineRenderer lineRenderer;

    private enum CoordinateCondition
    {
        PositiveXZ,             // x > StartPosWidth, z > StartPosHeight
        PositiveXNegativeZ,     // x > StartPosWidth, z < StartPosHeight
        NegativeXZ,             // x < StartPosWidth, z < StartPosHeight
        NegativeXPositiveZ,     // x < StartPosWidth, z > StartPosHeight
        ZeroXPositiveZ,         // x = StartPosWidth, z > StartPosHeight
        ZeroXNegativeZ,         // x = StartPosWidth, z < StartPosHeight
        PositiveXZeroZ,         // x > StartPosWidth, z = StartPosHeight
        NegativeXZeroZ          // x < StartPosWidth, z = StartPosHeight
    }

    private CoordinateCondition posCondition;
    private List<List<Vector3>> mapPos;

    // Start is called before the first frame update
    void Start()
    {
        mapPos = new List<List<Vector3>>();
        StartPosWidth = 10;
        StartPosHeight = 10;
        sideLength = 100;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .5f;
        lineRenderer.endWidth = .5f;
        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMap()
    {
        CreateStartPos();
        CreateAreaMap();
    }

    /// <summary>
    /// �������� �����ϴ� �Լ� 
    /// �������� radius�� ������ ����Ͽ� �����ϴ� ���ٰ����� �׷��� �������� ����
    /// </summary>
    private void CreateStartPos()
    {
        // ���Ȱ����� ��ȯ
        float angleInRandian = 2.0f * MathF.PI / placeCount;
        // ���� ���� ����
        float randomAngle = UnityEngine.Random.Range(0f, 360f);

        for(int i = 0; i < placeCount; i++)
        {
            // ���ٰ����� ������ ����
            Vector3 pos = new Vector3(
                (int)(MathF.Cos(angleInRandian * i + randomAngle) * radius),
                0,
                (int)(MathF.Sin(angleInRandian * i + randomAngle) * radius)
                );

            // �������� �������� +- ������ġ�� �������� �ű��
            pos += new Vector3(
                UnityEngine.Random.Range(-startPosRandomVal, startPosRandomVal),
                0,
                UnityEngine.Random.Range(-startPosRandomVal, startPosRandomVal)
                );

            //pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z);

            mapPos.Add(new List<Vector3>());
            mapPos[i].Add(pos);
            Instantiate(TestObj, pos, Quaternion.identity);
        }
    }

    private void CreateAreaMap()
    {
        for(int i = 0; i < mapPos.Count; i++)
        {
            foreach(Vector3 position in mapPos[i])
            {
                lineRenderer.positionCount = 5;
                DefinePosCondition(position);
                DefineVirtualSqaure(position);
            }
        }

    }

    private void DefineVirtualSqaure(Vector3 position)
    {
        switch (posCondition)
        {
            case CoordinateCondition.PositiveXZ:
                CreateSqaure(position, new Vector3(sideLength, 0, sideLength));
                break;
            case CoordinateCondition.PositiveXNegativeZ:
                //CreateSqaure(position, new Vector3(sideLength, 0, -sideLength));
                break;
            case CoordinateCondition.NegativeXPositiveZ:
                //CreateSqaure(position, new Vector3(-sideLength, 0, sideLength));
                break;
            case CoordinateCondition.NegativeXZ:
                //CreateSqaure(position, new Vector3(-sideLength, 0, sideLength));
                break;
        }
    }

    private void CreateSqaure(Vector3 startPos, Vector3 offset)
    {
        topLeft = startPos + new Vector3(0, 0, offset.z);
        topRight = startPos + offset;
        bottomLeft = startPos;
        bottomRight = startPos + new Vector3(offset.x, 0, 0);
        lineRenderer.SetPosition(0, topLeft);
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, bottomRight);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, topLeft);
    }

    // ������ ��ǥ ���� �����ϱ�
    private void DefinePosCondition(Vector3 pos)
    {
        if (pos.x > StartPosWidth && pos.z > StartPosHeight) posCondition = CoordinateCondition.PositiveXZ;
        else if (pos.x > StartPosWidth && pos.z < -StartPosHeight) posCondition = CoordinateCondition.PositiveXNegativeZ;
        else if (pos.x < -StartPosWidth && pos.z > StartPosHeight) posCondition = CoordinateCondition.NegativeXPositiveZ;
        else if (pos.x < -StartPosWidth && pos.z < -StartPosHeight) posCondition = CoordinateCondition.NegativeXZ;
        else if (pos.x <= StartPosWidth && pos.x >= -StartPosWidth && pos.z > StartPosHeight) posCondition = CoordinateCondition.ZeroXPositiveZ;
        else if (pos.x <= StartPosWidth && pos.x >= -StartPosWidth && pos.z < -StartPosHeight) posCondition = CoordinateCondition.ZeroXNegativeZ;
        else if (pos.x > StartPosWidth && pos.z <= StartPosHeight && pos.z >= -StartPosHeight) posCondition = CoordinateCondition.PositiveXZeroZ;
        else if (pos.x < -StartPosWidth && pos.z <= StartPosHeight && pos.z >= -StartPosHeight) posCondition = CoordinateCondition.NegativeXZeroZ;
    }
}
