using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    public void Highlight()
    {
        gameObject.AddComponent<Outline>();
        Outline outline = GetComponent<Outline>();
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 3.0f;
    }


    public void UnHighLight()
    {
        Outline outline = GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
        }
        else
        {
            Debug.Log("Outline component not found on edge object.");
        }
    }
}
