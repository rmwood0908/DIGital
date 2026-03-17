using UnityEngine;

public class DiggableEarthLayer : MonoBehaviour
{
    [SerializeField] public int digLayer;
    [SerializeField] public GameManager manager;
    public Question Question;

    void Update()
    {
        if(transform.childCount == 0)
        {
            manager.currentLayer++;

            // check if last layer was dug through
            if(digLayer == 0)
            {
                // ask a question
                KnowledgeCheck.instance.AskQuestion(Question);
            }

            Destroy(gameObject);
        }
    }
}
