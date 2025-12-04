using UnityEngine;
using UnityEngine.EventSystems;

public class ArtifactDragRotate : MonoBehaviour
{
    [SerializeField] private ArtifactAnalysisManager analysisManager;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.3f;

    // pivot the models are parented under
    [SerializeField] private Transform rotationPivot;

    // how far up/down we allow tilting (in degrees)
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Zoom (scales the pivot)")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoomScale = 0.5f;
    [SerializeField] private float maxZoomScale = 2.0f;

    private bool _dragging = false;
    private Vector3 _lastMousePos;

    // track how far tilted up/down we are so we can clamp it
    private float _currentVerticalAngle = 0f;

    // track base scale and current zoom factor
    private Vector3 _baseScale = Vector3.one;
    private float _currentZoom = 1f;

    private void Start()
    {
        // remember the original scale of the pivot
        if (rotationPivot != null)
        {
            _baseScale = rotationPivot.localScale;
        }
    }

    private void Update()
    {
        // ignore if we don't have a manager
        if (analysisManager == null)
        {
            return;
        }

        // don't rotate/zoom when over UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            _dragging = false;
            return;
        }

        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragging = true;
            _lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
        }

        if (!_dragging)
        {
            return;
        }

        Transform target =
            rotationPivot != null
                ? rotationPivot
                : analysisManager.CurrentActiveModel != null
                    ? analysisManager.CurrentActiveModel.transform
                    : null;

        if (target == null)
        {
            return;
        }

        Vector3 delta = Input.mousePosition - _lastMousePos;

        // -------- YAW (left/right) --------
        float yaw = -delta.x * rotationSpeed;
        target.Rotate(Vector3.up, yaw, Space.World);

        // -------- PITCH (up/down) with clamp --------
        float pitchDelta = delta.y * rotationSpeed;

        float newVerticalAngle =
            Mathf.Clamp(_currentVerticalAngle + pitchDelta,
                        -maxVerticalAngle,
                        maxVerticalAngle);

        float clampedDelta = newVerticalAngle - _currentVerticalAngle;
        _currentVerticalAngle = newVerticalAngle;

        target.Rotate(target.right, clampedDelta, Space.World);

        _lastMousePos = Input.mousePosition;
    }

    private void HandleZoom()
    {
        if (rotationPivot == null)
        {
            return;
        }

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) < 0.001f)
        {
            return;
        }

        // scroll up → zoom in (larger scale)
        _currentZoom = Mathf.Clamp(
            _currentZoom + scroll * zoomSpeed,
            minZoomScale,
            maxZoomScale
        );

        rotationPivot.localScale = _baseScale * _currentZoom;
    }
}