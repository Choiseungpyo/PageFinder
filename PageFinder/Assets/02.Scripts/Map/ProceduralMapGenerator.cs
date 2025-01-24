using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 10;
    [SerializeField] private float nodeSpacing = 2.0f; // �Ÿ� ����

    public enum NodeType { Battle_Normal, Battle_Elite, Quest, Treasure, Market, Comma, Boss }

    public class Node
    {
        public Vector2 Position;
        public NodeType Type;
        public List<Node> Neighbors = new List<Node>();

        public Node(Vector2 position, NodeType type)
        {
            Position = position;
            Type = type;
        }
    }

    private Node[,] nodes;
    private List<(Node, Node, float)> edges = new List<(Node, Node, float)>(); // ��� �� �Ÿ� ����

    [Header("Appearance Probability")]
    [SerializeField] private float battleNormalProbability = 0.45f;
    [SerializeField] private float battleEliteProbability = 0.15f;
    [SerializeField] private float questProbability = 0.20f;
    [SerializeField] private float treasureProbability = 0.0f;
    [SerializeField] private float marketProbability = 0.10f;
    [SerializeField] private float commaProbability = 0.10f;

    void Start()
    {
        GenerateNodes();
    }

    void GenerateNodes()
    {
        nodes = new Node[columns, rows];

        // 1�� ������ ���� ��� ����
        Node startNode = new Node(new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal);
        List<Node> firstColumnNodes = new List<Node>();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new Vector2(x * nodeSpacing, y * nodeSpacing);
                NodeType type = DetermineNodeType(x, y);

                Node newNode = new Node(position, type);
                nodes[x, y] = newNode;

                // 1�� ��� �߰�
                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // 10�� ������ ���� ������ ��� ����
        Node bossNode = new Node(new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss);

        // 1���� 10�� ó��
        HandleFirstColumnNodes(startNode, firstColumnNodes);
        Debug.Log("��� ���� �Ϸ�");

        ConnectNodes(bossNode);
    }

    NodeType DetermineNodeType(int x, int y)
    {
        if (x == 0) return NodeType.Battle_Normal; // ù ���� �Ϲ� ��Ʋ
        if (x == 4) return NodeType.Treasure; // 5���� Ʈ����
        if (x == 9) return NodeType.Comma; // 10���� �޸�
        if (x > 9) return NodeType.Boss; // 10�� ���Ĵ� ����

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 4) return NodeType.Battle_Normal; // �Ϲ� ��Ʋ ����
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 6) return NodeType.Battle_Elite; // ���� ��Ʋ ����
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        return NodeType.Comma; // �⺻��
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new HashSet<Node>();

        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null)
            {
                activeNodes.Add(nodes[0, y]);
            }
        }

        for (int x = 0; x < columns - 1; x++) // ������ �� ����
        {
            HashSet<Node> nextActiveNodes = new HashSet<Node>();
            foreach (Node currentNode in activeNodes)
            {
                List<Node> possibleNeighbors = new List<Node>();

                // ���� ���� ������ ��� ����
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int nextY = (int)(currentNode.Position.y / nodeSpacing) + offsetY;
                    if (nextY >= 0 && nextY < rows && nodes[x + 1, nextY] != null)
                    {
                        possibleNeighbors.Add(nodes[x + 1, nextY]);
                    }
                }

                // �������� �ִ� 2���� �̿� ��� ����
                int maxConnections = Random.Range(1, 3); // 1 �Ǵ� 2
                while (possibleNeighbors.Count > 0 && currentNode.Neighbors.Count < maxConnections)
                {
                    Node nextNode = possibleNeighbors[Random.Range(0, possibleNeighbors.Count)];
                    possibleNeighbors.Remove(nextNode);

                    // ��� �߰�
                    float distance = Vector2.Distance(currentNode.Position, nextNode.Position);
                    distance *= Random.Range(0.9f, 1.1f); // ���� ���� ����

                    currentNode.Neighbors.Add(nextNode);
                    edges.Add((currentNode, nextNode, distance));

                    nextActiveNodes.Add(nextNode);
                }
            }

            // ���� ���� Ȱ�� ��常 ����
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x + 1, y];
                if (node != null && !nextActiveNodes.Contains(node))
                {
                    nodes[x + 1, y] = null;
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
            Gizmos.DrawLine(edges[i].Item1.Position, edges[i].Item2.Position);
        }

        // ��� �׸���
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodes[x, y];
                if (node != null)
                {
                    Gizmos.color = GetNodeColor(node.Type);
                    Gizmos.DrawSphere(node.Position, 0.2f);
                }
            }
        }

        // ���� ���� ���� ��� ǥ��
        Node startNode = new Node(new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startNode.Position, 0.3f);

        Node bossNode = new Node(new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(bossNode.Position, 0.3f);
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
            float distance = Vector2.Distance(startNode.Position, selectedNode.Position);
            startNode.Neighbors.Add(selectedNode);
            edges.Add((startNode, selectedNode, distance));
        }

        // 1������ ���õ��� ���� ��� ����
        foreach (Node node in firstColumnNodes)
        {
            for (int y = 0; y < rows; y++)
            {
                if (nodes[0, y] == node) // ��Ȯ�� ��Ī�Ǵ� ��常 ����
                {
                    nodes[0, y] = null;
                    Debug.Log($"Removed unselected node at position: {node.Position}");
                    break;
                }
            }
        }
    }

    void HandleFinalBossNode(Node bossNode)
    {
        // 10�� ��� ����
        List<Node> lastColumnNodes = new List<Node>();

        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[columns - 1, y];
            if (node != null) lastColumnNodes.Add(node);
        }

        // 10���� ��� ��带 ������ ���� ����
        foreach (Node node in lastColumnNodes)
        {
            float distance = Vector2.Distance(bossNode.Position, node.Position);
            node.Neighbors.Add(bossNode);
            edges.Add((node, bossNode, distance));
        }

        if (!IsGraphConnected())
        {
            Debug.LogError("�׷����� ����Ǿ� ���� �ʽ��ϴ�!");
        }
    }

    bool IsGraphConnected()
    {
        HashSet<Node> visited = new HashSet<Node>();
        Stack<Node> stack = new Stack<Node>();

        // ù ��° Ȱ��ȭ�� ��� ã��
        Node startNode = null;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (nodes[x, y] != null)
                {
                    startNode = nodes[x, y];
                    break;
                }
            }
            if (startNode != null) break;
        }

        if (startNode == null)
        {
            Debug.LogError("No active nodes found. Cannot check connectivity.");
            return false;
        }

        stack.Push(startNode);

        // DFS Ž��
        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();

            if (currentNode != null && !visited.Contains(currentNode))
            {
                visited.Add(currentNode);
                Debug.Log($"Visited node at {currentNode.Position}");

                foreach (Node neighbor in currentNode.Neighbors)
                {
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        // ����� ��� ������ Ȱ��ȭ�� ��� ���� ��
        foreach (Node node in nodes)
        {
            if (node != null)
            {
                Debug.Log(node.Position.ToString());
                if (node != null && !visited.Contains(node))
                {
                    Debug.LogError($"Node at position {node.Position} is not connected.");
                    return false; // �湮���� ���� ��尡 ������ ������� ���� �׷���
                }
            }
        }

        return true; // ��� ��尡 ����Ǿ� ����
    }
}