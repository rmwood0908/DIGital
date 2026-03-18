using UnityEngine;

// scriptable object to hold question data for knowledge checks
// to create one, go to Assets -> Create -> Scriptable Objects -> Question
// and insert question data into the inspector for new question scriptable object
[CreateAssetMenu(fileName = "Question", menuName = "Scriptable Objects/Question")]
public class Question : ScriptableObject
{
    public string questionText;
    [Tooltip("Check to reveal the correct answer if the user gets it wrong")]
    public bool revealAnswerOnIncorrect;
    public int numOptions;
    public string correctAnswer;
    public string[] incorrectAnswers;
    public Question nextQuestion;

    private void OnValidate()
    {
        // make sure numOptions is max 4 and that incorrectAnswers array is the correct size
        if (numOptions > 4)
        {
            numOptions = 4;
        }
        int desiredSize = Mathf.Max(0, numOptions - 1);

        if (incorrectAnswers == null || incorrectAnswers.Length != desiredSize)
        {
            System.Array.Resize(ref incorrectAnswers, desiredSize);
        }
    }
}
