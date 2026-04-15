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

    private readonly List<GameObject> spawnedSurveyFlags = new List<GameObject>();

    // track diggable tile and number of recordered vs. required artifacts
    public bool CanExcavate => recordedSurfaceArtifacts >= requiredSurfaceArtifacts;
    public int RecordedSurfaceArtifacts => recordedSurfaceArtifacts;
    public int RequiredSurfaceArtifacts => requiredSurfaceArtifacts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (excavationBoundary != null)
        {
            excavationBoundary.SetActive(false);
        }

        if (surveyProgressUI != null)
        {
            surveyProgressUI.Refresh(recordedSurfaceArtifacts, requiredSurfaceArtifacts);
        }
    }

    // track spawned flags
    public void RegisterSpawnedSurveyFlag(GameObject flag)
    {
        if (flag == null)
        {
            return;
        }

        spawnedSurveyFlags.Add(flag);
    }

    // track surface artifact recording
    public void RegisterSurfaceArtifactRecorded()
    {
        recordedSurfaceArtifacts++;

        if (recordedSurfaceArtifacts > requiredSurfaceArtifacts)
        {
            recordedSurfaceArtifacts = requiredSurfaceArtifacts;
        }

        Debug.Log($"[GameManager] Surface artifacts recorded: {recordedSurfaceArtifacts}/{requiredSurfaceArtifacts}");

        if (surveyProgressUI != null)
        {
            surveyProgressUI.Refresh(recordedSurfaceArtifacts, requiredSurfaceArtifacts);
        }

        if (recordedSurfaceArtifacts >= requiredSurfaceArtifacts)
        {
            OnSurfaceSurveyCompleted();
        }
    }

    // after all artifacts have been recorded
    private void OnSurfaceSurveyCompleted()
    {
        // hide all spawned red flags
        foreach (GameObject flag in spawnedSurveyFlags)
        {
            if (flag != null)
            {
                flag.SetActive(false);
            }
        }

        // show the pre-placed excavation boundary
        if (excavationBoundary != null)
        {
            excavationBoundary.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentLayer == 7)
        {
            currentStep++;
            currentLayer = -1;
        }
    }
}
