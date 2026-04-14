using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class DroneInteractable : MonoBehaviour, Interactable
{
    [Header("UI")]
    [SerializeField] private TMP_Text textBox;

    [Header("Survey")]
    [SerializeField] private SurveyModeManager surveyModeManager;

    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string tooltipKey = "walk_and_excavate_drone_tooltip";

    [Header("Tooltip Timing")]
    [SerializeField] private float tooltipDuration = 0.03f;

    private float textDisplayedTime = 0f;
    private bool checkForText = false;

    public void Interact()
    {
        if (surveyModeManager == null)
        {
            Debug.LogWarning("[DroneInteractable] SurveyModeManager is not assigned.");
            return;
        }

        textBox.text = "";
        surveyModeManager.EnterSurveyMode();
    }

    private void Update()
    {
        if (checkForText)
        {
            if (textDisplayedTime > 0f)
            {
                textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, tooltipKey);
                textDisplayedTime -= Time.deltaTime;
            }
            else
            {
                textBox.text = "";
                checkForText = false;
            }
        }
    }

    public void displayTooltip()
    {
        if (WalkExcavateIntroController.IsIntroOpen)
            return;

        textDisplayedTime = tooltipDuration;
        checkForText = true;
    }
}