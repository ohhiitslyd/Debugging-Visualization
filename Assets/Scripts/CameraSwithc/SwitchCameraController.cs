using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraController : MonoBehaviour
{
    public Camera mainCam;
    public Camera topCam;
    public float transitionDuration = 1.0f;

    private bool transitioning = false;
    private Camera currentCam;

    private void Start()
    {
        topCam.enabled = false;
        mainCam.enabled = true;
        currentCam = mainCam;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !transitioning)
        {
            Camera nextCam = (currentCam == mainCam) ? topCam : mainCam; 
            StartCoroutine(TransitionCamera(currentCam, nextCam));
        }
    }

    IEnumerator TransitionCamera(Camera fromCamera, Camera toCamera)
    {
        transitioning = true;
        float time = 0;
        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;

        while (time < transitionDuration)
        {
            fromCamera.transform.position = Vector3.Lerp(startPosition, toCamera.transform.position, time / transitionDuration);
            fromCamera.transform.rotation = Quaternion.Lerp(startRotation, toCamera.transform.rotation, time / transitionDuration);
            time += Time.deltaTime;
            yield return null;
        }

        fromCamera.transform.position = toCamera.transform.position;
        fromCamera.transform.rotation = toCamera.transform.rotation;

        fromCamera.enabled = false;
        toCamera.enabled = true;

        transitioning = false;
        currentCam = toCamera;
    }
}
