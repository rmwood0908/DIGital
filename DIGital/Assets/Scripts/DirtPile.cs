using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class DirtPile : MonoBehaviour, Interactable
{
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private GameManager manager;
    [SerializeField] private GameObject refillLayer;

    private float textDisplayedTime = 0;
    private bool checkForText = false;

    [SerializeField] private float initialScalar;
    [SerializeField] private float scalarFalloff;

    [SerializeField] private string analysisSceneName = "ArtifactAnalysis";

    // added localization for backfill (spanish)
    [Header("Localization")]
    [SerializeField] private string table = "UI";
    [SerializeField] private string backfillTipKey = "walk_and_excavate_backfill";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (manager.currentStep == 3)
        {
            textBox.text = "";
            Instantiate(refillLayer, new Vector3(-112.5f, -0.125f, -62.5f), Quaternion.identity);

            Destroy(gameObject);

            SceneManager.LoadScene(analysisSceneName);
        }
    }

    void FixedUpdate()
    {
        if(checkForText)
        {
            if(textDisplayedTime > 0)
            {
                textBox.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, backfillTipKey);
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
        if (manager.currentStep == 3)
        {
            textDisplayedTime = 0.03f;
            checkForText = true;
        }
    }

    public void increaseSize()
    {
        if(transform.localScale == Vector3.zero)
        {
            transform.localScale = Vector3.one;
        }

        else
        {
            transform.localScale = transform.localScale * initialScalar;

            initialScalar = ((initialScalar - 1f ) * scalarFalloff) + 1f;
        }
    }
}
