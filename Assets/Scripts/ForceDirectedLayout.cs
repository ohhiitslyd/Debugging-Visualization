using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirectedLayout : MonoBehaviour
{
    public GraphStructure graph;
    public GameObject nodePrefab;
    private Dictionary<int, GameObject> nodeObjects = new Dictionary<int, GameObject>();
    private float repulsiveForce = 100.0f;
    private float springLength = 5.0f;
    private float springConstant = 0.1f;

    void Start()
    {
        InitializeNodes();
        StartCoroutine(LayoutCoroutine());
    }

    void InitializeNodes()
    {
        foreach (var node in graph.nodes.Values)
        {
            var nodeObj = Instantiate(nodePrefab, Random.insideUnitSphere * 10, Quaternion.identity);
            nodeObj.transform.localScale = new Vector3(7, 3, 7);
            nodeObjects[node.address] = nodeObj;
        }
    }

    IEnumerator LayoutCoroutine()
    {
        bool stable = false;
        while (!stable)
        {
            stable = ApplyForces();
            yield return null;  // Wait for the next frame
        }
    }

    bool ApplyForces()
    {
        Dictionary<int, Vector3> forces = new Dictionary<int, Vector3>();
        foreach (var nodeA in graph.nodes.Values)
        {
            forces[nodeA.address] = Vector3.zero;
            foreach (var nodeB in graph.nodes.Values)
            {
                if (nodeA.address != nodeB.address)
                {
                    Vector3 direction = nodeObjects[nodeA.address].transform.position - nodeObjects[nodeB.address].transform.position;
                    float distance = direction.magnitude;
                    float repulsive = repulsiveForce / (distance * distance);
                    forces[nodeA.address] += direction.normalized * repulsive;
                }
            }
        }

        foreach (var connectionList in graph.successors)
        {
            foreach (var connection in connectionList.Value)
            {
                var nodeA = graph.nodes[connectionList.Key];
                var nodeB = graph.nodes[connection.target];
                Vector3 direction = nodeObjects[nodeB.address].transform.position - nodeObjects[nodeA.address].transform.position;
                float displacement = springLength - direction.magnitude;
                Vector3 springForce = direction.normalized * (displacement * springConstant);
                forces[nodeA.address] -= springForce;
                forces[nodeB.address] += springForce;
            }
        }

        float maxDisplacement = 0.0f;
        foreach (var node in graph.nodes.Values)
        {
            Vector3 displacement = forces[node.address] * Time.deltaTime;
            nodeObjects[node.address].transform.position += displacement;
            maxDisplacement = Mathf.Max(maxDisplacement, displacement.magnitude);
        }

        return maxDisplacement < 0.01f;  // Consider it stable if max displacement is small
    }
}
