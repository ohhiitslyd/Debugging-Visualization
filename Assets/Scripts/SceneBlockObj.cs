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
        if(nameText == null || instructionText == null)
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
}
