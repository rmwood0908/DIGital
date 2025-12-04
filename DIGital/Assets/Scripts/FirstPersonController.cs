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
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 10.0f;

    [Header("Camera Parameters")]
    [SerializeField] private float cameraSens = 0.1f;
    [SerializeField] private float heightRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform interactionSource;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerHandler playerHandler;

    private Vector3 currentMovement;
    private float verticalRotation;

    // Variable for when sprinting is added
    //private float currentSpeed => walkSpeed;

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

        if(!characterController.isGrounded)
        {
            currentMovement.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(currentMovement * Time.deltaTime);   
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
        
        Ray r = new Ray(interactionSource.position, interactionSource.forward);
        if(Physics.Raycast(r, out RaycastHit hitInfo, 5))
        {
            if(hitInfo.collider.gameObject.TryGetComponent(out Interactable interactObj))
            {
                interactObj.displayTooltip();
                if(playerHandler.InteractTriggered)
                {
                    interactObj.Interact();
                    playerHandler.InteractTriggered = false;
                }
            }
        }
    }
}
