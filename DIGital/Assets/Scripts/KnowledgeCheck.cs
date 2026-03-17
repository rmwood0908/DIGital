using UnityEngine;
using UnityEngine.UI;

public class KnowledgeCheck : MonoBehaviour
{
    static public KnowledgeCheck instance;

    public FirstPersonController fpsController;
    public AssistantUI assistantUI;

    // question ui elements
    public GameObject quizUI;
    public Text questionText;
    public Button[] answerButtons;
    public Button nextButton;
    public Button closeButton;

    private Question currentQuestion;
    private int correctAnswerIndex;
    private bool questionComplete;
    private ColorBlock defaultButtonColors;
    private ColorBlock disabledIncorrectColors;
    private ColorBlock disabledCorrectColors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            defaultButtonColors = answerButtons[0].colors;
            disabledIncorrectColors = defaultButtonColors;
            disabledCorrectColors = defaultButtonColors;
            disabledCorrectColors.disabledColor = Color.green;
            disabledIncorrectColors.disabledColor = Color.red;
        }
        else
        {
            Debug.Log("Multiple instances of KnowledgeCheck detected! Destroying duplicate.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // call this method to ask a question
    public void AskQuestion(Question question)
    {
        // disable playermovement
        fpsController.enabled = false;

        // disable ai assistant
        assistantUI.enabled = false;

        // update question and show question UI
        currentQuestion = question;
        questionComplete = false;
        questionText.text = question.questionText;

        // deactivate all answer buttons
        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        // randomly assign the correct answer to one of the buttons and activate it
        int randomIndex = Random.Range(0, question.numOptions);
        answerButtons[randomIndex].gameObject.SetActive(true);
        answerButtons[randomIndex].interactable = true;
        answerButtons[randomIndex].colors = defaultButtonColors;
        answerButtons[randomIndex].GetComponentInChildren<Text>().text = question.correctAnswer;
        correctAnswerIndex = randomIndex;

        // assign the other answers to the remaining buttons
        int buttonIndex = 0;
        foreach (string incorrectAnswer in question.incorrectAnswers)
        {
            if (buttonIndex == randomIndex)
            {
                buttonIndex++;
            }
            answerButtons[buttonIndex].gameObject.SetActive(true);
            answerButtons[buttonIndex].interactable = true;
            answerButtons[buttonIndex].colors = defaultButtonColors;
            answerButtons[buttonIndex].GetComponentInChildren<Text>().text = incorrectAnswer;
            buttonIndex++;
        }

        quizUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void CheckAnswer(int index)
    {
        if (currentQuestion.revealAnswerOnIncorrect)
        {
            // reveal correct answer by changing its button color to green
            answerButtons[index].colors = disabledIncorrectColors;
            answerButtons[correctAnswerIndex].colors = disabledCorrectColors;
            questionComplete = true;
        }
        else
        {
            if (index == correctAnswerIndex)
            {
                // Answer is correct
                answerButtons[index].colors = disabledCorrectColors;
                questionComplete = true;
            }
            else
            {
                // Answer is incorrect
                answerButtons[index].colors = disabledIncorrectColors;
                answerButtons[index].interactable = false; // disable the button for the incorrect answer
            }
        }

        if (questionComplete)
        {
            // disable all buttons
            foreach (Button button in answerButtons)
            {
                button.interactable = false;
            }

            // make next button active if there is a next question
            if (currentQuestion.nextQuestion != null)
            {
                nextButton.gameObject.SetActive(true);
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() => AskQuestion(currentQuestion.nextQuestion));
            }
            // otherwise, assume no more questions and re-enable player movement and ai assistant
            else
            {
                closeButton.gameObject.SetActive(true);
            }
        }
    }

    // call this method to close the question UI and re-enable player movement and ai assistant
    public void Close()
    {
        nextButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        fpsController.enabled = true;
        assistantUI.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        quizUI.SetActive(false);
    }
}
