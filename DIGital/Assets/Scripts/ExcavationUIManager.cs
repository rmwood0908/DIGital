using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ExcavationUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text welcomeText;

    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string userKey = "walk_and_excavate_welcome_text";
    [SerializeField] private string guestKey = "walk_and_excavate_welcome_guest_text";
    
    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void Start()
    {
        UpdateWelcome();
    }

    void OnLocaleChanged(Locale _)
    {
        UpdateWelcome();
    }

    void UpdateWelcome()
    {
        if (welcomeText == null)
        {
            Debug.LogWarning("[ExcavationUIManager] Welcome Text is not assigned.");
            return;
        }

        bool loggedIn = SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn;

        if (loggedIn)
        {
            string username = SessionManager.Instance.Username;

            // smart string argument
            var localized = new LocalizedString(table, userKey);
            localized.Arguments = new object[] { new { username = username } };

            welcomeText.text = localized.GetLocalizedString() + " " + SessionManager.Instance.selectedSite + "!";
        }
        else
        {
            welcomeText.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, guestKey);
        }
    }

    // hide text when AI panel opens
    public void SetWelcomeVisible(bool visible) {
        
        if (welcomeText == null) return;
        welcomeText.gameObject.SetActive(visible);

        if (visible) UpdateWelcome();
    }
}
