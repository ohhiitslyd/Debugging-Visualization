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
    private CameraController cameraController;

    private bool myFocus = false; // if current block is being looked at; currently only toggled back to false when pressed ESC




    // Start is called before the first frame update
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        // node = new GraphNode();

        // node.address = address;
        // node.name = nodeName;
        // node.function_address = function_address;
        // node.instructions = instructions;
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

    void OnMouseDown()
    {

        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            Panel.Instance.panel.SetActive(true);
            Debug.Log("Node name: " + node.name);
            Panel.Instance.title.text = node.name;
            Panel.Instance.address.text = node.address.ToString();
            Panel.Instance.description.text = string.Join("\n", node.instructions.ToArray());
            Panel.Instance.successorsText.text = string.Join("\n", node.successors.Select(s => s.ToString()).ToArray());
            Panel.Instance.predecessorsText.text = string.Join("\n", node.predecessors.Select(s => s.ToString()).ToArray());

            // Assuming each code block has a predefined position and orthographic size
            Vector3 blockPosition = transform.position + new Vector3(0, 0, 0); // Adjust as needed
            cameraController.FocusOnBlock(blockPosition);

            StateManager.Instance.selectedBlock = node.address;
            StateManager.Instance.UpdateMaterials();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            GetComponent<Outline>().enabled = !GetComponent<Outline>().enabled;
        }
    }
}
