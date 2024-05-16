using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;

public class NodeGameObject : MonoBehaviour
{
    public int address;
    public string nodeName;
    public int function_address;
    public List<string> instructions;
    public List<Connection> successors;
    public List<Connection> predecessors;
    public GraphNode node;



    // Start is called before the first frame update
    void Start()
    {
        // node = new GraphNode();

        // node.address = address;
        // node.name = nodeName;
        // node.function_address = function_address;
        // node.instructions = instructions;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetNode(GraphNode graphNode)
    {
        node = graphNode;
        address = node.address;
        nodeName = node.name;
        function_address = node.function_address;
        instructions = node.instructions;
        successors = node.successors;
        predecessors = node.predecessors;
    }

    void OnMouseDown()
    {
        if(!Input.GetKey(KeyCode.Space)){
            Panel.Instance.panel.SetActive(true);
            Debug.Log("Node name: " + node.name);
            Panel.Instance.title.text = node.name;
            Panel.Instance.address.text = node.address.ToString();
            Panel.Instance.description.text = string.Join("\n", node.instructions.ToArray());
            Panel.Instance.successorsText.text = string.Join("\n", node.successors.Select(s => s.ToString()).ToArray());
            Panel.Instance.predecessorsText.text = string.Join("\n", node.predecessors.Select(s => s.ToString()).ToArray());
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            GetComponent<Outline>().enabled = !GetComponent<Outline>().enabled;
        }
    }
}
