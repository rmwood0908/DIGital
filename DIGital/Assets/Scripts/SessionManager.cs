using UnityEngine;

// class to remember user information between scene
public class SessionManager: MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    // to be filled after login/singup
    public string UserId { get; private set; }
    public string Username { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

    // carry information across scenes
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetUser(string userId, string username)
    {
        UserId = userId;
        Username = username;
        Debug.Log($"[SessionManager] Logged in as {username} ({userId})");
    }

    public void ClearUser()
    {
        UserId = null;
        Username = null;
        Debug.Log("[SessionManager] Cleared session");
    }
}