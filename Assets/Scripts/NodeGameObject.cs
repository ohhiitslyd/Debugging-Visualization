using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGameObject : MonoBehaviour
{
    public int address;
    public string nodeName;
    public int function_address;
    public List<string> instructions;
    public GraphNode node;

    // Start is called before the first frame update
    void Start()
    {
        node = new GraphNode();

        node.address = address;
        node.name = nodeName;
        node.function_address = function_address;
        node.instructions = instructions;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown() {
        Panel.Instance.panel.SetActive(true);
        Debug.Log(node);
        Panel.Instance.title.text = nodeName;
        Panel.Instance.address.text = address.ToString();
        Panel.Instance.description.text = string.Join("\n", instructions.ToArray());

    }
}
