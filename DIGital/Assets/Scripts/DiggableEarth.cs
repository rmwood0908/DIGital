using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class DiggableEarth : MonoBehaviour, Interactable
{
    [SerializeField] private TMP_Text textBox;
    private GameManager manager;
    private DiggableEarthLayer parentLayer;
    private float textDisplayedTime = 0;
    private bool checkForText = false;
    private int digLayer;

    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string digTipKey = "walk_and_excavate_dig";
    [SerializeField] private string surveyBlockedKey = "walk_and_excavate_complete_survey_first";
    [SerializeField] private string unitMarkingBlockedKey = "walk_and_excavate_mark_unit_first";

    void Start()
    {
        parentLayer = GetComponentInParent<DiggableEarthLayer>();
        digLayer = parentLayer.digLayer;
        manager = parentLayer.manager;
    }

    public void Interact()
    {
        if (manager == null) return;

        // Block 1: pedestrian survey not done
        if (manager.RecordedSurfaceArtifacts < manager.RequiredSurfaceArtifacts)
        {
            textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, surveyBlockedKey);
            return;
        }

        // Block 2: survey done but this section is not in a marked unit
        if (UnitMarkerSystem.Instance != null && UnitMarkerSystem.Instance.IsActive
            && !UnitMarkerSystem.Instance.IsSectionMarked(transform.position))
        {
            textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, unitMarkingBlockedKey);
            return;
        }

        // Block 3: must dig layers from top down — layer above must be cleared first
        if (UnitMarkerSystem.Instance != null && UnitMarkerSystem.Instance.IsActive)
        {
            if (!IsLayerAboveCleared())
            {
                textBox.text = "Dig the layer above first.";
                return;
            }
        }

        // Block 4: check layer — bypass global currentLayer check if unit system is active
        bool unitSystemActive = UnitMarkerSystem.Instance != null && UnitMarkerSystem.Instance.IsActive;

        if (unitSystemActive || manager.currentLayer == digLayer)
        {
            textBox.text = "";
            manager.dirtPile.increaseSize();

            if (UnitMarkerSystem.Instance != null)
                UnitMarkerSystem.Instance.NotifySectionFullyExcavated(transform.position, digLayer);

            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (checkForText)
        {
            if (textDisplayedTime > 0)
            {
                if (manager.RecordedSurfaceArtifacts < manager.RequiredSurfaceArtifacts)
                    textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, surveyBlockedKey);
                else if (UnitMarkerSystem.Instance != null && UnitMarkerSystem.Instance.IsActive
                         && !UnitMarkerSystem.Instance.IsSectionMarked(transform.position))
                    textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, unitMarkingBlockedKey);
                else if (manager.currentLayer == digLayer)
                    textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, digTipKey);
                else
                    textBox.text = "";
            }
            else
            {
                textBox.text = "";
                checkForText = false;
            }

            textDisplayedTime -= Time.deltaTime;
        }
    }

    public void displayTooltip()
    {
        if (manager == null) return;
        textDisplayedTime = 0.03f;
        checkForText = true;
    }

    private bool IsLayerAboveCleared()
    {
        if (UnitMarkerSystem.Instance == null) return true;
        if (digLayer <= 0) return true; // top layer, nothing above

        Vector2Int myCell = UnitMarkerSystem.Instance.WorldToCellPublic(transform.position);

        DiggableEarth[] allSections = FindObjectsByType<DiggableEarth>(FindObjectsSortMode.None);
        foreach (var section in allSections)
        {
            if (section == this) continue;

            // Is this section in the same unit?
            Vector2Int otherCell = UnitMarkerSystem.Instance.WorldToCellPublic(section.transform.position);
            if (!UnitMarkerSystem.Instance.IsSectionMarked(section.transform.position)) continue;

            // Does it have a lower digLayer (meaning it's above us in the unit)?
            DiggableEarthLayer parentL = section.GetComponentInParent<DiggableEarthLayer>();
            if (parentL != null && parentL.digLayer < digLayer)
                return false; // a higher layer still exists somewhere in the unit
        }
        return true;
    }
}