using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public GameObject mapObj;
    public GameObject TestObj;

    [Range(200, 300)]
    public int radius;
    [Range(1, 6)]
    public int placeCount;
    [Range(1, 20)]
    public int roomCount;

    // areaLength�� ¦��������.
    [Range(100,300)]
    public int areaLength;


    private GameObject[] pivots;
    private int areaOffset;


    private enum MapType
    {
        MiddleBoss,
        Token,
        Object
    }

    private Tuple<int, int>[] directions;
    private Tuple<int, int> currDir;
    private List<Area> mapArea;

    private MapType currMapType;

    // Start is called before the first frame update
    void Start()
    {
        pivots = new GameObject[placeCount];
        mapArea = new List<Area>();
        areaOffset = 50;
        directions = new Tuple<int, int>[4]{
            new Tuple<int, int>(0, -1), // left
            new Tuple<int, int>(0, 1), // right
            new Tuple<int, int>(1, 0), // up
            new Tuple<int, int>(-1, 0)  // down
        };

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
        InstantiateArea();
    }

    /// <summary>
    /// �������� �����ϴ� �Լ� 
    /// �������� radius�� ������ ����Ͽ� �����ϴ� ���ٰ����� �׷��� �������� ����
    /// </summary>
    private void CreateStartPos()
    {
        // ���Ȱ����� ��ȯ
        float angleInRandian = 2.0f * MathF.PI / placeCount;

        for(int i = 0; i < placeCount; i++)
        {
            // ���ٰ����� ������ ����
            Vector3 pos = new Vector3(
                (int)(MathF.Cos(angleInRandian * i) * radius),
                0,
                (int)(MathF.Sin(angleInRandian * i) * radius)
                );

            // ���� �ʿ�(�׻� �߾��� �������� �ε����� �����ؾ� ������
            // ���� �׷��� �ʰ� ����
            Node startNode = new Node(pos);
            int dim = 5;
            Tuple<int, int> startIndex = new Tuple<int, int>((UnityEngine.Random.Range(0, dim)), (UnityEngine.Random.Range(0, dim)));
            mapArea.Add(new Area(5, startIndex, startNode));
        }
    }

    private void CreateAreaMap()
    {
        for(int i = 0; i < mapArea.Count; i++)
        {
            CreateArea(mapArea[i], mapArea[i].StartIndex, roomCount);
        }

    }

    /*
     �� ���⿡�� bfs �ǽ�
    bfs�� �ǽ����� �� ���� ī��Ʈ�� �����ؾ� �Ǵ� roomCount ���ϸ� �ٸ� �������� ���� �����ϱ�
    
     */
    private void CreateArea(Area currArea, Tuple<int, int> index, int roomCount)
    {
        if (roomCount == 0)
            return;
        else
        {
            SetDirection();
            Tuple<int, int> next = new Tuple<int, int>(currDir.Item1 + index.Item1, currDir.Item2 + index.Item2);
            // ���ѷ����� ������ ��� ����
            // ��� ���� ���θ��� �� �� ���� ���� -> ��� �ذ��ؾ� �� ���ΰ�? -> �θ𿡼� ����ϱ�?
            while (true) 
            {
                if (currArea.CheckNodeInArea(next))
                {
                    if (currArea.GetNodeInArea(next) == null)
                    {
                        Node newNode = new Node(SetPosition(currArea.GetNodeInArea(index).Pos));
                        currArea.SetArea(next, newNode);
                        currArea.AddNode(newNode);
                        break;
                    }
                }
                 SetDirection();
                 next = new Tuple<int, int>(currDir.Item1 + index.Item1, currDir.Item2 + index.Item2);
            }

            CreateArea(currArea, next, --roomCount);
        }
    }

    private void SetDirection()
    {
        currDir = directions[UnityEngine.Random.Range(0, 4)];
    }

    

    private Vector3 SetPosition(Vector3 position)
    {
        Vector3 pos;
        if (currDir == directions[0])    // == left
            pos = position + new Vector3(-areaOffset, 0, 0);
        else if (currDir == directions[1]) // == right
            pos = position + new Vector3(areaOffset, 0, 0);
        else if (currDir == directions[2]) // == up
            pos = position + new Vector3(0, 0, areaOffset);
        else // == down
            pos = position + new Vector3(0, 0, -areaOffset);

        return pos;
    }

    private void InstantiateArea()
    {
        for(int i = 0; i < mapArea.Count; i++)
        {
            LinkedList<Node> list = mapArea[i].GetMapList();
            int index = 0;
            LinkedListNode<Node> node = list.First;
            while (index < list.Count)
            {
                GameObject obj = Instantiate(TestObj, node.Value.Pos, Quaternion.identity);
                obj.name = (index + 1).ToString();
                index++;
                node = node.Next;
            }
        }
    }
}
