using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    public Panel panel;
    private CameraController cameraController;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ClosePanel();
        }

        if (Input.GetKeyUp(KeyCode.R)){
          ToggleAnalyzeMode();
        }
    }

    public void ClosePanel()
    {
        if (StateManager.Instance.currentCameraState == StateManager.CameraState.Analyze)
        {
            cameraController.ExitAnalyzeMode();
        }
        panel.panel.SetActive(false);
        StateManager.Instance.selectedBlock = -1;
        StateManager.Instance.UpdateMaterials();
        cameraController.ResetFocus();
    }

    public void ToggleAnalyzeMode()
    {
        if (StateManager.Instance.currentCameraState == StateManager.CameraState.Analyze)
        {
            cameraController.ExitAnalyzeMode();
            panel.rotateIcon.SetActive(false);
        }
        else if (StateManager.Instance.currentCameraState == StateManager.CameraState.Focus)
        {
            cameraController.EnterAnalyzeMode();
            panel.rotateIcon.SetActive(true);
        }
    }
}
