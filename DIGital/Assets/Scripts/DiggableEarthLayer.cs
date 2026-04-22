using UnityEngine;

public class DiggableEarthLayer : MonoBehaviour
{
    [SerializeField] public int digLayer;
    [SerializeField] public GameManager manager;
    public Question Question;

    void Update()
    {
        if (transform.childCount == 0)
        {
            manager.currentLayer++;

            if (digLayer == 0)
            {
                KnowledgeCheck.instance.AskQuestion(Question);
            }

            Destroy(gameObject);
        }
    }
}