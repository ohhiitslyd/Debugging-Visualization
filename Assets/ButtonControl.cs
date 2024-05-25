using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    public Panel panel;
    private CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        panel.panel.SetActive(false);
        StateManager.Instance.selectedBlock = -1;
        StateManager.Instance.UpdateMaterials();
        cameraController.ResetFocus();
    }
}
