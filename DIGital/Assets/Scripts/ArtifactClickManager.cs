using UnityEngine;

// class
public class ArtifactClickManager : MonoBehaviour
{
    [Header("Camera & Raycast")]
    public Camera playerCamera;
    public float maxDistance = 100f;

    [Tooltip("Layers that contain clickable artifacts only.")]
    public LayerMask artifactLayerMask;

    private void Update()
    {
        // left mouse button (0) click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, artifactLayerMask))
            {
                var interactable = hit.collider.GetComponent<ArtifactInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
