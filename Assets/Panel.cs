using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Panel : MonoBehaviour
{
    public static Panel Instance;
    public GameObject panel;
    public TextMeshProUGUI title;
    public TextMeshProUGUI address;
    public TextMeshProUGUI description;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
