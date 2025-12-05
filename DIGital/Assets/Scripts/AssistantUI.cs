using UnityEngine;
using UnityEngine.UI;
using LLMUnity;

public class AssistantUI : MonoBehaviour
{
    public GameObject ui;

    public FirstPersonController fpsController;

    // AI STUFF
    public LLMCharacter llmCharacter;
    public InputField input;
    public Text AIText;

    // RAG STUFF
    public RAG rag;
    [TextArea(3,10)]
    public string data;

    // debug / options
    public KeyCode toggleKey = KeyCode.Tab;

    public void Start()
    {
        // start with UI closeed
        if( ui != null )
        {
            ui.SetActive(false);
        }

        // create RAG embeddings from data
        ChunkData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleAssistant();
        }
    }

    public void ToggleAssistant()
    {
        Debug.Log("[AssistantUI] ToggleAssistant called");

        if (ui == null)
        {
            Debug.LogWarning("[AssistantUI] ui is not assigned!");
            return;
        }

        bool newState = !ui.activeSelf;
        Debug.Log("[AssistantUI] ui active before toggle: " + ui.activeSelf);
        ui.SetActive(newState);

        if (newState)
        {
            // OPEN: unlock cursor & optionally freeze FPS controller
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;

            if (fpsController != null)
            {
                fpsController.enabled = false;
            }
        }
        else
        {
            // CLOSE: lock cursor again & re-enable FPS controller
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;

            if (fpsController != null)
            {
                fpsController.enabled = true;
            }
        }
    }

    public async void onInputFieldSubmit(string message)
    {
        input.interactable = false;
        AIText.text = "...";
        // Debug.Log("User: " + message);
        
        
        // search RAG for revevant data
        (string[] similarPhrases, float[] distances) = await rag.Search(message, 1);
        string prompt = "Answer the user query based on the provided data.\n\n";
        prompt += $"User query: {message}\n\n";
        prompt += $"Data:\n";
        foreach (string similarPhrase in similarPhrases) prompt += $"\n- {similarPhrase}";

        // Debug.Log("Prompt to LLM: " + prompt);
        _ = llmCharacter.Chat(prompt, SetAIText, AIReplyComplete);
    }

    public void SetAIText(string text)
    {
        AIText.text = text;
    }

    public void AIReplyComplete()
    {
        input.interactable = true;
        input.Select();
        input.text = "";
    }

    // RAG STUFF
    async void ChunkData()
    {
        // add the data to the RAG
        await rag.Add(data);
    }
}
