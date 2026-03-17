using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentStep = 2;
    public int currentLayer = 0;
    public int maxLayers = 7;

    [SerializeField] public DirtPile dirtPile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
