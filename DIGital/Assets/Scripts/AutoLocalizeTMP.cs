using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[DisallowMultipleComponent]
public class AutoLocalizeTMP : MonoBehaviour
{
    // intialize TMP and localzie table variables
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private string table = "UI";
    [SerializeField] private string key;

    void Awake()
    {
        if (!tmp) tmp = GetComponent<TMP_Text>();
    }

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        // wait a frame so other “init/reset UI” scripts run first
        yield return null;

        // reapply tmp if removed
        Apply();
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(Locale _)
    {
        Apply();
    }

    public void Apply()
    {
        if (!tmp || string.IsNullOrEmpty(key)) return;
        tmp.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
    }
}
