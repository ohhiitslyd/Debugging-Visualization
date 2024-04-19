using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GraphTemp : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera firstCam;
    [SerializeField] CinemachineVirtualCamera secondCam;

    private void OnEnable()
    {
        CameraSwitcher.Register(firstCam);
        CameraSwitcher.Register(secondCam);

        CameraSwitcher.SwitchCamera(firstCam);
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(firstCam);
        CameraSwitcher.Unregister(secondCam);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // switch camera
            if(CameraSwitcher.IsActiveCamera(secondCam))
            {
                CameraSwitcher.SwitchCamera(firstCam);
            }
            else if(CameraSwitcher.IsActiveCamera(firstCam))
            {
                CameraSwitcher.SwitchCamera(secondCam);
            }
        }
    }
}
