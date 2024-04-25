using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BlockManager : MonoBehaviour
{   List<SceneBlock> sceneBlocks;
    List<GameObject> sceneBlockObjs;
    // private GraphSctructure graph;
    public GameObject sceneBlockPrefab;


    public string filename = "angr_jsons/simple_debug__angr";
    private GraphStructure graphStructure;

    private Dictionary<int, GameObject> nodeObjects = new Dictionary<int, GameObject>();
    private float repulsiveForce = 100.0f;
    private float springLength = 5.0f;
    private float springConstant = 0.1f;


    // Start: Initializing lists and adding two test blocks
    void Start()
    {
        sceneBlocks = new List<SceneBlock>();
        sceneBlockObjs = new List<GameObject>();

        //SceneBlock sb = new SceneBlock(0, 0, 0, 10, sceneBlockPrefab);
        //SceneBlock sb2 = new SceneBlock(10, 0, 0, 10, sceneBlockPrefab);
        //sceneBlocks.Add(sb);
        //sceneBlocks.Add(sb2);


        GetGraphStructure();
        CreateGraphBlocks();
    }

    public void CreateBlocks()
    {
        foreach (SceneBlock sb in sceneBlocks) {
            Vector3 position = new Vector3(sb.GetPositionX(), sb.GetPositionY());
            GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
            sceneBlockObjs.Add(obj);
        }
    }

    public void GetGraphStructure()
    {
        ParsedJsonData parsedJsonData = ParsedJsonData.JSONToParsedData(filename);
        if (parsedJsonData == null)
        {
            Debug.Log("Failed to load JSON Data.");
            return;
        }
        graphStructure = GraphStructure.ParsedJSONToGraph(parsedJsonData);
        if (graphStructure == null)
        {
            Debug.Log("Failed to convert to Graph Structure.");
            return;
        }
    }

    // TODO: - floragan. temp logic
    private void CreateGraphBlocks()
    {
        int index = 0;
        int rowLength = 5;
        float spacing = 10f;

        foreach (GraphNode node in graphStructure.nodes.Values)
        {
            Debug.Log(node.name);

            int row = index / rowLength;
            int col = index % rowLength;

            Vector3 position = new Vector3(col * spacing, 0, row * -spacing);

            //GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
            GameObject obj = Instantiate(sceneBlockPrefab, Random.insideUnitSphere * 10, Quaternion.identity);
            sceneBlockObjs.Add(obj);
            nodeObjects[node.address] = obj;

            obj.GetComponent<SceneBlockObj>().SetGraphNode(node);
            obj.SetActive(true);

            index++;
        }

        // begin layout
        // TODO: - floragan. check here!!
        StartCoroutine(LayoutCoroutine());
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
        foreach (var nodeA in graphStructure.nodes.Values)
        {
            forces[nodeA.address] = Vector3.zero;
            foreach (var nodeB in graphStructure.nodes.Values)
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

        foreach (var connectionList in graphStructure.successors)
        {
            foreach (var connection in connectionList.Value)
            {
                var nodeA = graphStructure.nodes[connectionList.Key];
                var nodeB = graphStructure.nodes[connection.target];
                Vector3 direction = nodeObjects[nodeB.address].transform.position - nodeObjects[nodeA.address].transform.position;
                float displacement = springLength - direction.magnitude;
                Vector3 springForce = direction.normalized * (displacement * springConstant);
                forces[nodeA.address] -= springForce;
                forces[nodeB.address] += springForce;
            }
        }

        float maxDisplacement = 0.0f;
        foreach (var node in graphStructure.nodes.Values)
        {
            Vector3 displacement = forces[node.address] * Time.deltaTime;
            nodeObjects[node.address].transform.position += displacement;
            maxDisplacement = Mathf.Max(maxDisplacement, displacement.magnitude);
        }

        return maxDisplacement < 0.02f;  // Consider it stable if max displacement is small
    }

    // public void ChangeUIText()
    // {
    //     for (int i = 0; i < sceneBlocks.Count; i++)
    //     {
    //         sceneBlocks[i].SetCanvasReference(sceneBlockObjs[i].transform);
    //         sceneBlocks[i].codeBlockName.text = "This is a new name";
    //         sceneBlocks[i].codeBlockText.text = "This is a new body paragraph";
    //     }
    // }
}
