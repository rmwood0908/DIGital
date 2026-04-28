using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int currentStep = 2;
    public int currentLayer = 0;
    public int maxLayers = 7;

    [SerializeField] public DirtPile dirtPile;

    [Header("Surface Survey Artifacts")]
    [SerializeField] private int requiredSurfaceArtifacts = 6;
    [SerializeField] private int recordedSurfaceArtifacts = 0;

    [Header("Surface Survey UI")]
    [SerializeField] private SurfaceSurveyProgressUI surveyProgressUI;

    [Header("Scene Objects")]
    [SerializeField] private GameObject excavationBoundary;

    // ── Unit Marker reference ────────────────────────────────────────────────
    [Header("Unit Marking")]
    [SerializeField] private UnitMarkerSystem unitMarkerSystem;

    private readonly List<GameObject> spawnedSurveyFlags = new List<GameObject>();

    // CanExcavate: survey done AND (no unit system OR player is in a marked unit)
    public bool CanExcavate =>
        recordedSurfaceArtifacts >= requiredSurfaceArtifacts &&
        (unitMarkerSystem == null || unitMarkerSystem.CanPlayerDig);

    public int RecordedSurfaceArtifacts => recordedSurfaceArtifacts;
    public int RequiredSurfaceArtifacts => requiredSurfaceArtifacts;

    void Start()
    {
        if (excavationBoundary != null)
            excavationBoundary.SetActive(false);

        if (surveyProgressUI != null)
            surveyProgressUI.Refresh(recordedSurfaceArtifacts, requiredSurfaceArtifacts);
    }

    public void RegisterSpawnedSurveyFlag(GameObject flag)
    {
        if (flag == null) return;
        spawnedSurveyFlags.Add(flag);
    }

    public void RegisterSurfaceArtifactRecorded()
    {
        recordedSurfaceArtifacts++;
        if (recordedSurfaceArtifacts > requiredSurfaceArtifacts)
            recordedSurfaceArtifacts = requiredSurfaceArtifacts;

        Debug.Log($"[GameManager] Surface artifacts recorded: {recordedSurfaceArtifacts}/{requiredSurfaceArtifacts}");

        if (surveyProgressUI != null)
            surveyProgressUI.Refresh(recordedSurfaceArtifacts, requiredSurfaceArtifacts);

        if (recordedSurfaceArtifacts >= requiredSurfaceArtifacts)
            OnSurfaceSurveyCompleted();
    }

    private void OnSurfaceSurveyCompleted()
    {
        foreach (GameObject flag in spawnedSurveyFlags)
            if (flag != null) flag.SetActive(false);

        if (excavationBoundary != null)
            excavationBoundary.SetActive(true);

        // Begin unit marking phase
        if (unitMarkerSystem != null)
            unitMarkerSystem.BeginUnitMarkingPhase();
        else
            Debug.LogWarning("[GameManager] UnitMarkerSystem not assigned.");
    }

    void Update()
    {
        if (currentLayer == 7)
        {
            currentStep++;
            currentLayer = -1;
        }
    }
}