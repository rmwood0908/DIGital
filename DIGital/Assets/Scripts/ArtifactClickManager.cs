using UnityEngine;

// class
public class ArtifactClickManager : MonoBehaviour
{
    [Header("Camera & Raycast")]
    public Camera playerCamera;
    public float maxDistance = 100f;

    [Tooltip("Layers that contain clickable artifacts only.")]
    public LayerMask artifactLayerMask;

    [Tooltip("Layers that represent dirt/soil that should block clicking artifacts.")]
    public LayerMask dirtLayerMask;

    private void Update()
    {
        // left mouse button (0) click
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // find closest artifact hit
        if (!Physics.Raycast(ray, out RaycastHit artifactHit, maxDistance, artifactLayerMask)) return;

        // check if dirt blocks artifact
        if (Physics.Raycast(ray, out RaycastHit dirtHit, maxDistance, dirtLayerMask))
        {
            if (dirtHit.distance < artifactHit.distance)
            {
                return;
            }
        }

        // interact
        var interactable = artifactHit.collider.GetComponent<ArtifactInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
        }
    }
}
