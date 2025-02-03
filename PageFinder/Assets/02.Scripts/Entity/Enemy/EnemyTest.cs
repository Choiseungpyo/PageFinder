using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    public Vector3 pos = Vector3.zero;

    public int runDist = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetRandomPos(pos);
    }

    private void SetRandomPos(Vector3 pos)
    {
        // ���߿� Enemy�� ���������� ���� ������ �Է� �ޱ�(���Ÿ��� ���)

        Vector3 randomPoint = Random.insideUnitSphere * runDist;
        Debug.Log($"RandomPoint : {randomPoint}");
        Debug.Log($"pos + randomPoint : {pos + randomPoint}");

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos + new Vector3(0,0, 100), out hit, runDist, NavMesh.AllAreas))
        {
            Debug.Log($"hit.pos :  {hit.position}");
            transform.position = hit.position;
        }
        Debug.Log(hit);
    }
}
