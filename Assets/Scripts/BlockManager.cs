using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Edge
{
    public GameObject from;
    public GameObject to;
}

public class BlockManager : MonoBehaviour
{
    List<SceneBlock> sceneBlocks;
    List<GameObject> sceneBlockObjs;
    public Dictionary<int, GameObject> sceneBlockDict;
    // private GraphSctructure graph;
    public GameObject sceneBlockPrefab;


    public string filename = "angr_jsons/simple_debug__angr";
    private GraphStructure graphStructure;


    //private Dictionary<int, GameObject> nodeObjects = new Dictionary<int, GameObject>();
    //private float repulsiveForce = 100.0f;
    //private float springLength = 5.0f;
    //private float springConstant = 0.1f;

    public List<GameObject> testNodes;
    public List<Edge> testEdges;

    private bool updatingNodes;

    [SerializeField] private float movingThreshold = 0.01f;

    void UpdateGraphLayout(List<GameObject> nodes, List<Edge> edges, float repulsiveForceConstant, float springForceConstant, float damping)
    {
        Dictionary<GameObject, Vector3> forceDictionary = new Dictionary<GameObject, Vector3>();

        // Apply Repulsive Forces
        foreach (var nodeA in nodes)
        {
            forceDictionary[nodeA] = Vector3.zero;
            foreach (var nodeB in nodes)
            {
                if (nodeA != nodeB)
                {
                    Vector3 direction = nodeA.transform.position - nodeB.transform.position;
                    float distance = direction.magnitude;
                    float forceMagnitude = repulsiveForceConstant / distance;
                    forceDictionary[nodeA] += direction.normalized * forceMagnitude;
                }
            }
        }

        // Apply Spring (Attractive) Forces
        foreach (var edge in edges)
        {
            Vector3 direction = edge.from.transform.position - edge.to.transform.position;
            float distance = direction.magnitude;
            float forceMagnitude = (distance - 1) * springForceConstant; // Assuming ideal length of spring is 1 unit
            forceDictionary[edge.from] -= direction.normalized * forceMagnitude;
            forceDictionary[edge.to] += direction.normalized * forceMagnitude;
        }

        // Update Positions based on net forces
        bool isMoving = false;
        foreach (var node in nodes)
        {
            Vector3 velocity = forceDictionary[node] * Time.deltaTime;
            node.transform.position += velocity * damping;
            if (velocity.magnitude > movingThreshold) isMoving = true;
        }
        if (!isMoving) updatingNodes = false;
    }

    public void CreateFunctionBlocks()
    {
        Dictionary<int, Function> addrToFunction = Function.AddressToFunctionFromGraphStructure(graphStructure);
        List<Function> functions = addrToFunction.Values.ToList();
        functions.Sort((x, y) => y.nodes.Count - x.nodes.Count);
        for (int i = functions.Count - 1; i >= 0; i--)
        {
            List<GraphNode> nodes = functions[i].nodes;
            // Debug.Log($"Function: {functions[i].name} has {nodes.Count} nodes");
            if (nodes.Count > 1) break;
            if (nodes.First().name == null) functions.RemoveAt(i);
        }

        float spacingX = 20f;
        float spacingZ = 10f;

        for (int funcIndex = 0; funcIndex < functions.Count; funcIndex++)
        {
            GameObject functionObj = new GameObject("Function " + funcIndex);
            for (int blockIndex = 0; blockIndex < functions[funcIndex].nodes.Count; blockIndex++)
            {
                GraphNode node = functions[funcIndex].nodes[blockIndex];
                Debug.Log(node.name);
                int row = blockIndex;
                int col = funcIndex;

                Vector3 position = new Vector3(col * spacingX, 0, row * -spacingZ);

                GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
                obj.transform.parent = functionObj.transform;
                sceneBlockObjs.Add(obj);
                sceneBlockDict[node.address] = obj;
                // test force thing
                if (funcIndex == 0)
                {
                    testNodes.Add(obj);
                }

                obj.GetComponent<SceneBlockObj>().SetGraphNode(node);
                obj.GetComponent<NodeGameObject>().SetNode(node);

                obj.SetActive(true);

            }
        }
        // test force thing
        for (int blockIndex = 0; blockIndex < functions[0].nodes.Count; blockIndex++)
        {
            GraphNode node = functions[0].nodes[blockIndex];
            sceneBlockDict[node.address].transform.position = Vector3.Scale(Random.onUnitSphere * Random.Range(0f, 0.1f), new Vector3(1f, 0f, 1f));
            List<Connection> successors = graphStructure.getSuccessors(node.address);
            foreach (Connection succ in successors)
            {
                if (graphStructure.nodes[succ.target].function_address != node.function_address) continue;
                Edge edge = new Edge();
                edge.from = sceneBlockDict[node.address];
                edge.to = sceneBlockDict[succ.target];
                testEdges.Add(edge);
            }
        }
        Debug.Log("testEdges.Count: " + testEdges.Count);
    }

    // Start: Initializing lists and adding two test blocks
    void Start()
    {
        sceneBlocks = new List<SceneBlock>();
        sceneBlockObjs = new List<GameObject>();
        sceneBlockDict = new Dictionary<int, GameObject>();
        testEdges = new List<Edge>();
        testNodes = new List<GameObject>();


        //SceneBlock sb = new SceneBlock(0, 0, 0, 10, sceneBlockPrefab);
        //SceneBlock sb2 = new SceneBlock(10, 0, 0, 10, sceneBlockPrefab);
        //sceneBlocks.Add(sb);
        //sceneBlocks.Add(sb2);


        GetGraphStructure();
        // CreateGraphBlocks();
        CreateFunctionBlocks();

        updatingNodes = true;
    }

    public void CreateBlocks()
    {
        foreach (SceneBlock sb in sceneBlocks)
        {
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

            GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
            sceneBlockObjs.Add(obj);
            sceneBlockDict[node.address] = obj;

            obj.GetComponent<SceneBlockObj>().SetGraphNode(node);
            obj.GetComponent<NodeGameObject>().SetNode(node);
            obj.SetActive(true);

            index++;
        }
    }

    void Update()
    {
        // test force thing
        if (updatingNodes && testNodes.Count > 0 && testEdges.Count > 0) UpdateGraphLayout(testNodes, testEdges, 40f, 1f, 1f);
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
