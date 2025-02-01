using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 10;
    [Tooltip("��� ���� ����")]
    [SerializeField] private float nodeSpacing = 2.0f;
    [SerializeField] private float minOffset = 0.9f;
    [SerializeField] private float maxOffset = 1.1f;

    public enum NodeType { Unknown, Battle_Normal, Battle_Elite, Quest, Treasure, Market, Comma, Boss }

    public class Node
    {
        public int column;
        public int row;
        public Vector2 position;
        public NodeType type;
        public List<Node> Neighbors = new();
        public Node prevNode;
        public GameObject map;

        public Node(int column, int row, Vector2 position, NodeType type, GameObject map)
        {
            this.column = column;
            this.row = row;
            this.position = position;
            this.type = type;
            this.map = map;
        }
    }

    public Node[,] nodes; // �� �����հ� �����Ͽ� �̿� ��带 ��Ż �̵��� Ȱ���� ����
    private Node startNode;
    private List<(Node, Node, float)> edges = new();

    [Header("Appearance Probability")]
    [SerializeField] private float battleNormalProbability = 0.45f;
    [SerializeField] private float battleEliteProbability = 0.15f;
    [SerializeField] private float questProbability = 0.20f;
    [SerializeField] private float treasureProbability = 0.0f;
    [SerializeField] private float marketProbability = 0.10f;
    [SerializeField] private float commaProbability = 0.10f;

    [Header("UI Setting")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject battleNormalPrefab;
    [SerializeField] private GameObject battleElitePrefab;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject marketPrefab;
    [SerializeField] private GameObject commaPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject lineUIPrefab;
    
    private Dictionary<Node, GameObject> nodeUIMap = new(); // ���� UI ����
    private List<GameObject> activeLines = new();
    private Dictionary<NodeType, GameObject> prefabMap;

    [Header("Map Setting")]
    [SerializeField] private GameObject testMap;

    void Start()
    {
        prefabMap = new Dictionary<NodeType, GameObject>
        {
            { NodeType.Battle_Normal, battleNormalPrefab },
            { NodeType.Battle_Elite, battleElitePrefab },
            { NodeType.Quest, questPrefab },
            { NodeType.Treasure, treasurePrefab },
            { NodeType.Market, marketPrefab },
            { NodeType.Comma, commaPrefab },
            { NodeType.Boss, bossPrefab }
        };

        GenerateNodes();
    }

    void GenerateNodes()
    {
        nodes = new Node[columns, rows];

        // 1�� ������ ���� ��� ����
        startNode = new(-1, rows / 2, new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal, testMap);
        CreateNodeUI(startNode);

        // 1~10�� ��� ����
        List<Node> firstColumnNodes = new();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new(x * nodeSpacing, y * nodeSpacing * Random.Range(minOffset, maxOffset));

                Node newNode = new(x, y, position, NodeType.Unknown, testMap);
                nodes[x, y] = newNode;

                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // 10�� ������ ���� ������ ��� ����
        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss, testMap);
        CreateNodeUI(bossNode);

        HandleFirstColumnNodes(startNode, firstColumnNodes);

        ConnectNodes(bossNode);
    }

    NodeType DetermineNodeType(int x, int y)
    {
        if (x == 0) return NodeType.Battle_Normal; // 1��: �Ϲ� ��Ʋ
        if (x == 4) return NodeType.Treasure; // 5��: Ʈ����
        if (x == 9) return NodeType.Comma; // 10��: �޸�
        if (x > 9) return NodeType.Boss; // 10�� ����: ����

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 1) return NodeType.Battle_Normal; // �Ϲ� ��Ʋ ���� ����
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 2) return NodeType.Battle_Elite; // ���� ��Ʋ ���� ����
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        return NodeType.Comma; // �⺻��
    }

    void CreateNodeUI(Node node)
    {
        if (!prefabMap.TryGetValue(node.type, out GameObject selectedPrefab) || selectedPrefab == null)
        {
            Debug.LogWarning($"No prefab found for NodeType: {node.type}");
            return;
        }

        // ����� ��ġ�� Screen Space�� ��ȯ�Ͽ� UI ��ġ
        Vector3 worldPosition = new(node.position.x, node.position.y, 0f);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        GameObject uiElement = Instantiate(selectedPrefab, canvas.transform);

        uiElement.GetComponent<RectTransform>().position = screenPosition;
        nodeUIMap[node] = uiElement;
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new();

        // ù ��° ���� Ȱ�� ��� �߰�
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null) activeNodes.Add(nodes[0, y]);
        }

        // ��(column) �������� ��� ����
        for (int x = 0; x < columns - 1; x++)
        {
            HashSet<Node> nextActiveNodes = new();
            bool[] hasTwoNeighbors = { false, false };

            // �� ����� �̿� ��� ����
            foreach (Node currentNode in activeNodes)
            {
                List<Node> neighborCandidates = new();
                int currentY = currentNode.row; // ����� ��(row) ��������

                // ���� ��(x+1)���� �̿� �ĺ� Ž��
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int nextY = currentY + offsetY;
                    if (nextY >= 0 && nextY < rows && nodes[x + 1, nextY] != null)
                    {
                        neighborCandidates.Add(nodes[x + 1, nextY]);
                    }
                }

                currentNode.type = DetermineNodeType(x, currentY);
                CreateNodeUI(currentNode);

                // ������ �̿� ����
                while (neighborCandidates.Count > 0)
                {
                    // �̿� �ĺ� ���� ����
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    // ��� �߰� (�Ÿ��� ���� ���� ����)
                    float distance = Vector2.Distance(currentNode.position, nextNode.position);
                    distance *= Random.Range(minOffset, maxOffset);

                    // �̿� �ĺ� ����
                    currentNode.Neighbors.Add(nextNode);
                    nextNode.prevNode = currentNode;
                    edges.Add((currentNode, nextNode, distance));
                    nextActiveNodes.Add(nextNode);

                    if (Random.value < 0.5f &&
                            !(x >= 2 && currentNode.prevNode.prevNode.Neighbors.Count == 1 && currentNode.prevNode.Neighbors.Count == 1)) // ���� ����
                    {
                        Debug.Log($"Single: {currentNode.column},{currentNode.row}");
                        break;
                    }
                    else
                    {
                        if (currentNode.Neighbors.Count == 2) break; // ����� �̿� ��尡 2���� ���
                        else if (System.Array.Exists(hasTwoNeighbors, n => !n)) // ���� ������ ������ ���
                        {
                            if (!hasTwoNeighbors[0]) hasTwoNeighbors[0] = true;
                            else hasTwoNeighbors[1] = true;
                            Debug.Log($"Double: {currentNode.column},{currentNode.row}");
                            continue;
                        }
                        else if (x >= 2 && currentNode.prevNode.prevNode.Neighbors.Count == 1 && currentNode.prevNode.Neighbors.Count == 1) Debug.Log($"Exception: {currentNode.column},{currentNode.row}");
                        else break;
                    }
                }
            }

            // ���� ���� Ȱ�� ��常 ����
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x + 1, y];
                if (node != null && !nextActiveNodes.Contains(node))
                {
                    nodes[x + 1, y] = null;

                    if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
                    {
                        Destroy(uiElement);
                        nodeUIMap.Remove(node);
                    }
                }
            }

            activeNodes = nextActiveNodes;
        }

        HandleFinalBossNode(bossNode);
    }

    void OnDrawGizmos()
    {
        if (nodes == null || edges == null) return;

        // ��� �׸���
        Gizmos.color = Color.green;
        for (int i = 0; i < edges.Count; i++)
        {
            Gizmos.DrawLine(edges[i].Item1.position, edges[i].Item2.position);
        }

        // ��� �׸���
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x, y];
                if (node != null)
                {
                    Gizmos.color = GetNodeColor(node.type);
                    Gizmos.DrawSphere(node.position, 0.2f);
                }
            }
        }

        // ���� ���� ���� ��� ǥ��
        Node startNode = new(-1, -1, new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal, testMap);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startNode.position, 0.3f);

        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss, testMap);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(bossNode.position, 0.3f);
    }

    Color GetNodeColor(NodeType type)
    {
        switch (type)
        {
            case NodeType.Battle_Normal: return Color.red;
            case NodeType.Battle_Elite: return Color.magenta;
            case NodeType.Quest: return Color.blue;
            case NodeType.Treasure: return Color.yellow;
            case NodeType.Market: return Color.cyan;
            case NodeType.Comma: return Color.green;
            case NodeType.Boss: return Color.black;
            default: return Color.white;
        }
    }

    void HandleFirstColumnNodes(Node startNode, List<Node> firstColumnNodes)
    {
        // 1������ �������� �ִ� 2���� ��带 ����
        while (startNode.Neighbors.Count < 2 && firstColumnNodes.Count > 0)
        {
            Node selectedNode = firstColumnNodes[Random.Range(0, firstColumnNodes.Count)];
            firstColumnNodes.Remove(selectedNode);

            // ���� �߰�
            float distance = Vector2.Distance(startNode.position, selectedNode.position);
            startNode.Neighbors.Add(selectedNode);
            edges.Add((startNode, selectedNode, distance));
        }

        // 1������ ���õ��� ���� ��� ����
        foreach (Node node in firstColumnNodes)
        {
            if (nodeUIMap.TryGetValue(node, out GameObject uiElement))
            {
                Destroy(uiElement);
                nodeUIMap.Remove(node);
            }

            for (int y = 0; y < rows; y++)
            {
                if (nodes[0, y] == node) // ��Ȯ�� ��Ī�Ǵ� ��常 ����
                {
                    nodes[0, y] = null;
                    Debug.Log($"Removed unselected node at position: {node.position}");
                    break;
                }
            }
        }
    }

    void HandleFinalBossNode(Node bossNode)
    {
        // 10���� �����ϴ� ��带 ������ ���� ����
        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[columns - 1, y];
            if (node is not null)
            {
                node.type = NodeType.Comma;
                CreateNodeUI(node);
                float distance = Vector2.Distance(bossNode.position, node.position);
                node.Neighbors.Add(bossNode);
                edges.Add((node, bossNode, distance));
            }
        }

        DrawPaths();

        if (!IsGraphConnected())
        {
            Debug.LogError("�׷����� ����Ǿ� ���� �ʽ��ϴ�!");
        }
    }

    bool IsGraphConnected()
    {
        HashSet<Node> visited = new();
        Stack<Node> stack = new();

        if (startNode == null)
        {
            Debug.LogError("No active nodes found. Cannot check connectivity.");
            return false;
        }

        // DFS Ž��
        stack.Push(startNode);
        visited.Add(startNode);
        
        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();

            foreach (Node neighbor in currentNode.Neighbors)
            {
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
        }

        // Ȱ��ȭ�� ��� ��� �湮 ���� ��Ȯ��
        foreach (Node node in nodes.Cast<Node>().Where(n => n != null))
        {
            if (!visited.Contains(node))
            {
                Debug.LogError($"Node at position {node.position} is not connected.");
                return false;
            }
        }

        return true;
    }

    void DrawPaths()
    {
        foreach (var edge in edges)
        {
            Node nodeA = edge.Item1;
            Node nodeB = edge.Item2;

            // ���� ��ǥ �� ��ũ�� ��ǥ ��ȯ
            Vector3 screenPositionA = mainCamera.WorldToScreenPoint(new Vector3(nodeA.position.x + 0.5f, nodeA.position.y, 0f));
            Vector3 screenPositionB = mainCamera.WorldToScreenPoint(new Vector3(nodeB.position.x - 0.5f, nodeB.position.y, 0f));

            CreateLineUI(screenPositionA, screenPositionB);
        }
    }

    void CreateLineUI(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(lineUIPrefab, canvas.transform);
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();

        // ���� �߽� ��ġ ����
        rectTransform.position = (start + end) / 2;

        // ���� ���� ����
        float distance = Vector3.Distance(start, end);
        rectTransform.sizeDelta = new Vector2(distance, 5f);  // �β� 5, ���̴� �� ��� ���� �Ÿ��� ����

        // ���� ȸ�� ����
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        activeLines.Add(lineObject);  // ���߿� ������ �� �ֵ��� ����
    }
}