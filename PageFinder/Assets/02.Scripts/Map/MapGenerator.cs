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

    private Node newNode;
    private GameObject newObj;
    private Transform newObjTr;

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

    /// <summary>
    /// ��ü���� �� ���� ���μ���
    /// ���� 3�ܰ� ������ ���� ������
    /// </summary>
    public void CreateMap()
    {
        CreateArea();
        CreateNodeInArea();
        //InstantiateArea();
    }

    /// <summary>
    /// �������� �����ϴ� �Լ� 
    /// �������� radius�� ������ ����Ͽ� �����ϴ� ���ٰ����� �׷��� �������� ����
    /// </summary>
    private void CreateArea()
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

            newObj = Instantiate(TestObj, pos, Quaternion.identity);
            newObj.name = "Start";

            int dim = 5;
            Tuple<int, int> startIndex = new Tuple<int, int>((UnityEngine.Random.Range(0, dim)), (UnityEngine.Random.Range(0, dim)));
            Debug.Log($"startIndex: [{startIndex.Item1}, {startIndex.Item2}]");
            newNode = newObj.AddComponent<Node>();
            mapArea.Add(new Area(5, startIndex, newNode, pos));
        }
    }

    private void CreateNodeInArea()
    {
        for(int i = 0; i < mapArea.Count; i++)
        {
            CreateNode(mapArea[i], mapArea[i].StartIndex, roomCount);
        }
    }

    private void CreateNode(Area currArea, Tuple<int, int> index, int roomCount)
    {
        if (roomCount == 0)
            return;
        else
        {
            Tuple<int, int> next;
            while (true) 
            {
                SetDirection();
                next = new Tuple<int, int>(currDir.Item1 + index.Item1, currDir.Item2 + index.Item2);

                if (currArea.CheckNodeInArea(next))
                {
                    if (currArea.GetNodeInArea(next) == null)
                    {
                        // ���� ������ ������ �������� bfs ������
                        int currDirectionLeftAreaCount = currArea.BFS(next);
                        // ���� ���� ������ ���⿡�� bfs�� ������ �� ���� �� �ִ� ���� ������
                        // ������ �ϴ� ���� �������� �۰ų� ������
                        // ������ �����߿��� �ٽ� �����ϱ�
                        if (!(currDirectionLeftAreaCount <= roomCount))
                        {
                            Debug.Log($"currIndex: [{index.Item1}, {index.Item2}] | nextIndex : [{next.Item1}, {next.Item2}]");
                            newObj = Instantiate(TestObj, currArea.GetNodesLocalPosition(next), Quaternion.identity);
                            if (roomCount == 1)
                                newObj.name = "End";
                            newNode = newObj.AddComponent<Node>();
                            currArea.SetNode(next, newNode);
                            currArea.ConnectNode(next, newNode);
                            currArea.AddNode(newNode);
                            break;
                        }
                    }
                }
            }

            CreateNode(currArea, next, --roomCount);
        }
    }

    private int SetDirection()
    {
        int dir = UnityEngine.Random.Range(0, 4);
        currDir = directions[dir];
        return dir;
    }
}
