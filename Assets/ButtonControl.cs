using UnityEngine;

public class ButtonControl : MonoBehaviour
{
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
        Panel.Instance.panel.SetActive(false);
        StateManager.Instance.selectedBlock = -1;
        StateManager.Instance.UpdateMaterials();
        cameraController.ResetFocus();
    }

    public void ToggleAnalyzeMode()
    {
        if (StateManager.Instance.currentCameraState == StateManager.CameraState.Analyze)
        {
            cameraController.ExitAnalyzeMode();
            Panel.Instance.rotateIcon.SetActive(false);
        }
        else if (StateManager.Instance.currentCameraState == StateManager.CameraState.Focus)
        {
            cameraController.EnterAnalyzeMode();
            Panel.Instance.rotateIcon.SetActive(true);
        }
    }
}
