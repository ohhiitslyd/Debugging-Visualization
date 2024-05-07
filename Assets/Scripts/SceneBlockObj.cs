using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SceneBlockObj : MonoBehaviour
{
    private TMP_Text nameText;
    private TMP_Text instructionText;

    private GraphNode node;

    public void SetGraphNode(GraphNode graphNode)
    {
        node = graphNode;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (nameText == null || instructionText == null)
        {
            nameText = transform.Find("VerticalAdjust/Canvas/Name").GetComponent<TMP_Text>();
            instructionText = transform.Find("VerticalAdjust/Canvas/TextInCodeBlock").GetComponent<TMP_Text>();
        }

        if (node != null)
        {
            nameText.text = node.name;
            instructionText.text = String.Join("\n", node.instructions);
        }
    }

    public Vector3 GetFrontCenter()
    {
        //Debug.Log("====== jiaqi front ======");
        //Debug.Log(transform.position);
        //Debug.Log(transform.position + transform.forward * (transform.localScale.z / 2));
        return transform.position + transform.forward * (transform.localScale.z / 2);
    }

    public Vector3 GetBackCenter()
    {
        //Debug.Log("====== jiaqi back======");
        //Debug.Log(transform.position);
        //Debug.Log(transform.position - transform.forward * (transform.localScale.z / 2));
        return transform.position - transform.forward * (transform.localScale.z / 2);
    }
}
