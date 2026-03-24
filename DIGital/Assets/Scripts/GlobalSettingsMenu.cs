using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class GlobalSettingsMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsRoot;
    [SerializeField] private GameObject mainSettingsPanel;
    [SerializeField] private GameObject languagePanel;

    [Header("Scene Names")]
    [SerializeField] private string accountManagementSceneName = "AccountManagement";
    [SerializeField] private string siteSelectionSceneName = "SiteSelection";

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    [Header("Pause gameplay while open?")]
    [SerializeField] private bool pauseWithTimeScale = false;

    // singleton instance
    public static GlobalSettingsMenu Instance { get; private set; }

    // popup state
    private bool isOpen;
    private CursorLockMode prevLock;
    private bool prevVisible;
    private float prevTimeScale;

    // runtime disabled scripts
    private readonly List<MonoBehaviour> disabledRuntime = new();

    private void Awake()
    {
        // prevent duplicate copies
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // store instance so it persists between scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // list for changes so menu can reset
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // stop listening
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // set intial behavior and start with menu closed
        ApplySceneMode(SceneManager.GetActiveScene().name);
        CloseAll();
    }

    private void Update()
    {
        // toggle menu with hotkey
        if(!Input.GetKeyDown(toggleKey))
        {
            return;
        }

        // do not open the menu if the user is typing in an input field
        if (IsTypingInInputField()) 
        {
            return;
        }

        ToggleMenu();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // re-apply scene behavior when a new scene loads
        // start with meny closed
        ApplySceneMode(scene.name);
        CloseAll();
    }

    private bool IsAccountManagementScene()
    {
        return SceneManager.GetActiveScene().name == accountManagementSceneName;
    }

    // prepares menu for different scenes (account vs. everything else)
    private void ApplySceneMode(string sceneName)
    {
        bool isAccountScene = sceneName == accountManagementSceneName;

        if ( settingsRoot == null || mainSettingsPanel == null || languagePanel == null )
        {
            Debug.LogWarning("[GlobalSettingsMenu] One or more panel references are missing.");
            return;
        }

        mainSettingsPanel.SetActive(false);
        languagePanel.SetActive(false);
        settingsRoot.SetActive(false);
    }

    // checks whether the user is currently typing in a TMP input field
    private bool IsTypingInInputField()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) 
        { 
            return false;
        }

        if (selected.GetComponentInParent<TMP_InputField>() != null) 
        {
            return true;
        }

        if (selected.GetComponentInParent<InputField>() != null) 
        {
            return true;
        }

        return false;
    }

    // opens/closes menu and follows current account scene behavior
    public void ToggleMenu()
    {
        if (isOpen)
        {
            CloseAll();
        }

        else
        {
            OpenMenu();
        }
    }

    // opens/closes menu and follows current account scene behavior
    public void OpenMenu()
    {
        if (settingsRoot == null || mainSettingsPanel == null || languagePanel == null)
        {
            Debug.LogWarning("[GlobalSettingsMenu] Cannot toggle menu because panel references are missing.");
            return;
        }

        // check if panel is already open
        if (isOpen) 
        {
            return;
        }

        isOpen = true;

        // save current cursor/time state so it can be restored later
        prevLock = Cursor.lockState;
        prevVisible = Cursor.visible;
        prevTimeScale = Time.timeScale;

        // show the popup root
        settingsRoot.SetActive(true);

        // account management opens language only, other scenes open main settings first
        if (IsAccountManagementScene())
        {
            mainSettingsPanel.SetActive(false);
            languagePanel.SetActive(true);
        }

        else
        {
            mainSettingsPanel.SetActive(true);
            languagePanel.SetActive(false);
        }

        // disable scene gameplay scripts while menu is open
        DisableGameplayForPopup();

        // unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // optional pause
        if (pauseWithTimeScale)
        {
            Time.timeScale = 0f;
        }
    }

    // opens langauge subpanel from main settings menu
    public void OpenLanguagePanel()
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(true);
        }

        if (mainSettingsPanel != null)
        {
            mainSettingsPanel.SetActive(false);
        }

        if (languagePanel != null)
        {
            languagePanel.SetActive(true);
        }
    }

    // returns to main settings panel from lanuage panel
    public void BackToMainSettings()
    {
        // in account management, just closes panel
        if (IsAccountManagementScene())
        {
            CloseAll();
            return;
        }

        if (settingsRoot != null)
        {
            settingsRoot.SetActive(true);
        }

        if (mainSettingsPanel != null)
        {
            mainSettingsPanel.SetActive(true);
        }

        if (languagePanel != null)
        {
            languagePanel.SetActive(false);
        }
    }

    // logs out user and returns to account management
    public void OnLogoutButtonClicked()
    {
        CloseAll();

        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ClearUser();
        }

        SceneManager.LoadScene(accountManagementSceneName);
    }

    // sends user back to site selection scene
    public void OnBackToSiteSelectionClicked()
    {
        CloseAll();
        SceneManager.LoadScene(siteSelectionSceneName);
    }

    // closes menu and all child panels
    public void CloseAll()
    {
        if (mainSettingsPanel != null)
        {
            mainSettingsPanel.SetActive(false);
        }

        if (languagePanel != null)
        {
            languagePanel.SetActive(false);
        }

        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }

        if (!isOpen) 
        {
            return;
        }

        isOpen = false;

        // re-enable whatever gameplay scripts were disabled
        RestoreGameplayAfterPopup();

        // restore previous cursor state
        Cursor.lockState = prevLock;
        Cursor.visible = prevVisible;

        // restore previous time scale
        if (pauseWithTimeScale)
        {
            Time.timeScale = prevTimeScale;
        } 
    }

    public void ChooseEnglish()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.SetLanguage("en");
        }

        BackToMainSettings();
    }

    public void ChooseSpanish()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.SetLanguage("es");
        }

        BackToMainSettings();
    }

        // disables gameplay scripts that should stop while the menu is open
    private void DisableGameplayForPopup()
    {
        disabledRuntime.Clear();

        // player movement/look
        var fps = FindFirstObjectByTypeSafe<FirstPersonController>();
        if (fps != null && fps.enabled)
        {
            fps.enabled = false;
            disabledRuntime.Add(fps);
        }

        // camera movement/look
        var camMove = FindFirstObjectByTypeSafe<MoveCamera>();
        if (camMove != null && camMove.enabled)
        {
            camMove.enabled = false;
            disabledRuntime.Add(camMove);
        }
    }

    // re-enables all scripts that were disabled when the menu opened
    private void RestoreGameplayAfterPopup()
    {
        for (int index = 0; index < disabledRuntime.Count; index++)
        {
            if (disabledRuntime[index] != null)
                disabledRuntime[index].enabled = true;
        }

        disabledRuntime.Clear();
    }

    // helper for safely finding scene objects in different Unity versions
    private T FindFirstObjectByTypeSafe<T>() where T : Object
    {
#if UNITY_6000_0_OR_NEWER
        var found = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
        return (found != null && found.Length > 0) ? found[0] : null;
#else
        return Object.FindObjectOfType<T>();
#endif
    }
}