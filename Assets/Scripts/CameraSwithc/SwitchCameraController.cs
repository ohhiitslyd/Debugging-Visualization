using System.Collections;
using UnityEngine;

public class SwitchCameraController : MonoBehaviour
{
    private Camera camera;
    private bool transitioning = false;
    private bool isTopView = false;
    private float transitionDuration = 1f;


    private void Start()
    {
        camera = Camera.main.GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !transitioning)
        {
            Debug.Log("going to switch!");
            StartCoroutine(TransitionCamera());
        }
    }

    IEnumerator TransitionCamera()
    {
        transitioning = true;
        float time = 0;

        Quaternion startRotation = camera.transform.rotation;
        Quaternion endRotation = isTopView ? Quaternion.Euler(30f, -15f, 0f) : Quaternion.Euler(90f, 0f, 0f);

        while (time <= transitionDuration)
        {
            float t = time / transitionDuration;
            t = t * t * (3f - 2f * t); 
            camera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            time += Time.deltaTime;
            yield return null;
        }

        camera.transform.rotation = endRotation;
        camera.gameObject.GetComponent<CameraController>().SetCurrentTargetRotation(endRotation);
        transitioning = false;
        isTopView = !isTopView;
    }
}
