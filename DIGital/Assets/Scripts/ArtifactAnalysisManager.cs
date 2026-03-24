using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

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

    [Header("Logout / Navigation")]
    [SerializeField] private string accountManagementSceneName = "AccountManagement";

    // dropdown menu
    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown SelectArtifact;
    [SerializeField] private string dropdownPlaceholderText = "Select an artifact from this dropdown menu";

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

    // artifact pivot
    [Header("3D Preview Pivot")]
    [SerializeField] private Transform artifactPivot;

    // localization table keys
    [SerializeField] private string LoadingList = "analysis_status_loading_list";
    [SerializeField] private string ErrorLoadingList = "analysis_status_error_loading_list";
    [SerializeField] private string NoArtifactsToAnalyze = "analysis_status_no_artifacts_to_analyze";
    [SerializeField] private string InvalidSelection = "analysis_status_invalid_selection";
    [SerializeField] private string DataLoaded = "analysis_status_data_loaded";
    [SerializeField] private string NoArtifactData = "analysis_status_no_artifact_data";
    [SerializeField] private string NoArtifactsInDb = "analysis_status_no_artifacts_in_db";
    [SerializeField] private string SelectAndAnalyze = "analysis_status_select_and_analyze";
    [SerializeField] private string DropdownLabelKey = "analysis_dropdown_label";

    // re-translation
    private enum StatusMode { None, Key, ServerError }
    private StatusMode _statusMode = StatusMode.None;
    private string _lastStatusKey = null;
    private string _lastServerError = null;

    // smart string error 
    [SerializeField] private string ServerErrorDetails = "analysis_status_server_error_details";

    // registry
    [SerializeField] private ArtifactSceneModelRegistry modelRegistry;
    [SerializeField] private GameObject defaultModel; // optional fallback

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

        if (SelectArtifact != null)
        {
            SelectArtifact.onValueChanged.AddListener(OnArtifactDropdownChanged);
        }
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

        if (SelectArtifact != null)
        {
            SelectArtifact.onValueChanged.RemoveListener(OnArtifactDropdownChanged);
        }
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
        /* // Guard: if not logged in, send them to account management
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn)
        {
            SceneManager.LoadScene(accountManagementSceneName);
            return;
        } */

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

    // logout button
    public void OnLogoutButtonClicked()
    {
        // hide any current active model(s)
        CleanupActiveModelForLogout();

        // clear session
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ClearUser();
        }

        // load account management scene
        SceneManager.LoadScene(accountManagementSceneName);
    }

    // hide active models for logging out
    private void CleanupActiveModelForLogout()
    {
        if (CurrentActiveModel == null) return;

        // detach model pivot
        CurrentActiveModel.transform.SetParent(null, worldPositionStays: true);
        CurrentActiveModel.SetActive(false);
        CurrentActiveModel = null;

        // hide panel
        if (BackgroundPanel != null)
        {
            BackgroundPanel.SetActive(true);
        }
    }

    // CHANGED from analyze button to analyze on dropdown clicked
    public void OnArtifactDropdownChanged(int index)
    {
        // error handling
        if( _artifacts == null || _artifacts.Count == 0 )
        {
            SetStatus(NoArtifactsToAnalyze);
            return;
        }

        // index placeholder
        if (index == 0)
        {
            ClearUI();

            if (StatusText != null)
            {
                SetStatus(SelectAndAnalyze);
            }

            return;
        }

        int artifactIndex = index - 1;

        // error handling
        if ( artifactIndex < 0 || artifactIndex >= _artifacts.Count )
        {
            SetStatus(InvalidSelection);
            return;
        }

        // select artifact
        Artifact selectedArtifact = _artifacts[artifactIndex];
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

    // clear fields when placeholder is selected
    private void ClearUI()
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
        ArtifactIDInput.text = "";

        if (modelRegistry != null)
        {
            modelRegistry.HideAll();
        }

        if (defaultModel != null)
        {
            defaultModel.SetActive(false);
        }

        CurrentActiveModel = null;

        if (BackgroundPanel != null)
        {
            BackgroundPanel.SetActive(true);
        }
    }

    // load artifact from database
    private IEnumerator LoadArtifactListCoroutine()
    {
        // make sure user is logged in and has id
        if ( SessionManager.Instance == null || string.IsNullOrEmpty(SessionManager.Instance.UserId))
        {
            Debug.LogError("[ArtifactAnalysisManager] No session userId found. Cannot load user artifacts.");
            if (StatusText != null) SetStatus(ErrorLoadingList);
            yield break;
        }

        // use node app JS code
        string userId = SessionManager.Instance.UserId;
        string url = $"{apiUrl}/mine/{UnityWebRequest.EscapeURL(userId)}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

        // placeholder
        options.Add(new TMP_Dropdown.OptionData(dropdownPlaceholderText));

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
        SelectArtifact.SetValueWithoutNotify(0);
        SelectArtifact.RefreshShownValue();

        ClearUI();
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
        // turn off all models referenced by the registry (so only one shows)
        if (modelRegistry != null)
        modelRegistry.HideAll();

        // hide default model (optional if we use one later)
        if (defaultModel != null)
            defaultModel.SetActive(false);

        CurrentActiveModel = null;

        if (artifact == null)
        {
            Debug.LogWarning("[ArtifactAnalysisManager] Artifact is null in ShowModelForArtifact");
            return;
        }

        string id = (artifact.artifact_id ?? "").Trim();
        Debug.Log($"[ArtifactAnalysisManager] ShowModelForArtifact called for artifact_id: '{id}'");

        // look up the model in the registry
        GameObject model = (modelRegistry != null) ? modelRegistry.GetModel(id) : null;

        if (model != null)
        {
            model.SetActive(true);
            CurrentActiveModel = model;
            SetupPivotWithoutMovingModel(CurrentActiveModel);
            return;
        }

        // fallback if no match
        Debug.LogWarning($"[ArtifactAnalysisManager] No model found for artifact_id '{id}'. Using default model.");
        if (defaultModel != null)
        {
            defaultModel.SetActive(true);
            CurrentActiveModel = defaultModel;
            SetupPivotWithoutMovingModel(CurrentActiveModel);
        }
    }

    private void SetupPivotWithoutMovingModel(GameObject model)
    {
        if (artifactPivot == null || model == null) return;

        // find visual center of model in world space
        var renderers = model.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0) return;

        Bounds bound = renderers[0].bounds;

        for (int index = 1; index < renderers.Length; index++)
        {
            bound.Encapsulate(renderers[index].bounds);
        }

        // put pivot at model center
        artifactPivot.position = bound.center;

        // reset artifact rotation so each artifact starts upright
        artifactPivot.rotation = Quaternion.identity;

        // parent the model to the pivot
        model.transform.SetParent(artifactPivot, worldPositionStays: true);
    }
}