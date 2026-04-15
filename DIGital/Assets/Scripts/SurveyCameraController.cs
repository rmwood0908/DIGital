using UnityEngine;

public class SurveyCameraController : MonoBehaviour
{
    [Header("Rig References")]
    [SerializeField] private Transform focusRoot;
    [SerializeField] private Transform pitchPivot;
    [SerializeField] private Camera surveyCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float boostMultiplier = 1.75f;

    [Header("Site Bounds")]
    [SerializeField] private bool clampToBounds = false;
    [SerializeField] private Vector2 xBounds = new Vector2(-50f, 50f);
    [SerializeField] private Vector2 zBounds = new Vector2(-50f, 50f);

    [Header("Yaw / Pitch")]
    [SerializeField] private float yawAngle = 0f;
    [SerializeField] private float yawSpeed = 90f;
    [SerializeField] private float pitchAngle = 55f;
    [SerializeField] private float pitchSpeed = 45f;
    [SerializeField] private float minPitch = 35f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Zoom")]
    [SerializeField] private float zoomDistance = 18f;
    [SerializeField] private float zoomSpeed = 8f;
    [SerializeField] private float minZoom = 6f;
    [SerializeField] private float maxZoom = 30f;

    [Header("Keys")]
    [SerializeField] private KeyCode exitKey = KeyCode.Space;
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.Q;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.E;
    [SerializeField] private KeyCode tiltMoreTopDownKey = KeyCode.R;
    [SerializeField] private KeyCode tiltMoreShallowKey = KeyCode.F;

    [SerializeField] private ArtifactFormManager artifactFormManager;

    private SurveyModeManager owner;
    private float fixedY;

    private void OnEnable()
    {
        if (focusRoot != null)
        {
            fixedY = focusRoot.position.y;
        }

        ApplyView();
    }

    public void BeginSurveyMode(SurveyModeManager modeManager)
    {
        owner = modeManager;

        if (focusRoot != null)
        {
            fixedY = focusRoot.position.y;
        }

        ApplyView();
    }

    public void SetFocusPoint(Vector3 worldPosition)
    {
        if (focusRoot == null)
        {
            return;
        }

        worldPosition.y = focusRoot.position.y;
        focusRoot.position = worldPosition;
    }

    private void Update()
    {
        if (artifactFormManager != null && artifactFormManager.IsFormOpen)
        {
            return;
        }

        HandleExit();
        HandleMovement();
        HandleYawAndPitch();
        HandleZoom();
        ApplyView();
    }

    private void HandleExit()
    {
        if (Input.GetKeyDown(exitKey))
        {
            if (owner != null)
            {
                owner.ExitSurveyMode();
            }
        }
    }

    // lateral movement
    private void HandleMovement()
    {
        if (focusRoot == null)
        {
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal -= 1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal += 1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            vertical += 1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            vertical -= 1f;
        }

        Vector3 forward = focusRoot.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = focusRoot.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDirection = (forward * vertical) + (right * horizontal);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed *= boostMultiplier;
        }

        Vector3 nextPosition = focusRoot.position + moveDirection * speed * Time.deltaTime;
        nextPosition.y = fixedY;

        if (clampToBounds)
        {
            nextPosition.x = Mathf.Clamp(nextPosition.x, xBounds.x, xBounds.y);
            nextPosition.z = Mathf.Clamp(nextPosition.z, zBounds.x, zBounds.y);
        }

        focusRoot.position = nextPosition;
    }

    // rotation (Yaw = Y and Pitch = X)
    private void HandleYawAndPitch()
    {
        float yawInput = 0f;

        if (Input.GetKey(rotateLeftKey))
        {
            yawInput -= 1f;
        }

        if (Input.GetKey(rotateRightKey))
        {
            yawInput += 1f;
        }

        yawAngle += yawInput * yawSpeed * Time.deltaTime;

        float pitchInput = 0f;

        // R = more top-down
        if (Input.GetKey(tiltMoreTopDownKey))
            pitchInput += 1f;

        // F = more shallow / lower angle
        if (Input.GetKey(tiltMoreShallowKey))
            pitchInput -= 1f;

        pitchAngle += pitchInput * pitchSpeed * Time.deltaTime;
        pitchAngle = Mathf.Clamp(pitchAngle, minPitch, maxPitch);
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.001f)
        {
            zoomDistance -= scroll * zoomSpeed;
            zoomDistance = Mathf.Clamp(zoomDistance, minZoom, maxZoom);
        }
    }

    private void ApplyView()
    {
        if (focusRoot == null || pitchPivot == null || surveyCamera == null)
        {
            return;
        }

        focusRoot.rotation = Quaternion.Euler(0f, yawAngle, 0f);
        pitchPivot.localRotation = Quaternion.Euler(pitchAngle, 0f, 0f);

        surveyCamera.transform.localPosition = new Vector3(0f, 0f, -zoomDistance);
        surveyCamera.transform.localRotation = Quaternion.identity;
    }
}