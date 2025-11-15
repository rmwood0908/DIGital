using UnityEngine;

public class DiggableEarthLayer : MonoBehaviour
{
    [SerializeField] public int digLayer;
    [SerializeField] public GameManager manager;

    void Update()
    {
        if(transform.childCount == 0)
        {
            manager.currentLayer++;
            Destroy(gameObject);
        }
    }
}
