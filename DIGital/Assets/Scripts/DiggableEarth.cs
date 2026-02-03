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

    void Start()
    {
        digLayer = GetComponentInParent<DiggableEarthLayer>().digLayer;
        manager = GetComponentInParent<DiggableEarthLayer>().manager;
    }

    public void Interact()
    {
        if (manager.currentLayer == digLayer)
        {
            textBox.text = "";
            manager.dirtPile.increaseSize();
            
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if(checkForText)
        {
            if(textDisplayedTime > 0)
            {
                textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, digTipKey);
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
        if (manager.currentLayer == digLayer)
        {
            textDisplayedTime = 0.03f;
            checkForText = true;
        }
    }
}
