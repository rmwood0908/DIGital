using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

// class
public class ArtifactAnalysisManager : MonoBehaviour
{
    // input fields (really output fields but Unity calls them input)
    [Header("Display Fields (InputFields)")]
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
    [SerializeField] private TMP_InputField ArtifactIDInput;

    // dropdown menu
    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown SelectArtifact;

    // status text
    [Header("Status Text")]
    [SerializeField] private TMP_Text StatusText;

    // api
    [Header("API Settings")]
    [SerializeField] private string apiUrl =
                     "https://digital-ty59.onrender.com/api/artifacts";

    // localization
    [Header("Localization")]
    [SerializeField] private string table = "UI";

    // localization table keys
    [SerializeField] private string LoadingList = "analysis_status_loading_list";
    [SerializeField] private string ErrorLoadingList = "analysis_status_error_loading_list";
    [SerializeField] private string NoArtifactsToAnalyze = "analysis_status_no_artifacts_to_analyze";
    [SerializeField] private string InvalidSelection = "analysis_status_invalid_selection";
    [SerializeField] private string DataLoaded = "analysis_status_data_loaded";
    [SerializeField] private string NoArtifactData = "analysis_status_no_artifact_data";
    [SerializeField] private string NoArtifactsInDb = "analysis_status_no_artifacts_in_db";
    [SerializeField] private string SelectAndAnalyze = "analysis_status_select_and_analyze";
    [SerializeField] private string DropdownLabelKey = "artifact_analysis_dropdown_label";

    // re-translation
    private enum StatusMode { None, Key, ServerError }
    private StatusMode _statusMode = StatusMode.None;
    private string _lastStatusKey = null;
    private string _lastServerError = null;

    // smart string error 
    [SerializeField] private string ServerErrorDetails = "analysis_status_server_error_details";

    private LocalizedString lsServerErrorDetails;
    private LocalizedString lsDropdownLabel;

    // node app variables
    [System.Serializable]
    private class Artifact
    {
        public int id;
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
    }

    [System.Serializable]
    private class ArtifactListResponse
    {
        public bool ok;
        public Artifact[] artifacts;
        public string error;
    }

    // 3d model bindings
    [System.Serializable]
    private class ModelBinding
    {
        public string artifactId;
        public GameObject modelObject;
    }

    [Header("3D Model Bindings")]
    [SerializeField] private List<ModelBinding> modelBindings = 
                                                new List<ModelBinding>();

    [Header("UI Panel")]
    [SerializeField] private GameObject BackgroundPanel;

    // local cache of artifacts
    private List<Artifact> _artifacts = new List<Artifact>();

    public GameObject CurrentActiveModel { get; private set; }

    // functions

    private void Awake()
    {
        // intialize error string and dropdown
        lsServerErrorDetails = new LocalizedString(table, ServerErrorDetails);
        lsDropdownLabel = new LocalizedString(table, DropdownLabelKey);
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale _)
    {
        // refresh dropdown labels
        if (_artifacts != null && _artifacts.Count > 0)
            PopulateDropdownOptions();

        // refresh the current status message
        RefreshStatusText();
    }

    private void RefreshStatusText()
    {
        if (StatusText == null) return;

        switch (_statusMode)
        {
            case StatusMode.Key:
                if (!string.IsNullOrEmpty(_lastStatusKey))
                    StatusText.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, _lastStatusKey);
                break;

            case StatusMode.ServerError:
                if (!string.IsNullOrEmpty(_lastServerError))
                {
                    lsServerErrorDetails.Arguments = new object[] { new { error = _lastServerError } };
                    StatusText.text = lsServerErrorDetails.GetLocalizedString();
                }
                break;
        }
    }

    // initialize analysis panel
    private void Start()
    {
        // make input fields read only
        SetFieldsReadOnly(true);

        if( StatusText != null )
        {
            SetStatus(LoadingList);
        }

        StartCoroutine(LoadArtifactListCoroutine());
    }

    private void SetStatus(string key)
    {
        _statusMode = StatusMode.Key;
        _lastStatusKey = key;

        if (StatusText == null) return;
        StatusText.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
    }

    private void SetStatusServerError(string error)
    {
        _statusMode = StatusMode.ServerError;
        _lastServerError = error;

        if (StatusText == null) return;

        // uses Smart String in table
        lsServerErrorDetails.Arguments = new object[] { new { error } };
        StatusText.text = lsServerErrorDetails.GetLocalizedString();
    }

    // analyze button
    public void OnAnalyzeButtonClicked()
    {
        // error handling
        if( _artifacts.Count == 0 )
        {
            SetStatus(NoArtifactsToAnalyze);
            return;
        }

        // initalize index
        int index = SelectArtifact != null ? SelectArtifact.value : 0;

        // error handling
        if ( index < 0 || index >= _artifacts.Count )
        {
            SetStatus(InvalidSelection);
            return;
        }

        // select artifact
        Artifact selectedArtifact = _artifacts[index];
        PopulateUI(selectedArtifact);

        // hide background panel TEMP WORKAROUND TO SHOW 3D MODEL
        if( BackgroundPanel != null )
        {
            BackgroundPanel.SetActive(false);
        }

        if( StatusText != null )
        {
            SetStatus(DataLoaded);
        }
    }

    // set input to read only
    private void SetFieldsReadOnly(bool readOnly)
    {
        TMP_InputField[] fields =
        {
            DateDiscoveredInput, InvestigatorInput, AreaInput, UnitInput,
            LayerInput, SiteInput, AssociatedFeaturesInput,
            MaterialTypeInput, QuantityInput, WeightInput,
            BagNumberInput, ArtifactIDInput
        };

        foreach ( var field in fields )
        {
            if( field == null ) continue;
            field.readOnly = readOnly;
            field.interactable = !readOnly;
        }
    }

    // load artifact from database
    private IEnumerator LoadArtifactListCoroutine()
    {
        // use node app JS code
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            // error handling
            if (request.result != UnityWebRequest.Result.Success)
            {
                string serverText = request.downloadHandler != null
                    ? request.downloadHandler.text
                    : "";

                Debug.LogError(
                    $"Error fetching artifact list: " +
                    $"{request.responseCode} - {request.error} - {serverText}");


                if (StatusText != null)
                {
                    SetStatus(ErrorLoadingList);
                }

                yield break;
            }

            string json = request.downloadHandler.text;

        #if UNITY_EDITOR
            Debug.Log($"Artifact list JSON: {json}");
        #endif

            ArtifactListResponse response =
                JsonUtility.FromJson<ArtifactListResponse>(json);

            if (response == null || !response.ok || response.artifacts == null)
            {

                if (response != null && !string.IsNullOrEmpty(response.error))
                {
                    SetStatusServerError(response.error);
                }

                else
                {
                    SetStatus(NoArtifactData);
                }

                yield break;
            }

            // get artifact list
            _artifacts = new List<Artifact>(response.artifacts);

            if( _artifacts.Count == 0 )
            {
                if( StatusText != null )
                {
                    SetStatus(NoArtifactsInDb);
                }

                if ( SelectArtifact != null )
                {
                    SelectArtifact.ClearOptions();
                }

                yield break;
            }

            // success, populate dropdown
            PopulateDropdownOptions();

            if( StatusText != null )
            {
                SetStatus(SelectAndAnalyze);
            }
        }
    }

    // populate dropdown function
    private void PopulateDropdownOptions()
    {
        if (SelectArtifact == null)
        {
            Debug.LogWarning("SelectArtifact is not assigned.");
            return;
        }

        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var artifact in _artifacts)
        {
            string rawDate = artifact.date_discovered ?? "";
            string shortDate = rawDate.Length >= 10 ? rawDate.Substring(0, 10) : rawDate;

            // Smart String arguments (named placeholders)
            lsDropdownLabel.Arguments = new object[]
            {
                new
                {
                    id = artifact.artifact_id ?? "",
                    bag = artifact.bag_number ?? "",
                    date = shortDate
                }
            };

            string label = lsDropdownLabel.GetLocalizedString();
            options.Add(new TMP_Dropdown.OptionData(label));
        }

        SelectArtifact.ClearOptions();
        SelectArtifact.AddOptions(options);
        SelectArtifact.value = 0;
        SelectArtifact.RefreshShownValue();
    }

    // populate input fields
    private void PopulateUI(Artifact artifact)
    {
        // format date
        if (!string.IsNullOrEmpty(artifact.date_discovered))
        {
            string[] parts = artifact.date_discovered.Split('T');
            DateDiscoveredInput.text = parts[0].Trim();
        }
        
        else
        {
            DateDiscoveredInput.text = "";
        }

        InvestigatorInput.text = artifact.investigator;
        AreaInput.text = artifact.area;
        UnitInput.text = artifact.unit;
        LayerInput.text = artifact.layer;
        SiteInput.text = artifact.site;
        AssociatedFeaturesInput.text = artifact.associated_features;
        MaterialTypeInput.text = artifact.material_type;
        QuantityInput.text = artifact.quantity.ToString();
        WeightInput.text = artifact.weight ?? "";
        BagNumberInput.text = artifact.bag_number;
        ArtifactIDInput.text = artifact.artifact_id;
        ShowModelForArtifact(artifact);
    }

    private void ShowModelForArtifact(Artifact artifact)
    {
        // hide all bound models to start
        if (modelBindings != null)
        {
            foreach (var binding in modelBindings)
            {
                if (binding != null && binding.modelObject != null)
                {
                    binding.modelObject.SetActive(false);
                }
            }
        }

        CurrentActiveModel = null;

        if (artifact == null)
        {
            Debug.LogWarning("[ArtifactAnalysisManager] Artifact is null in ShowModelForArtifact");
            return;
        }

        string dbId = artifact.artifact_id != null
            ? artifact.artifact_id.Trim()
            : "(null)";

        Debug.Log($"[ArtifactAnalysisManager] ShowModelForArtifact called for DB ID: '{dbId}'");

        bool foundMatch = false;

        if (modelBindings != null && modelBindings.Count > 0)
        {
            foreach (var binding in modelBindings)
            {
                if (binding == null || binding.modelObject == null)
                    continue;

                string bindingId = binding.artifactId != null
                    ? binding.artifactId.Trim()
                    : "(null)";

                Debug.Log($"[ArtifactAnalysisManager] Checking binding: '{bindingId}' " +
                        $"against DB ID: '{dbId}'");

                if (string.Equals(bindingId,
                                dbId,
                                StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("[ArtifactAnalysisManager] Match found! " +
                            $"Enabling model: {binding.modelObject.name}");

                    binding.modelObject.SetActive(true);
                    CurrentActiveModel = binding.modelObject;
                    foundMatch = true;
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("[ArtifactAnalysisManager] modelBindings list is empty or null.");
        }

        if (!foundMatch)
        {
            Debug.LogWarning("[ArtifactAnalysisManager] No modelBinding matched DB ID: '" +
                            dbId + "'");
        }
    }
}