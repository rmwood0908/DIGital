using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiggableEarth : MonoBehaviour, Interactable
{
    [SerializeField] private TMP_Text textBox;
    private GameManager manager;
    private float textDisplayedTime = 0;
    private bool checkForText = false;

    private int digLayer;

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
                textBox.text = "Dig (Left Click)";
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
