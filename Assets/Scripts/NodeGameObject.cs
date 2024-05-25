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
    public Panel panel;

    private CameraController cameraController;

    private bool myFocus = false; // if current block is being looked at; currently only toggled back to false when pressed ESC


    public bool myHighlight = false;
    private BlockManagerFunctionPlacement blockManager;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        // node = new GraphNode();

        // node.address = address;
        // node.name = nodeName;
        // node.function_address = function_address;
        // node.instructions = instructions;

        // TODO: - flora. temp write like this, will change later after we turn the blockmanager as singleton
        blockManager = GameObject.Find("BlockManger").GetComponent<BlockManagerFunctionPlacement>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (cameraController.focusMode){
        //     if(!myFocus){
        //         GetComponent<Renderer>().material = unfocused;
        //     } else {
        //         GetComponent<Renderer>().material = normal;
        //     }
        // } else {
        //     GetComponent<Renderer>().material = normal;
        // }

        // if (Input.GetKey(KeyCode.Escape)){
        //     myFocus = false;
        // }
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

    public void Click()
    {

        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            foreach (GameObject sceneBlock in StateManager.Instance.sceneBlockDict.Values)
            {
                sceneBlock.GetComponent<Panel>().panel.SetActive(false);
            }
            panel.panel.SetActive(true);
            Debug.Log("Node name: " + node.name);
            panel.title.text = node.name;
            panel.address.text = node.address.ToString();
            panel.description.text = string.Join("\n", node.instructions.ToArray());
            panel.successorsText.text = string.Join("\n", node.successors.Select(s => s.ToString()).ToArray());
            panel.predecessorsText.text = string.Join("\n", node.predecessors.Select(s => s.ToString()).ToArray());

            // Assuming each code block has a predefined position and orthographic size
            Vector3 blockPosition = transform.position + new Vector3(0, 0, 0); // Adjust as needed
            cameraController.FocusOnBlock(blockPosition);

            StateManager.Instance.selectedBlock = node.address;
            StateManager.Instance.UpdateMaterials();
        }

        // TODO: - flora. after we focus the node, highlight outline is hard to see
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetHighlight();
        }
    }

    private void SetHighlight()
    {
        // for itself
        GetComponent<Outline>().enabled = !myHighlight;
        myHighlight = !myHighlight;

        // for edges connected to it
        foreach (Edge edge in blockManager.GetAllEdges())
        {
            NodeGameObject fromNode = edge.from.GetComponent<NodeGameObject>();
            NodeGameObject toNode = edge.to.GetComponent<NodeGameObject>();

            if (fromNode.myHighlight && toNode.myHighlight)
            {
                Debug.Log("+++++++++");
                edge.Highlight();
            }
            else
            {
                edge.Unhighlight();
            }
        }


    }
}
