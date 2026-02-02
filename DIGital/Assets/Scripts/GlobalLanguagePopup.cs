using UnityEngine;

public class GlobalLanguagePopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;

    [Header("Optional - disable these while popup is open")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    [Header("Pause gameplay while open?")]
    [SerializeField] private bool pauseWithTimeScale = false;

    private bool isOpen;
    private CursorLockMode prevLock;
    private bool prevVisible;
    private float prevTimeScale;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (panelRoot == null) return;

        isOpen = true;

        // save previous state
        prevLock = Cursor.lockState;
        prevVisible = Cursor.visible;
        prevTimeScale = Time.timeScale;

        // show UI
        panelRoot.SetActive(true);

        // disable gameplay scripts that fight cursor / input
        if (disableWhileOpen != null)
        {
            foreach (var mb in disableWhileOpen)
                if (mb != null) mb.enabled = false;
        }

        // cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // optional pause
        if (pauseWithTimeScale)
            Time.timeScale = 0f;
    }

    public void Close()
    {
        if (panelRoot == null) return;

        isOpen = false;

        panelRoot.SetActive(false);

        // re-enable gameplay scripts
        if (disableWhileOpen != null)
        {
            foreach (var mb in disableWhileOpen)
                if (mb != null) mb.enabled = true;
        }

        // restore previous cursor state
        Cursor.lockState = prevLock;
        Cursor.visible = prevVisible;

        // restore time
        if (pauseWithTimeScale)
            Time.timeScale = prevTimeScale;
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
