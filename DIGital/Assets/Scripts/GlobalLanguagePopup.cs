using UnityEngine;

public class GlobalLanguagePopup : MonoBehaviour
{
    public static GlobalLanguagePopup Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    bool isOpen;

    void Awake()
    {
        // persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (panelRoot == null) panelRoot = gameObject;
        panelRoot.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        panelRoot.SetActive(isOpen);
    }

    public void Open()
    {
        isOpen = true;
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        isOpen = false;
        panelRoot.SetActive(false);
    }

    // Hook these to button OnClick()
    public void ChooseEnglish()
    {
        LanguageManager.Instance.SetLanguage("en");
        Close();
    }

    public void ChooseSpanish()
    {
        LanguageManager.Instance.SetLanguage("es");
        Close();
    }
}
