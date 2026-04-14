using UnityEngine;
using UnityEngine.UI;

public class WalkExcavateIntroController : MonoBehaviour
{
    public static bool IsIntroOpen { get; private set; }

    public static WalkExcavateIntroController Instance { get; private set; }

    [Header("Popup")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Button continueButton;

    [Header("Disable while popup is open")]
    [SerializeField] private MonoBehaviour[] gameplayScriptsToDisable;
    [SerializeField] private MonoBehaviour[] uiScriptsToDisable;

    [Header("Cursor")]
    [SerializeField] private bool relockCursorOnClose = true;
    [SerializeField] private bool hideCursorOnClose = true;

    private void Awake()
    {
        Instance = this;

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(CloseIntro);
        }
    }

    private void Start()
    {
        OpenIntro();
    }

    public void OpenIntro()
    {
        IsIntroOpen = true;

        if (popupRoot != null)
        {
            popupRoot.SetActive(true);
        }

        SetScriptsEnabled(gameplayScriptsToDisable, false);
        SetScriptsEnabled(uiScriptsToDisable, false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseIntro()
    {
        IsIntroOpen = false;

        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
        }

        SetScriptsEnabled(gameplayScriptsToDisable, true);
        SetScriptsEnabled(uiScriptsToDisable, true);

        if (relockCursorOnClose)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Cursor.visible = !hideCursorOnClose;
    }

    private void SetScriptsEnabled(MonoBehaviour[] scripts, bool enabledState)
    {
        if (scripts == null) return;

        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
            {
                script.enabled = enabledState;
            }
        }
    }

    // let player close with Enter or Space
    private void Update()
    {
        if (!IsIntroOpen) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            CloseIntro();
        }
    }

    public void HideIntroVisuals()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
        }
    }

    public void ShowIntroVisuals()
    {
        if (IsIntroOpen && popupRoot != null)
        {
            popupRoot.SetActive(true);
        }
    }
}