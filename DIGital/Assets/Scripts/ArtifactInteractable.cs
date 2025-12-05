using UnityEngine;

// class for artifacts to be able to be clicked
public class ArtifactInteractable : MonoBehaviour
{
    [Tooltip("Optional: an ID or label for this artifact.")]
    public string artifactId;

    [Tooltip("Reference to the ArtifactFormManager in the scene.")]
    public ArtifactFormManager formManager;

    private void OnMouseDown()
    {
        // this is called when you click on this object in Game view
        // (requires a Collider on this GameObject and an active Camera)
        if (formManager == null)
        {
            Debug.LogWarning(
                "[ArtifactInteractable] formManager is not assigned on " 
                + gameObject.name
            );
            return;
        }

        formManager.OpenForm();
    }
}