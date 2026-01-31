using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[LocaleDebug] Start() running on: " + gameObject.name);
        StartCoroutine(InitCheck());
    }

    IEnumerator InitCheck()
    {
        yield return LocalizationSettings.InitializationOperation;
        Debug.Log("[LocaleDebug] Localization initialized. SelectedLocale = " +
                  (LocalizationSettings.SelectedLocale == null ? "null" : LocalizationSettings.SelectedLocale.Identifier.Code));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var loc = LocalizationSettings.SelectedLocale;
            Debug.Log("[LocaleDebug] L pressed. SelectedLocale = " +
                      (loc == null ? "null" : loc.Identifier.Code));
        }
    }
}
