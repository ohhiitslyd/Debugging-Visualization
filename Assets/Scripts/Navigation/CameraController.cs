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
    [SerializeField] float focusLerpSpeed = 9f; // Speed at which focus lerps
    [SerializeField] bool activeKeyboardMovement = false; // Add this line
    [SerializeField] float blockRotationSpeed = 6f; // Add this line


    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging;
    private float targetOrthographicSize;
    private Vector3 targetPosition;
    private bool isFocusing = false; // used by camera to understand when to lerp certain controls
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private float analyzeYRotation; // Store the Y rotation separately

    private Vector3 originalPosition;
    private float originalOrthographicSize;

    public Quaternion IsometricCameraAngles = Quaternion.Euler(30f, -15f, 0f);

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Ensure the camera is orthographic and set it to an isometric angle
        cam.orthographic = true;
        transform.rotation = IsometricCameraAngles; // Adjust as needed

        originalPosition = transform.position;
        originalOrthographicSize = cam.orthographicSize;
        targetOrthographicSize = cam.orthographicSize;
        targetPosition = transform.position;

        originalRotation = transform.rotation;
        targetRotation = transform.rotation;
        analyzeYRotation = transform.eulerAngles.y; // Initialize analyze rotation
    }

    private void Start()
    {
        if (StateManager.Instance == null)
        {
            Debug.LogError("StateManager.Instance is not initialized in Start().");
        }
    }

    private void Update()
    {
        if (StateManager.Instance == null)
        {
            Debug.LogError("StateManager.Instance is not initialized in Update().");
            // Exit the method if StateManager is not yet initialized
            return;
        }

        switch (StateManager.Instance.currentCameraState)
        {
            case StateManager.CameraState.Focus:
                HandleFocusState();
                // HandleDefaultState();
                break;
            case StateManager.CameraState.Analyze:
                HandleAnalyzeRotation();
                break;
            case StateManager.CameraState.Default:
            default:
                HandleDefaultState();
                break;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * focusLerpSpeed);
    }

    private void HandleFocusState()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * focusLerpSpeed);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, Time.deltaTime * focusLerpSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f && Mathf.Abs(cam.orthographicSize - targetOrthographicSize) < 0.01f && Quaternion.Angle(transform.rotation, targetRotation) < 0.01f)
        {
            isFocusing = false;
        }
    }

    private void HandleAnalyzeRotation()
    {
        float horizontalInput = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(0))
        {
            float rotationAmount = horizontalInput * blockRotationSpeed;
            analyzeYRotation += rotationAmount;

            //targetRotation = Quaternion.Euler(originalRotation.eulerAngles.x, analyzeYRotation, originalRotation.eulerAngles.z); // Update target rotation only on Y axis
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, analyzeYRotation, targetRotation.eulerAngles.z);
        }
    }

    private void HandleDefaultState()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            HandleDragMovement();
            HandleTrackedZoom();
        }

        if (activeKeyboardMovement) // Check for keyboard movement
        {
            HandleKeyboardMovement();
        }

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
            targetPosition = transform.position; // Update target position during drag
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
        targetPosition = transform.position; // Update target position during keyboard movement
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
            targetPosition = transform.position; // Update target position during zoom
        }
    }

    public void FocusOnBlock(Vector3 blockPosition)
    {
        originalPosition = transform.position;
        originalOrthographicSize = cam.orthographicSize;

        targetPosition = blockPosition;
        targetOrthographicSize = 8f; // Fixed orthographic size for focusing on blocks
        isFocusing = true;

        StateManager.Instance.SetCameraState(StateManager.CameraState.Focus);
    }

    public void ResetFocus()
    {
        targetPosition = originalPosition;
        targetOrthographicSize = originalOrthographicSize;
        targetRotation = originalRotation; // Reset rotation to original
        isFocusing = true;

        StateManager.Instance.SetCameraState(StateManager.CameraState.Default);
    }

    public void EnterAnalyzeMode()
    {
        analyzeYRotation = transform.eulerAngles.y; // Store the current Y rotation as the starting analyze Y rotation
        StateManager.Instance.SetCameraState(StateManager.CameraState.Analyze);
    }

    public void ExitAnalyzeMode()
    {
        StateManager.Instance.SetCameraState(StateManager.CameraState.Focus);
        targetRotation = originalRotation; // Reset rotation to original
    }

    public void SetCurrentTargetRotation(Quaternion quaternion)
    {
        targetRotation = quaternion;
    }
}
