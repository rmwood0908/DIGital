using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class DiggableEarth : MonoBehaviour, Interactable
{
    [SerializeField] private TMP_Text textBox;
    private GameManager manager;
    private float textDisplayedTime = 0;
    private bool checkForText = false;

    private int digLayer;

    // added localization for backfill (spanish)
    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string digTipKey = "walk_and_excavate_dig";
    [SerializeField] private string surveyBlockedKey = "walk_and_excavate_complete_survey_first";

    void Start()
    {
        digLayer = GetComponentInParent<DiggableEarthLayer>().digLayer;
        manager = GetComponentInParent<DiggableEarthLayer>().manager;
    }

    public void Interact()
    {
        if (manager == null)
        {
            return;
        }

        // block excavation until pedestrian survey is complete
        if (!manager.CanExcavate)
        {
            textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, surveyBlockedKey);
            return;
        }

        if (manager.currentLayer == digLayer)
        {
            textBox.text = "";
            manager.dirtPile.increaseSize();
            
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (checkForText)
        {
            if(textDisplayedTime > 0)
            {
                // switcfh text depending on if excavation is allowed
                if (!manager.CanExcavate)
                {
                    textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, surveyBlockedKey);
                }
                else if (manager.currentLayer == digLayer)
                {
                    textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, digTipKey);
                }
                else
                {
                    textBox.text = "";
                }
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
        if (manager == null)
        {
            return;
        }

        textDisplayedTime = 0.03f;
        checkForText = true;
    }
}
