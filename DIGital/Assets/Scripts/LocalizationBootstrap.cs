using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class LocalizationBootstrap : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        yield return null;

        RefreshAll();
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        var all = FindObjectsOfType<LocalizeStringEvent>(true);
        foreach (var lse in all)
            lse.RefreshString();

        Debug.Log($"[LocalizationBootstrap] Refreshed {all.Length} LocalizeStringEvent(s).");
    }
}
