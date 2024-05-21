using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManagerFunctionPlacement : MonoBehaviour
{
    List<SceneBlock> sceneBlocks;
    List<GameObject> sceneBlockObjs;
    // private GraphSctructure graph;
    public GameObject sceneBlockPrefab;
    public GameObject edgePrefab;
    public GameObject functionPanelPrefab;


    public string filename = "angr_jsons/simple_debug__angr";
    private GraphStructure graphStructure;

    public List<GameObject> testNodes;
    public List<Edge> testEdges;
    public Dictionary<int, Function> addrToFunction;
    public Dictionary<Function, List<Edge>> functionToEdges;
    public Dictionary<Function, List<GameObject>> functionToNodes;
    public Dictionary<int, GameObject> functionAddrToFunctionGameObject;
    public List<NodeEdge> edgesBetweenFunctions;


    private bool updatingNodes;

    private string dottedGrayArrowEdgeType = "dotted_gray_arrow";

    [SerializeField] private float movingThreshold = 0.01f;

    float UpdateGraphLayout(List<GameObject> nodes, List<Edge> edges, float repulsiveForceConstant, float springForceConstant, float damping, float idealSpringLength)
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
            float forceMagnitude = (distance - idealSpringLength) * springForceConstant; // Now using customizable ideal length
            forceDictionary[edge.from] -= direction.normalized * forceMagnitude;
            forceDictionary[edge.to] += direction.normalized * forceMagnitude;
        }

        // Update Positions based on net forces
        float maxVelocity = 0;
        foreach (var node in nodes)
        {
            Vector3 velocity = forceDictionary[node];
            Vector3 timeScaledVelocity = velocity * 0.006f; // Example time scaling factor

            node.transform.position += timeScaledVelocity * damping;
            if (velocity.magnitude > maxVelocity) maxVelocity = velocity.magnitude;
        }
        return maxVelocity;
    }
    public void CreateFunctionBlocks()
    {
        functionToEdges = new Dictionary<Function, List<Edge>>();
        functionToNodes = new Dictionary<Function, List<GameObject>>();
        addrToFunction = Function.AddressToFunctionFromGraphStructure(graphStructure);
        functionAddrToFunctionGameObject = new Dictionary<int, GameObject>();
        edgesBetweenFunctions = new List<NodeEdge>();

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
            Function function = functions[funcIndex];
            // List<Edge> functionEdges = new List<Edge>();
            List<GameObject> functionNodes = new List<GameObject>();
            GameObject functionObj = new GameObject("Function " + funcIndex);
            functionAddrToFunctionGameObject[function.address] = functionObj;
            for (int blockIndex = 0; blockIndex < functions[funcIndex].nodes.Count; blockIndex++)
            {
                GraphNode node = functions[funcIndex].nodes[blockIndex];
                // Debug.Log(node.name);
                int row = blockIndex;
                int col = funcIndex;

                Vector3 position = Vector3.Scale(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0f, 0.1f), new Vector3(1f, 0f, 1f));

                GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
                obj.transform.parent = functionObj.transform;
                sceneBlockObjs.Add(obj);
                StateManager.Instance.sceneBlockDict[node.address] = obj;
                // test force thing
                if (funcIndex == 0)
                {
                    testNodes.Add(obj);
                }
                functionNodes.Add(obj);

                obj.GetComponent<SceneBlockObj>().SetGraphNode(node);
                obj.GetComponent<NodeGameObject>().SetNode(node);

                obj.SetActive(true);

            }
            functionToNodes[function] = functionNodes;
            // functionToEdges[function] = functionEdges;
        }
        // test force thing
        // for (int blockIndex = 0; blockIndex < functions[0].nodes.Count; blockIndex++)
        // {
        //     GraphNode node = functions[0].nodes[blockIndex];
        //     sceneBlockDict[node.address].transform.position = Vector3.Scale(Random.onUnitSphere * Random.Range(0f, 0.1f), new Vector3(1f, 0f, 1f));
        //     List<Connection> successors = graphStructure.getSuccessors(node.address);
        //     foreach (Connection succ in successors)
        //     {
        //         if (graphStructure.nodes[succ.target].function_address != node.function_address) continue;
        //         Edge edge = new Edge();
        //         edge.from = sceneBlockDict[node.address];
        //         edge.to = sceneBlockDict[succ.target];
        //         testEdges.Add(edge);
        //     }
        // }
        // Debug.Log("testEdges.Count: " + testEdges.Count);
        foreach (Function function in functions)
        {
            List<GameObject> functionNodes = functionToNodes[function];
            List<Edge> functionEdges = new List<Edge>();

            for (int i = 0; i < functionNodes.Count; i++)
            {
                GameObject nodeObj = functionNodes[i];
                GraphNode node = nodeObj.GetComponent<NodeGameObject>().node;
                List<Connection> successors = graphStructure.getSuccessors(node.address);
                foreach (Connection succ in successors)
                {
                    if (node.function_address != graphStructure.nodes[succ.target].function_address)
                    {
                        edgesBetweenFunctions.Add(new NodeEdge
                        {
                            from = node,
                            to = graphStructure.nodes[succ.target],
                            type = succ.type,
                        });
                        continue;
                    }
                    Edge edge = new Edge
                    {
                        from = nodeObj,
                        to = StateManager.Instance.sceneBlockDict[succ.target],
                        type = succ.type,
                    };
                    functionEdges.Add(edge);
                }

                if (i == 0) continue;

                List<Connection> predecessors = graphStructure.getPredecessors(node.address);
                if (predecessors.Count == 0) continue;
                // check if none of the predecessors are in the same function
                bool noPreds = true;
                foreach (Connection pred in predecessors)
                {
                    if (graphStructure.nodes[pred.target].function_address == node.function_address)
                    {
                        noPreds = false;
                        break;
                    }
                }

                if (noPreds)
                {
                    Edge edge = new Edge
                    {
                        from = functionNodes[i - 1],
                        to = nodeObj,
                        type = dottedGrayArrowEdgeType,
                    };
                    functionEdges.Add(edge);
                }
            }


            functionToEdges[function] = functionEdges;
        }
        foreach (Function function in functions)
        {
            Debug.Log($"Function: {function.name} has {function.nodes.Count} nodes and {functionToEdges[function].Count} edges");
        }


        StartCoroutine(MakeGraph());


    }


    IEnumerator MakeGraph()
    {
        yield return new WaitUntil(() => positionFunctionNodes());
        yield return new WaitUntil(() => createFunctionPanels());
        yield return new WaitUntil(() => positionFunctions());
        // TODO: - flora. it is ugly....
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => placeEdges());
    }

    bool placeEdges()
    {
        foreach (Function function in functionToNodes.Keys)
        {
            List<Edge> functionEdges = functionToEdges[function];
            CreateGraphEdges(functionEdges);
        }
        return true;

    }
    bool positionFunctions()
    {
        List<GameObject> functionObjs = new List<GameObject>();
        List<Edge> functionEdges = new List<Edge>();
        foreach (GameObject functionObj in functionAddrToFunctionGameObject.Values)
        {
            functionObj.transform.position = Vector3.Scale(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0f, 0.1f), new Vector3(1f, 0f, 1f));
            functionObjs.Add(functionObj);
        }
        foreach (NodeEdge edge in edgesBetweenFunctions)
        {
            int fa1 = edge.from.function_address;
            int fa2 = edge.to.function_address;
            if (!functionAddrToFunctionGameObject.ContainsKey(fa1) || !functionAddrToFunctionGameObject.ContainsKey(fa2)) continue;
            GameObject f1 = functionAddrToFunctionGameObject[fa1];
            GameObject f2 = functionAddrToFunctionGameObject[fa2];
            functionEdges.Add(new Edge
            {
                from = f1,
                to = f2,
                type = edge.type,
            });
        }
        // updatingNodes = true;
        // while (updatingNodes)
        // {
        //     float functionMaxVelocity = UpdateGraphLayout(functionObjs, functionEdges, 1f, 1f, 1f, 50f);
        //     if (functionMaxVelocity < movingThreshold) updatingNodes = false;
        // }
        List<GameObject> sortedFunctionObjs = SortFunctionNodes(functionObjs, functionEdges);
        float spacing = 50f;
        for (int i = 0; i < sortedFunctionObjs.Count; i++)
        {
            GameObject functionObj = sortedFunctionObjs[i];
            functionObj.transform.position = new Vector3(i * spacing, 0f, 0f);
        }
        Debug.Log("Function placement complete");
        return true;
    }

    GameObject createFunctionPanel(Function function)
    {
        float padding = 5f;
        List<GameObject> functionNodes = functionToNodes[function];
        // get min and max x and z values for all nodes - incorporating the size of the node block cubes
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        foreach (GameObject node in functionNodes)
        {
            Transform nodeTransform = node.transform;
            float bigx = nodeTransform.position.x + 0.5f * nodeTransform.localScale.x;
            float smallx = nodeTransform.position.x - 0.5f * nodeTransform.localScale.x;
            float bigz = nodeTransform.position.z + 0.5f * nodeTransform.localScale.z;
            float smallz = nodeTransform.position.z - 0.5f * nodeTransform.localScale.z;
            if (bigx > maxX) maxX = bigx;
            if (smallx < minX) minX = smallx;
            if (bigz > maxZ) maxZ = bigz;
            if (smallz < minZ) minZ = smallz;
        }

        Vector3 panelPosition = new Vector3((minX + maxX) / 2, -1f, (minZ + maxZ) / 2);
        GameObject panel = Instantiate(functionPanelPrefab, panelPosition, Quaternion.identity);
        panel.transform.localScale = new Vector3(maxX - minX + padding, 1, maxZ - minZ + padding);
        return panel;
    }

    bool createFunctionPanels()
    {
        foreach (Function function in functionToNodes.Keys)
        {
            GameObject functionObj = functionAddrToFunctionGameObject[function.address];
            GameObject panel = createFunctionPanel(function);
            panel.transform.parent = functionObj.transform;
        }
        return true;
    }

    private void CreateGraphEdges(List<Edge> edges)
    {
        foreach (Edge edge in edges)
        {
            Vector3 fromCenter = edge.from.transform.position;
            Vector3 toCenter = edge.to.transform.position;

            Vector3 fromEdgePoint = FindClosestIntersectionPoint(fromCenter, toCenter, edge.from.transform.localScale * 0.5f);
            Vector3 toEdgePoint = FindClosestIntersectionPoint(toCenter, fromCenter, edge.to.transform.localScale * 0.5f);

            GameObject edgeObj = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
            Arrow.ArrowRenderer animatedArrowRenderer = edgeObj.GetComponent<Arrow.ArrowRenderer>();
            animatedArrowRenderer.SetPositions(fromEdgePoint, toEdgePoint);
        }
    }

    private Vector3 FindClosestIntersectionPoint(Vector3 center, Vector3 target, Vector3 halfExtents)
    {
        Vector3 direction = (target - center).normalized;
        float distance = Vector3.Distance(center, target);

        if (Physics.Raycast(center, direction, out RaycastHit hit, distance))
        {
            return hit.point;
        }
        else
        {
            return target;
        }
    }


    bool positionFunctionNodes()
    {
        updatingNodes = true;
        while (updatingNodes)
        {
            float globalMaxVelocity = 0f;
            foreach (Function function in functionToNodes.Keys)
            {
                List<GameObject> functionNodes = functionToNodes[function];
                List<Edge> functionEdges = functionToEdges[function];
                float functionMaxVelocity = UpdateGraphLayout(functionNodes, functionEdges, 40f, 1f, 1f, 5f);
                if (functionMaxVelocity > globalMaxVelocity) globalMaxVelocity = functionMaxVelocity;
            }
            if (globalMaxVelocity < movingThreshold) updatingNodes = false;
        }
        return true;
    }

    // Start: Initializing lists and adding two test blocks

    List<GameObject> SortFunctionNodes(List<GameObject> nodes, List<Edge> edges)
    {
        // Build the graph as an adjacency list
        Dictionary<GameObject, List<GameObject>> graph = new Dictionary<GameObject, List<GameObject>>();
        foreach (var node in nodes)
        {
            graph[node] = new List<GameObject>();
        }

        foreach (var edge in edges)
        {
            graph[edge.from].Add(edge.to);
            graph[edge.to].Add(edge.from); // Assuming undirected graph for mutual visibility
        }

        // List to hold the sorted nodes
        List<GameObject> sortedNodes = new List<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // DFS function to traverse nodes
        Action<GameObject> Dfs = null;
        Dfs = (GameObject node) =>
        {
            visited.Add(node);
            sortedNodes.Add(node);
            foreach (var neighbor in graph[node])
            {
                if (!visited.Contains(neighbor))
                {
                    Dfs(neighbor);
                }
            }
        };

        // Visit each node (this ensures that disconnected parts of the graph are handled)
        foreach (var node in nodes)
        {
            if (!visited.Contains(node))
            {
                Dfs(node);
            }
        }

        return sortedNodes;
    }
    void Start()
    {
        sceneBlocks = new List<SceneBlock>();
        sceneBlockObjs = new List<GameObject>();
        StateManager.Instance.sceneBlockDict = new Dictionary<int, GameObject>();
        testEdges = new List<Edge>();
        testNodes = new List<GameObject>();



        //SceneBlock sb = new SceneBlock(0, 0, 0, 10, sceneBlockPrefab);
        //SceneBlock sb2 = new SceneBlock(10, 0, 0, 10, sceneBlockPrefab);
        //sceneBlocks.Add(sb);
        //sceneBlocks.Add(sb2);


        GetGraphStructure();
        // CreateGraphBlocks();
        CreateFunctionBlocks();

        // updatingNodes = true;
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

            GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
            sceneBlockObjs.Add(obj);
            StateManager.Instance.sceneBlockDict[node.address] = obj;

            obj.GetComponent<SceneBlockObj>().SetGraphNode(node);
            obj.GetComponent<NodeGameObject>().SetNode(node);
            obj.SetActive(true);

            index++;
        }
    }

    void Update()
    {
        // previous function placement code, now in positionFunctionNodes() for instant placement rather than updating every frame
        // if (!updatingNodes) return;
        // float globalMaxVelocity = 0f;
        // foreach (Function function in functionToNodes.Keys)
        // {
        //     List<GameObject> functionNodes = functionToNodes[function];
        //     List<Edge> functionEdges = functionToEdges[function];
        //     float functionMaxVelocity = UpdateGraphLayout(functionNodes, functionEdges, 40f, 1f, 1f);
        //     if (functionMaxVelocity > globalMaxVelocity) globalMaxVelocity = functionMaxVelocity;
        // }
        // if (globalMaxVelocity < movingThreshold) updatingNodes = false;


        // call placeedges when I press k
        if (Input.GetKeyDown(KeyCode.K))
        {
            placeEdges();
        }
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
