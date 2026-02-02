using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    // declare / initialize variables
    public static LanguageManager Instance { get; private set; }

    public const string PrefKey = "preferred_language"; // "en" or "es"
    public string CurrentCode { get; private set; } = "en";

    public event Action<string> OnLanguageChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        // load saved language
        CurrentCode = PlayerPrefs.GetString(PrefKey, "en");
        StartCoroutine(ApplyLocaleWhenReady(CurrentCode));
    }

    // allow changes during runtime
    public void SetLanguage(string code)
    {
        if (string.IsNullOrEmpty(code) || code == CurrentCode) return;

        CurrentCode = code;
        PlayerPrefs.SetString(PrefKey, code);
        PlayerPrefs.Save();

        StartCoroutine(ApplyLocaleWhenReady(code));
    }

    // apply locale to Unity Localization
    private IEnumerator ApplyLocaleWhenReady(string code)
    {
        yield return LocalizationSettings.InitializationOperation;

        Locale target = null;
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code.StartsWith(code, StringComparison.OrdinalIgnoreCase))
            {
                target = locale;
                break;
            }
        }

        if (target != null)
        {
            LocalizationSettings.SelectedLocale = target;
            OnLanguageChanged?.Invoke(code);
        }
    }
}
