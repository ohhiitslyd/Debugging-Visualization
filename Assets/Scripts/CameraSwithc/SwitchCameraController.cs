using System.Collections;
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
            currentCam = nextCam;
        }
    }

    IEnumerator TransitionCamera(Camera fromCamera, Camera toCamera)
    {
        transitioning = true;
        float time = 0;

        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;
        Vector3 endPosition = toCamera.transform.position;
        Quaternion endRotation = toCamera.transform.rotation;

        while (time <= transitionDuration)
        {
            float t = time / transitionDuration;
            t = t * t * (3f - 2f * t); 
            fromCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            fromCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            time += Time.deltaTime;
            yield return null;
        }


        Debug.Log("fromCam position: " + fromCamera.transform.position);
        Debug.Log("toCam position: " + toCamera.transform.position);
        Debug.Log("fromCam rotation: " + fromCamera.transform.rotation);
        Debug.Log("toCam rotation: " + toCamera.transform.rotation);

        fromCamera.transform.position = toCamera.transform.position;
        fromCamera.transform.rotation = toCamera.transform.rotation;


        fromCamera.enabled = false;
        toCamera.enabled = true;

        fromCamera.transform.position = startPosition;
        fromCamera.transform.rotation = startRotation;

        transitioning = false;
    }
}
