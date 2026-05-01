using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

interface Interactable
{
    public void Interact();
    public void displayTooltip();
}

public class FirstPersonController : MonoBehaviour
{
    public bool IsUnitMarkingMode { get; set; } = false;

    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 10.0f;

    [Header("Camera Parameters")]
    [SerializeField] private float cameraSens = 0.1f;
    [SerializeField] private float heightRange = 80f;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 3f;

    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float climbDetectDistance = 0.5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform interactionSource;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerHandler playerHandler;

    private Vector3 currentMovement;
    private float verticalRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleInteraction();
    }

    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerHandler.MovementInput.x, 0f, playerHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * walkSpeed;
        currentMovement.z = worldDirection.z * walkSpeed;

        if (characterController.isGrounded)
        {
            if (currentMovement.y < 0) currentMovement.y = -2f; // small constant keeps grounded flag stable

            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * Time.deltaTime;
        }

        // Wall climb only if not already jumping upward
        if (IsTouchingClimbableWall() && playerHandler.MovementInput.magnitude > 0.1f && currentMovement.y <= 0)
        {
            currentMovement.y = climbSpeed;
        }

        characterController.Move(currentMovement * Time.deltaTime);
    }

    private bool IsTouchingClimbableWall()
    {
        Vector3 moveDir = CalculateWorldDirection();
        if (moveDir.magnitude < 0.1f) return false;

        // Cast from slightly above the player's feet so we detect walls not floors
        Vector3 rayOrigin = transform.position + Vector3.up * 0.3f;
        Ray ray = new Ray(rayOrigin, moveDir);

        if (Physics.Raycast(ray, out RaycastHit hit, characterController.radius + climbDetectDistance))
        {
            if (hit.collider.gameObject != gameObject)
                return true;
        }
        return false;
    }

    private void HorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void VerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -heightRange, heightRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerHandler.RotationInput.x * cameraSens;
        float mouseYRotation = playerHandler.RotationInput.y * cameraSens;

        HorizontalRotation(mouseXRotation);
        VerticalRotation(mouseYRotation);
    }

    private void HandleInteraction()
    {
        if (IsUnitMarkingMode) return;
        if (mainCamera == null) return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // 1) prefer dirt (DiggableEarth) if present
            DiggableEarth dirt = hit.collider.GetComponentInParent<DiggableEarth>();
            if (dirt != null)
            {
                if (Input.GetMouseButtonDown(0)) dirt.Interact();
                else dirt.displayTooltip();
                return;
            }

            // 2) else, try artifact
            ArtifactInteractable artifact = hit.collider.GetComponentInParent<ArtifactInteractable>();
            if (artifact != null)
            {
                if (Input.GetMouseButtonDown(0)) artifact.Interact();
                else artifact.displayTooltip();
                return;
            }

            // 3) generic Interactable fallback
            Interactable generic = hit.collider.GetComponentInParent<Interactable>();
            if (generic != null)
            {
                if (Input.GetMouseButtonDown(0)) generic.Interact();
                else generic.displayTooltip();
            }
        }
    }
}