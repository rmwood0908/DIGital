using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class WelcomeManualLocalizeTest : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private string table = "UI";
    [SerializeField] private string key = "welcome_title";

    IEnumerator Start()
    {
        if (!tmp) tmp = GetComponent<TMP_Text>();

        yield return LocalizationSettings.InitializationOperation;

        Apply("startup");

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(Locale _)
    {
        Apply("locale_changed");
    }

    private void Apply(string reason)
    {
        string s = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        var code = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "null";

        Debug.Log($"[WelcomeManualLocalizeTest] {reason} locale={code} -> '{s}'");

        tmp.text = s;
    }
}
