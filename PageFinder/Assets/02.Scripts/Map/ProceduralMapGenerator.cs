using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 10;
    [SerializeField] private float nodeSpacing = 2.0f; // �Ÿ� ����
    [SerializeField] private float minOffset = 0.9f;
    [SerializeField] private float maxOffset = 1.1f;
    private Node startNode;

    public enum NodeType { Battle_Normal, Battle_Elite, Quest, Treasure, Market, Comma, Boss }

    public class Node
    {
        public int column;
        public int row;
        public Vector2 position;
        public NodeType type;
        public List<Node> Neighbors = new();

        public Node(int column, int row, Vector2 position, NodeType type)
        {
            this.column = column;
            this.row = row;
            this.position = position;
            this.type = type;
        }
    }

    private Node[,] nodes;
    private List<(Node, Node, float)> edges = new(); // ��� �� �Ÿ� ����

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
        startNode = new(-1, rows / 2, new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal);
        List<Node> firstColumnNodes = new();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new(x * nodeSpacing, y * nodeSpacing * Random.Range(minOffset, maxOffset));
                NodeType type = DetermineNodeType(x, y);

                Node newNode = new(x, y, position, type);
                nodes[x, y] = newNode;

                // 1�� ��� �߰�
                if (x == 0) firstColumnNodes.Add(newNode);
            }
        }

        // 10�� ������ ���� ������ ��� ����
        Node bossNode = new(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss);

        // 1���� 10�� ó��
        HandleFirstColumnNodes(startNode, firstColumnNodes);
        Debug.Log("��� ���� �Ϸ�");

        ConnectNodes(bossNode);
    }

    NodeType DetermineNodeType(int x, int y)
    {
        if (x == 0) return NodeType.Battle_Normal; // 1��: �Ϲ� ��Ʋ
        if (x == 4) return NodeType.Treasure; // 5��: Ʈ����
        if (x == 9) return NodeType.Comma; // 10��: �޸�
        if (x > 9) return NodeType.Boss; // 10�� ����: ����

        float rand = Random.value;

        if (rand < battleNormalProbability && x <= 4) return NodeType.Battle_Normal; // �Ϲ� ��Ʋ ���� ����
        rand -= battleNormalProbability;

        if (rand < battleEliteProbability && x >= 6) return NodeType.Battle_Elite; // ���� ��Ʋ ���� ����
        rand -= battleEliteProbability;

        if (rand < questProbability) return NodeType.Quest;
        rand -= questProbability;

        if (rand < marketProbability) return NodeType.Market;
        rand -= marketProbability;

        return NodeType.Comma; // �⺻��
    }

    void ConnectNodes(Node bossNode)
    {
        HashSet<Node> activeNodes = new();

        // ù ��° ���� Ȱ�� ��� �߰�
        for (int y = 0; y < rows; y++)
        {
            if (nodes[0, y] != null)
            {
                activeNodes.Add(nodes[0, y]);
            }
        }

        // ��(column) �������� ��� ����
        for (int x = 0; x < columns - 1; x++)
        {
            HashSet<Node> nextActiveNodes = new();
            bool hasTwoNeighbors = false;
            int localSingleConnections = 0; // ���� ������ 1�� ����� ��� ��

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

                // ������ �̿� ����
                while (neighborCandidates.Count > 0)
                {
                    Node nextNode = neighborCandidates[Random.Range(0, neighborCandidates.Count)];
                    neighborCandidates.Remove(nextNode);

                    // ��� �߰� (�Ÿ��� ���� ���� ����)
                    float distance = Vector2.Distance(currentNode.position, nextNode.position);
                    distance *= Random.Range(minOffset, maxOffset);

                    currentNode.Neighbors.Add(nextNode);
                    edges.Add((currentNode, nextNode, distance));
                    nextActiveNodes.Add(nextNode);

                    if (localSingleConnections == activeNodes.Count - 1)
                    {
                        hasTwoNeighbors = true;
                        localSingleConnections++;
                        continue;
                    }
                    else
                    {
                        if (Random.value < 0.5f || hasTwoNeighbors)
                        {
                            localSingleConnections++; // ���� ������ ���� ����� Ƚ�� ����
                            break;
                        }
                        else hasTwoNeighbors = true;
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
        Node startNode = new Node(-1, -1, new Vector2(-nodeSpacing, rows / 2 * nodeSpacing), NodeType.Battle_Normal);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startNode.position, 0.3f);

        Node bossNode = new Node(columns+1, rows+1, new Vector2(columns * nodeSpacing, rows / 2 * nodeSpacing), NodeType.Boss);
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
        // 10�� ��� ����
        List<Node> lastColumnNodes = new();

        for (int y = 0; y < rows; y++)
        {
            Node node = nodes[columns - 1, y];
            if (node != null) lastColumnNodes.Add(node);
        }

        // 10���� ��� ��带 ������ ���� ����
        foreach (Node node in lastColumnNodes)
        {
            float distance = Vector2.Distance(bossNode.position, node.position);
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
}