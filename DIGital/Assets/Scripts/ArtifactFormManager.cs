using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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
    [SerializeField] private TMP_InputField ArtifactIDInput;

    // popup panel game object  
    [Header("UI Feedback")]
    [SerializeField] private TMP_Text StatusText;
    [SerializeField] private GameObject PanelRoot;

    // api settings
    [Header("API Settings")]
    [SerializeField] private string apiUrl = 
                    "http://localhost:4000/api/artifacts";


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
    }

    // submit button clicked
    public void OnSubmitButtonClicked()
    {
        // front end validation
        if (!ValidateInputs(out int Quantity))
        {
            return;
        }

        StatusText.text = "Submitting artifact...";

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
            artifact_id = ArtifactIDInput.text.Trim()
        };

        StartCoroutine(SubmitArtifactCoroutine(data));
    }

    // cancel button clicked
    public void OnCancelButtonClicked()
    {
        // TEMP: clear the form, later it will close the pop-up overlay
        ClearForm();

        StatusText.text = "";
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
            string.IsNullOrWhiteSpace(BagNumberInput.text) ||
            string.IsNullOrWhiteSpace(ArtifactIDInput.text))
        {
            StatusText.text = "Please fill in all required fields.";
            return false;
        }

        // quantity validation
        if (!int.TryParse(QuantityInput.text.Trim(), out Quantity))
        {
            StatusText.text = "Quantity must be a valid integer (whole number).";
            return false;
        }

        return true;
    }

    // submit artifact coroutine
    private IEnumerator SubmitArtifactCoroutine(ArtifactData artifactData)
    {
        StatusText.text = "Submitting artifact to database...";

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

                StatusText.text = "Artifact submitted successfully!";

                ClearForm();
            }

            else
            {
                string serverText = request.downloadHandler != null
                    ? request.downloadHandler.text
                    : "";

                Debug.LogError($"Error submitting artifact: " +
                            $"{request.responseCode} - " +
                            $"{request.error} - {serverText}");

                StatusText.text =
                    "Error submitting artifact: " +
                    (!string.IsNullOrEmpty(serverText)
                        ? serverText
                        : request.error);
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
        ArtifactIDInput.text = "";
    }
}