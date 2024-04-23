using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{   List<SceneBlock> sceneBlocks;
    List<GameObject> sceneBlockObjs;
    // private GraphSctructure graph;
    public GameObject sceneBlockPrefab;

    // Start: Initializing lists and adding two test blocks
    void Start()
    {
        sceneBlocks = new List<SceneBlock>();
        sceneBlockObjs = new List<GameObject>();

        SceneBlock sb = new SceneBlock(0, 0, 0, 10, sceneBlockPrefab);
        SceneBlock sb2 = new SceneBlock(10, 0, 0, 10, sceneBlockPrefab);
        sceneBlocks.Add(sb);
        sceneBlocks.Add(sb2);
    }

    public void CreateBlocks()
    {
        foreach (SceneBlock sb in sceneBlocks) {
            Vector3 position = new Vector3(sb.GetPositionX(), sb.GetPositionY());
            GameObject obj = Instantiate(sceneBlockPrefab, position, Quaternion.identity);
            sceneBlockObjs.Add(obj);
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
