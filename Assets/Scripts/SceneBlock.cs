
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneBlock
{

    public GameObject sceneBlockPrefab;
    public TextMeshProUGUI codeBlockText;
    public TextMeshProUGUI codeBlockName;

     private float positionX;
     private float positionY;
     private float positionZ;
     private float size;
    //  private GraphNode node;

    /* Constructor for a single SceneBlock class. 
    Should be instantiated in BlockManager.cs */
    public SceneBlock(float x, float y, float z, float size, GameObject prefab) 
    {
        this.positionX = x;
        this.positionY = y;
        this.positionZ = z;
        this.size = size;
        this.sceneBlockPrefab = prefab;
    }

    public void SetCanvasReference(Transform obj) 
    {
       codeBlockName = obj.Find("Name").GetComponent<TextMeshProUGUI>();
       codeBlockName = obj.Find("TextInCodeBlock").GetComponent<TextMeshProUGUI>();
    }

    public void SetBlockPrefab(GameObject prefab)
    {
        sceneBlockPrefab = prefab;
    }

    public void SetBlockPositon(float x, float y, float z)
    {
        positionX = x;
        positionY = y;
        positionZ = z;
    }

    public void SetBlockSize(float newSize)
    {
        this.size = newSize;
    }

    public float GetSize()
    {
        return size;
    }

    public float GetPositionX() {
        return positionX;
    }

    public float GetPositionY() {
        return positionY;
    }

    public float GetPositionZ() {
        return positionZ;
    }
}
