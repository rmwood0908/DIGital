using TMPro;
using UnityEngine;

// class
public class ExcavationUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text WelcomeText;

    private void Start()
    {
        if (welcomeText == null)
        {
            Debug.LogWarning("[ExcavationUIManager] Welcome Text is not assigned.");
            return;
        }

        if (SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn)
        {
            string username = SessionManager.Instance.Username;
            welcomeText.text = $"Welcome, {username}!";
        }
        else
        {
            welcomeText.text = "Welcome, archaeologist!";
        }
    }
}