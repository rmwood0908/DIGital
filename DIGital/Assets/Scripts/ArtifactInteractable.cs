using TMPro;
using UnityEngine;

public class ArtifactInteractable : MonoBehaviour, Interactable
{
    [Header("UI / Form")]
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private ArtifactFormManager formManager;

    [Header("Tooltip Text")]
    [SerializeField] private string tooltipText = "Record artifact (Left Click)";

    private float textDisplayedTime = 0f;
    private bool checkForText = false;

    public void Interact()
    {
        if (formManager != null)
        {
            Debug.Log("[ArtifactInteractable] Interact() called, opening form.");
            if (textBox != null)
            {
                textBox.text = "";
            }
            formManager.OpenForm();
        }
        else
        {
            Debug.LogWarning("[ArtifactInteractable] formManager is not assigned on "
                             + gameObject.name);
        }
    }

    public void displayTooltip()
    {
        // Called by FirstPersonController when this is the target
        textDisplayedTime = 0.03f;
        checkForText = true;
    }

    private void FixedUpdate()
    {
        if (!checkForText)
        {
            return;
        }

        if (textDisplayedTime > 0f)
        {
            if (textBox != null)
            {
                textBox.text = tooltipText;
            }
        }
        else
        {
            if (textBox != null)
            {
                textBox.text = "";
            }
            checkForText = false;
        }

        textDisplayedTime -= Time.deltaTime;
    }
}