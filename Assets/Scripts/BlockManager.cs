using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{   List<SceneBlock> sceneBlocks;
    List<GameObject> sceneBlockObjs;
    // private GraphSctructure graph;
    public GameObject sceneBlockPrefab;


    public string filename = "angr_jsons/simple_debug__angr";
    private GraphStructure graphStructure;


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

            GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
            sceneBlockObjs.Add(obj);

            obj.GetComponent<SceneBlockObj>().SetGraphNode(node);
            obj.SetActive(true);

            index++;
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
