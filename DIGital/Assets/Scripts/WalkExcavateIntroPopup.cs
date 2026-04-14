using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalkExcavateIntroPopup : MonoBehaviour
{
    [Header("Popup References")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Button continueButton;

    [Header("Player References")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableWhileOpen;

    [Header("Cursor")]
    [SerializeField] private bool unlockCursorWhileOpen = true;

    private void Start()
    {
        // open popup
        OpenPopup();

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ClosePopup);
        }
    }

    public void OpenPopup()
    {
        // show poup
        if (popupRoot != null)
        {
            popupRoot.SetActive(true);
        }

        // disable movement
        foreach (var script in scriptsToDisableWhileOpen)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // unlock cursor
        if (unlockCursorWhileOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // freeze
        Time.timeScale = 0f;
    }

    public void ClosePopup()
    {
        // hide popup
        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
        }

        // enable movement
        foreach (var script in scriptsToDisableWhileOpen)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // lock cursor
        if (unlockCursorWhileOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // unfreeze
        Time.timeScale = 1f;
    }
}