using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

// class
public class ArtifactFormManager : MonoBehaviour
{
    // input fields
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField DateDiscoveredInput;
    [SerializeField] private TMP_InputField InvestigatorInput;
    [SerializeField] private TMP_InputField AreaInput;
    [SerializeField] private TMP_InputField UnitInput;
    [SerializeField] private TMP_InputField LayerInput;
    [SerializeField] private TMP_InputField SiteInput;
    [SerializeField] private TMP_InputField AssociatedFeaturesInput;
    [SerializeField] private TMP_InputField MaterialTypeInput;
    [SerializeField] private TMP_InputField QuantityInput;
    [SerializeField] private TMP_InputField WeightInput;
    [SerializeField] private TMP_InputField BagNumberInput;
    [SerializeField] private TMP_Dropdown ArtifactIdDropdown;

    // popup panel game object  
    [Header("UI Feedback")]
    [SerializeField] private TMP_Text StatusText;
    [SerializeField] private GameObject PanelRoot;

    // api settings
    [Header("API Settings")]
    [SerializeField] private string apiUrl = 
                    "https://digital-ty59.onrender.com/api/artifacts";

    // localization
    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string errorDetailsKey = "artifact_collection_status_error_details";

    // registry
    [Header("Registry")]
    [SerializeField] private ArtifactIdRegistry idRegistry;

    // cache to not recreate error message each time
    private LocalizedString errorLocalizedString;

    // input field types
    [System.Serializable]
    private class ArtifactData
    {
        public string date_discovered;
        public string investigator;
        public string area;
        public string unit;
        public string layer;
        public string site;
        public string associated_features;
        public string material_type;
        public int quantity;
        public string weight;
        public string bag_number;
        public string artifact_id;
        public string userId;
    }

    private void Awake()
    {
        // cache this once (uses key + table from Inspector)
        errorLocalizedString = new LocalizedString(table, errorDetailsKey);
    }

    private void Start()
    {
        // ensure the panel starts hidden if you want it that way
        if (PanelRoot != null)
        {
            PanelRoot.SetActive(false);
        }

        PopulateArtifactIdDropdown();
    }

    // status text with localization
    private void SetStatus(string key)
    {
        if (StatusText == null) return;
        StatusText.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
    }

    private void SetStatusErrorWithDetails(string details)
    {
        if (StatusText == null) return;

        errorLocalizedString.Arguments = new object[] { new { details } }; // smart string
        StatusText.text = errorLocalizedString.GetLocalizedString();
    }

    public void OpenForm()
    {
        if (PanelRoot != null)
        {
            Debug.Log("[ArtifactFormManager] OpenForm() - enabling PanelRoot");
            PanelRoot.SetActive(true);

            // unlock cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

            // pause game while form is open
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("[ArtifactFormManager] PanelRoot is not assigned!");
        }
    }

    public void CloseForm()
    {
        if (PanelRoot != null)
        {
            PanelRoot.SetActive(false);

            // restore FPS controls
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;

            // resume game if you paused it
            Time.timeScale = 1f;
        }
    }


    // submit button clicked
    public void OnSubmitButtonClicked()
    {
        // front end validation
        if (!ValidateInputs(out int Quantity))
        {
            return;
        }

        string enteredId = GetSelectedArtifactId();
        
        if (string.IsNullOrEmpty(enteredId))
        {
            SetStatus("artifact_collection_status_invalid_artifact_id");
            return;
        }

        SetStatus("artifact_collection_status_submit");

        ArtifactData data = new ArtifactData
        {
            date_discovered = DateDiscoveredInput.text.Trim(),
            investigator = InvestigatorInput.text.Trim(),
            area = AreaInput.text.Trim(),
            unit = UnitInput.text.Trim(),
            layer = LayerInput.text.Trim(),
            site = SiteInput.text.Trim(),
            associated_features = AssociatedFeaturesInput.text.Trim(),
            material_type = MaterialTypeInput.text.Trim(),
            quantity = Quantity,
            weight = WeightInput.text.Trim(),
            bag_number = BagNumberInput.text.Trim(),
            artifact_id = enteredId,
            userId = (SessionManager.Instance != null &&
                        SessionManager.Instance.IsLoggedIn)
                        ? SessionManager.Instance.UserId
                        : null            
        };

        StartCoroutine(SubmitArtifactCoroutine(data));
    }

    // cancel button clicked
    public void OnCancelButtonClicked()
    {
        Debug.Log("[ArtifactFormManager] Cancel button clicked");
        ClearForm();
        StatusText.text = "";
        CloseForm();
    }

    private bool ValidateInputs( out int Quantity )
    {
        Quantity = 0;

        // required fields
        if (string.IsNullOrWhiteSpace(DateDiscoveredInput.text) ||
            string.IsNullOrWhiteSpace(InvestigatorInput.text) ||
            string.IsNullOrWhiteSpace(AreaInput.text) ||
            string.IsNullOrWhiteSpace(UnitInput.text) ||
            string.IsNullOrWhiteSpace(LayerInput.text) ||
            string.IsNullOrWhiteSpace(SiteInput.text) ||
            string.IsNullOrWhiteSpace(MaterialTypeInput.text) ||
            string.IsNullOrWhiteSpace(QuantityInput.text) ||
            string.IsNullOrWhiteSpace(WeightInput.text) ||
            string.IsNullOrWhiteSpace(BagNumberInput.text))

        {
            SetStatus("artifact_collection_status_missing_fields");
            return false;
        }

        // quantity validation
        if (!int.TryParse(QuantityInput.text.Trim(), out Quantity))
        {
            SetStatus("artifact_collection_status_invalid_quantity");
            return false;
        }

        return true;
    }

    // submit artifact coroutine
    private IEnumerator SubmitArtifactCoroutine(ArtifactData artifactData)
    {
        SetStatus("artifact_collection_status_database");

        string json = JsonUtility.ToJson(artifactData);
        byte[] postData = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

    #if UNITY_EDITOR
            Debug.Log($"Artifact JSON: {json}");
    #endif

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
    #if UNITY_EDITOR
                Debug.Log("Artifact submit success: " +
                        request.downloadHandler.text);
    #endif

                SetStatus("artifact_collection_status_success");

                ClearForm();
            }

            else
            {
                string serverText = request.downloadHandler != null ? request.downloadHandler.text : "";
                string details = !string.IsNullOrEmpty(serverText) ? serverText : request.error;

                SetStatusErrorWithDetails(details);
            }
        }
    }

    // clear form
    private void ClearForm()
    {
        DateDiscoveredInput.text = "";
        InvestigatorInput.text = "";
        AreaInput.text = "";
        UnitInput.text = "";
        LayerInput.text = "";
        SiteInput.text = "";
        AssociatedFeaturesInput.text = "";
        MaterialTypeInput.text = "";
        QuantityInput.text = "";
        WeightInput.text = "";
        BagNumberInput.text = "";
    }

    private void PopulateArtifactIdDropdown()
    {
        if (ArtifactIdDropdown == null || idRegistry == null || idRegistry.entries == null)
        {
            Debug.LogWarning("[ArtifactFormManager] Dropdown or ID registry not assigned.");
            return;
        }

        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var entry in idRegistry.entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.artifactId)) continue;

            string id = entry.artifactId.Trim();
            string label = !string.IsNullOrWhiteSpace(entry.displayName)
                ? $"{entry.displayName} ({id})"
                : id;

            options.Add(new TMP_Dropdown.OptionData(label));
        }

        ArtifactIdDropdown.ClearOptions();
        ArtifactIdDropdown.AddOptions(options);
        ArtifactIdDropdown.value = 0;
        ArtifactIdDropdown.RefreshShownValue();
    }

    private string GetSelectedArtifactId()
    {
        if (ArtifactIdDropdown == null || idRegistry == null || idRegistry.entries == null)
            return null;

        int index = ArtifactIdDropdown.value;
        if (index < 0 || index >= idRegistry.entries.Count)
            return null;

        return idRegistry.entries[index]?.artifactId?.Trim();
    }
}
