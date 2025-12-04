using System.Collections;
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

    // status text
    [Header("Status Text")]
    [SerializeField] private TMP_Text StatusText;

    // api
    [Header("API Settings")]
    [SerializeField] private string apiURL =
                     "http://localhost:4000/api/artifacts/latest";

    // node app variables
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

    private class ArtifactResponse
    {
        public bool ok;
        public Artifact artifact;
        public string error;
    }

    // functions

    // initialize analysis panel
    private void Start()
    {
        // make input fields read only
        SetFieldsReadOnly(true);

        StatusText.text = "Loading latest artifact...";
        StartCoroutine(LoadLatestArtifactCoroutine());
    }

    // set input to read only
    private void SetFieldsReadOnly(bool readOnly)
    {
        TMP_InputField[] fields =
        {
            DateField, InvestigatorField, AreaField, UnitField,
            LayerField, SiteField, AssociatedFeaturesField,
            MaterialTypeField, QuantityField, WeightField,
            BagNumberField, ArtifactIDField
        };

        foreach ( var field in fields )
        {
            if( field == null ) continue;
            field.readOnly = readOnly;
            field.interactable = !readOnly;
        }
    }

    // load artifact from database
    private IEnumerator LoadLatestArtifactCoroutine()
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
                    $"Error fetching artifact: " +
                    $"{request.responseCode} - {request.error} - {serverText}");

                StatusText.text = "Error loading artifact data.";
                yield break;
            }

            string json = request.downloadHandler.text;

        #if UNITY_EDITOR
            Debug.Log($"Artifact analysis JSON: {json}");
        #endif

            ArtifactResponse response =
                JsonUtility.FromJson<ArtifactResponse>(json);

            if (response == null || !response.ok || response.artifact == null)
            {
                StatusText.text =
                    response != null && !string.IsNullOrEmpty(response.error)
                        ? $"Server error: {response.error}"
                        : "No artifact data available.";
                yield break;
            }

            // success
            PopulateUI(response.artifact);
            StatusText.text = "Artifact data loaded.";
        }
    }

    // populate input fields
    private void PopulateUI(Artifact artifact)
    {
        DateField.text = artifact.date_discovered;
        InvestigatorField.text = artifact.investigator;
        AreaField.text = artifact.area;
        UnitField.text = artifact.unit;
        LayerField.text = artifact.layer;
        SiteField.text = artifact.site;
        AssociatedFeaturesField.text = artifact.associated_features;
        MaterialTypeField.text = artifact.material_type;
        QuantityField.text = artifact.quantity.ToString();
        WeightField.text = artifact.weight.ToString("0.##");
        BagNumberField.text = artifact.bag_number;
        ArtifactIDField.text = artifact.artifact_id;
    }
}