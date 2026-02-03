using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[DisallowMultipleComponent]
public class AutoLocalizeTMP : MonoBehaviour
{
    [Header("Target (leave empty to use TMP on this object)")]
    [SerializeField] private TMP_Text target;

    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string key;

    private LocalizedString localizedString;
    private bool hooked;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<TMP_Text>();

        localizedString = new LocalizedString(table, key);
    }

    private void OnEnable()
    {
        // if the popup becomes active after locale is already chosen,
        // must refresh RIGHT NOW.
        StartCoroutine(InitAndRefresh());
    }

    private void OnDisable()
    {
        Unhook();
    }

    private IEnumerator InitAndRefresh()
    {
        // wait for Localization system (important for popups created early/late)
        yield return LocalizationSettings.InitializationOperation;

        Hook();

        // force an immediate refresh whenever this object is enabled
        localizedString.RefreshString();
    }

    private void Hook()
    {
        if (hooked) return;
        hooked = true;

        // when the string updates (including locale changes), apply it.
        localizedString.StringChanged += Apply;

        // if locale changes, refresh this entry.
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void Unhook()
    {
        if (!hooked) return;
        hooked = false;

        localizedString.StringChanged -= Apply;
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale _)
    {
        // force refresh when locale changes
        localizedString.RefreshString();
    }

    private void Apply(string value)
    {
        if (target == null) return;
        target.text = value;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // keep the localizedString in sync when you edit fields in inspector
        if (!string.IsNullOrEmpty(table) && !string.IsNullOrEmpty(key))
        {
            localizedString = new LocalizedString(table, key);
        }
    }
#endif
}