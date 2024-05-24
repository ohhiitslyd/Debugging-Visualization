using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildClickEvent : MonoBehaviour
{
    [SerializeField] NodeGameObject nodeGameObject;
    void OnMouseDown()
    {
        nodeGameObject.Click();
    }
}
