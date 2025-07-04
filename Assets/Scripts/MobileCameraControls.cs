using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileCameraControls : MonoBehaviour
{
    [SerializeField] private Camera cam;

    public float panSpeed = 0.5f;
    public float zoomSpeed = 0.5f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 20f;

    private Vector3 lastPanPosition;
    private int panFingerId;
    private bool isPanning;

    private float lastPinchDistance;
    private bool isZooming;

    private Vector3 camOffset = new Vector3(4, 5, -4);
    public bool canPan = true;

    [SerializeField] private InputManager inputMan;
    private void Awake()
    {
        if(inputMan == null)
        {
            inputMan = FindObjectOfType<InputManager>();
        }
    }

    private void Update()
    {
#if UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#else
        HandleMouseInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            isPanning = false;
            isZooming = false;
            return;
        }

        if (Input.touchCount == 1)
        {
            if (!canPan)
                return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && inputMan.IsPointerOverUI()) return;

                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
                isPanning = true;
            }
            else if (touch.fingerId == panFingerId && isPanning)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isPanning = false;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

            if (!isZooming)
            {
                lastPinchDistance = currentDistance;
                isZooming = true;
            }
            else
            {
                float distanceDelta = currentDistance - lastPinchDistance;
                //ZoomCamera(distanceDelta);
                lastPinchDistance = currentDistance;
            }
        }
    }
    private void HandleMouseInput()
    {
        if (!canPan) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && inputMan.IsPointerOverUI()) return;

            lastPanPosition = Input.mousePosition;
            isPanning = true;
        }
        else if(Input.GetMouseButton(0) && isPanning)
        {
            PanCamera(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(Mathf.Abs(scroll) > 0.01f)
        {
            ZoomCamera(scroll * 10f);
        }
    }

    private void PanCamera(Vector3 newPanPosition)
    {
        Vector3 delta = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);

        // Move along camera right (local X) and forward projected on XZ plane (local Z without Y)
        Vector3 right = cam.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up); // flattened forward direction

        Vector3 move = (right * delta.x + forward * delta.y) * panSpeed;

        cam.transform.position += move;

        lastPanPosition = newPanPosition;
    }

    private void ZoomCamera(float deltaMagnitude)
    {
        Vector3 direction = cam.transform.forward;
        Vector3 newPosition = cam.transform.position + direction * deltaMagnitude * zoomSpeed;

        float distance = Vector3.Distance(newPosition, Vector3.zero); // assuming you want to keep focus around world origin

        if (distance > minZoomDistance && distance < maxZoomDistance)
        {
            cam.transform.position = newPosition;
        }
    }
}
