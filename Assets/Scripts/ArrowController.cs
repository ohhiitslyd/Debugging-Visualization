using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Material highlightMat;
    public Material material;

    public void SetMaterial(Material mat)
    {
        material = mat;
        UnHighLight();
    }

    public void Highlight()
    {
        //gameObject.AddComponent<Outline>();
        //Outline outline = GetComponent<Outline>();
        //outline.OutlineColor = Color.yellow;
        //outline.OutlineWidth = 3.0f;


        Renderer[] childrenRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in childrenRenderers)
        {
            rend.material = highlightMat;
        }
    }


    public void UnHighLight()
    {
        //Outline outline = GetComponent<Outline>();
        //if (outline != null)
        //{
        //    Destroy(outline);
        //}
        //else
        //{
        //    Debug.Log("Outline component not found on edge object.");
        //}

        Renderer[] childrenRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in childrenRenderers)
        {
            rend.material = material;
        }
    }
}
