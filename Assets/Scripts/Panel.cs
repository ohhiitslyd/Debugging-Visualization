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
        description = transform.Find("Canvas/Panel/description").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
