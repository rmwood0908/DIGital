using UnityEngine;

// class to lock cursor in analysis scene
public class AnalysisSceneController : MonoBehaviour
{
    [Header("Optional: FPS controller to disable in analysis scene")]
    [SerializeField] private FirstPersonController fpsController;

    private void Start()
    {
        // UI mode: show and unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (fpsController != null)
        {
            fpsController.enabled = false;
        }
    }
}
