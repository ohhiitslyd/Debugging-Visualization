using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public static Panel Instance;
    public GameObject panel;
    public TMP_InputField title;
    public TMP_InputField address;
    public TMP_InputField description;
    public TMP_InputField successorsText;
    public TMP_InputField predecessorsText;
    public GameObject rotateIcon;


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        title = transform.Find("Canvas/Panel/title").GetComponent<TMP_InputField>();
        address = transform.Find("Canvas/Panel/address").GetComponent<TMP_InputField>();
        description = transform.Find("Canvas/Panel/Scroll View/Viewport/Content/basic block/basic block text body").GetComponent<TMP_InputField>();
        successorsText = transform.Find("Canvas/Panel/Scroll View/Viewport/Content/successors/successors text").GetComponent<TMP_InputField>();
        predecessorsText = transform.Find("Canvas/Panel/Scroll View/Viewport/Content/predecessors/predecessors text").GetComponent<TMP_InputField>();
        rotateIcon = transform.Find("Canvas/Panel/RotateIcon").gameObject;
        rotateIcon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
