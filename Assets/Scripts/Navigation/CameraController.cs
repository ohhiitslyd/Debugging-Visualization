using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float navigationSpeed = 2.7f;
    [SerializeField] float shiftMultiplier = 2f; // Speed multiplier when Shift is pressed
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float minOrthographicSize = 5.5f;
    [SerializeField] float maxOrthographicSize = 50f;
    [SerializeField] float zoomLerpSpeed = 10f; // Speed at which zoom lerps
    [SerializeField] float keyboardPanSpeed = 5f; // Speed for WASD navigation

    [SerializeField] bool activeKeyboardMovement = false;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging;
    private float targetOrthographicSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Ensure the camera is orthographic and set it to an isometric angle
        cam.orthographic = true;
        transform.rotation = Quaternion.Euler(30f, -45f, 0f); // Adjust as needed

        targetOrthographicSize = cam.orthographicSize;
    }

    private void Update()
    {

        if(Input.GetKey(KeyCode.Space)){
            HandleDragMovement();
            HandleTrackedZoom();
        }


        if(activeKeyboardMovement){
            HandleKeyboardMovement();
        }

        // Smoothly interpolate towards the target orthographic size
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, Time.deltaTime * zoomLerpSpeed);
    }

    private void HandleDragMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference;
        }
    }

    private void HandleKeyboardMovement()
    {
        float speed = keyboardPanSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f) * Time.deltaTime;

        Vector3 movement = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            movement += new Vector3(1, 1, 0).normalized * speed; // Forward (top-right in isometric view)
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += new Vector3(-1, -1, 0).normalized * speed; // Backward (bottom-left in isometric view)
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += new Vector3(-1, 1, 0).normalized * speed; // Left (top-left in isometric view)
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += new Vector3(1, -1, 0).normalized * speed; // Right (bottom-right in isometric view)
        }

        transform.position += movement;
    }

    private void HandleTrackedZoom()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (scrollData != 0.0f)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPosBeforeZoom = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize - scrollData * zoomSpeed, minOrthographicSize, maxOrthographicSize);

            Vector3 worldPosAfterZoom = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            Vector3 offset = worldPosBeforeZoom - worldPosAfterZoom;

            transform.position += offset;
        }
    }
}
