using UnityEngine;
using UnityEngine.UI;
using LLMUnity;

public class AssistantUI : MonoBehaviour
{
    public GameObject ui;

    // AI STUFF
    public LLMCharacter llmCharacter;
    public InputField input;
    public Text AIText;

    // RAG STUFF
    public RAG rag;
    [TextArea(3,10)]
    public string data;

    public void Start()
    {
        // create RAG embeddings from data
        ChunkData();
    }

    public void ToggleAssistant()
    {
        ui.SetActive(!ui.activeSelf);
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
