using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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
                     "http://localhost:4000/api/artifacts/latest";

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
        public float weight;
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

    // local cache of artifacts
    private List<Artifact> _artifacts = new List<Artifact>();

    // functions

    // initialize analysis panel
    private void Start()
    {
        // make input fields read only
        SetFieldsReadOnly(true);

        if( StatusText.text != null )
        {
            StatusText.text = "Loading artifact list...";
        }

        StartCoroutine(LoadArtifactListCoroutine());
    }

    // analyze button
    public void OnAnalyzeButtonClicked()
    {
        // error handling
        if( _artifacts.Count == 0 )
        {
            StatusText.text = "No artifacts available to analyze.";
            return;
        }

        // initalize index
        int index = SelectArtifact != null ? SelectArtifact.value : 0;

        // error handling
        if ( index < 0 || index >= _artifacts.Count )
        {
            StatusText.text = "Invalid selection.";
            return;
        }

        // select artifact
        Artifact selectedArtifact = _artifacts[index];
        PopulateUI(selectedArtifact);

        if( StatusText.text != null )
        {
            StatusText.text = "Artifact data loaded.";
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
                    StatusText.text = "Error loading artifact list.";
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
                if( StatusText != null )
                {
                    StatusText.text =
                        response != null && !string.IsNullOrEmpty(response.error)
                            ? $"Server error: {response.error}"
                            : "No artifact data available.";
                }

                yield break;
            }

            // get artifact list
            _artifacts = new List<Artifact>(response.artifacts);

            if( _artifacts.Count == 0 )
            {
                if( StatusText != null )
                {
                    StatusText.text = "No artifacts in database.";
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
                StatusText.text = "Select an artifact and press Analyze.";
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

        List<TMP_Dropdown.OptionData> options =
            new List<TMP_Dropdown.OptionData>();

        foreach (var a in _artifacts)
        {
            // clean date to just YYYY-MM-DD if it has a T timestamp
            string rawDate = a.date_discovered ?? "";
            string shortDate =
                rawDate.Length >= 10 ? rawDate.Substring(0, 10) : rawDate;

            string label =
                $"{a.bag_number} | {a.artifact_id} | {shortDate}";

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
        WeightInput.text = artifact.weight.ToString("0.##");
        BagNumberInput.text = artifact.bag_number;
        ArtifactIDInput.text = artifact.artifact_id;
    }
}