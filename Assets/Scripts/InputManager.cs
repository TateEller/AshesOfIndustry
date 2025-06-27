using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayermask;

    public event Action<Vector3> OnStartPlacement;
    public event Action<Vector3> OnDragPlacement;
    public event Action<Vector3> OnEndPlacement;
    private bool isDragging = false;

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Vector3 pos = GetWorldPosition(Input.mousePosition);
            Debug.Log("[InputManager] Mouse down at world pos " + pos);
            OnStartPlacement?.Invoke(pos);
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 pos = GetWorldPosition(Input.mousePosition);
            OnEndPlacement?.Invoke(pos);
            isDragging = false;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 pos = GetWorldPosition(Input.mousePosition);
            OnDragPlacement?.Invoke(pos);
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            if (touch.phase == TouchPhase.Began && !IsPointerOverUI(touch.fingerId))
            {
                Vector3 pos = GetWorldPosition(touchPos);
                OnStartPlacement?.Invoke(pos);
                isDragging = true;
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isDragging)
            {
                Vector3 pos = GetWorldPosition(touchPos);
                OnEndPlacement?.Invoke(pos);
                isDragging = false;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 pos = GetWorldPosition(touchPos);
                OnDragPlacement?.Invoke(pos);
            }
        }
#endif
    }

    public Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 screenPos = new Vector3(screenPosition.x, screenPosition.y, sceneCamera.nearClipPlane);
        Ray ray = sceneCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, placementLayermask))
        {
            Debug.Log("[InputManager] Raycast HIT " + hit.collider.name + " at " + hit.point);
            return hit.point;
        }
        Debug.Log("[InputManger] Raycast MISSED");
        return lastPosition;
    }

    public bool IsPointerOverUI()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
#endif
        return EventSystem.current.IsPointerOverGameObject();
    }
}
