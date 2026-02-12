using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class GlobalLanguagePopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;

    [Header("Pause gameplay while open?")]
    [SerializeField] private bool pauseWithTimeScale = false;

    // runtime disabled scripts
    private readonly List<MonoBehaviour> disabledRuntime = new();

    private bool isOpen;
    private CursorLockMode prevLock;
    private bool prevVisible;
    private float prevTimeScale;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    // updated update function to disable menu popup 
    // when entering artifact information
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.M)) return;

        // if the player is typing in any TMP_InputField, ignore the hotkey
        if (IsTypingInInputField()) return;

        Toggle();
    }

    private bool IsTypingInInputField()
    {
        if (EventSystem.current == null) return false;

        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return false;

        // if the selected object is a TMP_InputField or inside one, treat as typing
        if (selected.GetComponentInParent<TMP_InputField>() != null) return true;

        // ADDED for AI assistant panel
        if (selected.GetComponentInParent<InputField>() != null) return true;

        // otherwise
        return false;
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (panelRoot == null) return;
        if (isOpen) return;

        isOpen = true;

        // save previous state
        prevLock = Cursor.lockState;
        prevVisible = Cursor.visible;
        prevTimeScale = Time.timeScale;

        // show UI
        panelRoot.SetActive(true);

        // disable player/camera scripts in THIS scene
        DisableGameplayForPopup();

        // Cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // optional pause
        if (pauseWithTimeScale)
            Time.timeScale = 0f;
    }

    public void Close()
    {
        if (panelRoot == null) return;
        if (!isOpen) return;

        isOpen = false;

        panelRoot.SetActive(false);

        // Re-enable what we disabled
        RestoreGameplayAfterPopup();

        // restore cursor state
        Cursor.lockState = prevLock;
        Cursor.visible = prevVisible;

        // restore time
        if (pauseWithTimeScale)
            Time.timeScale = prevTimeScale;
    }

    private void DisableGameplayForPopup()
    {
        disabledRuntime.Clear();

        // disable FirstPersonController (player movement/look)
        var fps = FindFirstObjectByTypeSafe<FirstPersonController>();
        if (fps != null && fps.enabled)
        {
            fps.enabled = false;
            disabledRuntime.Add(fps);
        }

        // 2) disable MoveCamera (camera look script)
        var camMove = FindFirstObjectByTypeSafe<MoveCamera>();
        if (camMove != null && camMove.enabled)
        {
            camMove.enabled = false;
            disabledRuntime.Add(camMove);
        }
    }

    private void RestoreGameplayAfterPopup()
    {
        for (int i = 0; i < disabledRuntime.Count; i++)
        {
            if (disabledRuntime[i] != null)
                disabledRuntime[i].enabled = true;
        }
        disabledRuntime.Clear();
    }

    private T FindFirstObjectByTypeSafe<T>() where T : Object
    {
#if UNITY_6000_0_OR_NEWER

        var found = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
        return (found != null && found.Length > 0) ? found[0] : null;
#else
        return Object.FindObjectOfType<T>();
#endif
    }

    public void ChooseEnglish()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage("en");
        Close();
    }

    public void ChooseSpanish()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage("es");
        Close();
    }
}
