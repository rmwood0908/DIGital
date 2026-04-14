using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AuthApiResponse
{
    public bool ok;
    public string error;
    public string message;
    public string userId;
    public string username;
    public string email;
    public SignupUser user;
}

[System.Serializable]
public class SignupUser
{
    public string user_id;
    public string email;
    public string username;
    public string created_at;
}

// authentication manager class for login and signup
public class AuthManager : MonoBehaviour
{
    // login panel
    [Header("Login UI")]
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;

    // create account panel
    [Header("Signup UI")]
    public TMP_InputField EmailInput;
    public TMP_InputField CreateUsernameInput;
    public TMP_InputField CreatePasswordInput;

    [Header("Status UI")]
    [SerializeField] private TMP_Text authStatusText;
    [SerializeField] private GameObject authStatusBox;

    [Header("Server Settings")]
    public string serverBaseUrl = "https://digital-ty59.onrender.com";

    [Header("Scenes")]
    public string excavationSceneName = "Walk&Excavate";

    private void Start()
    {
        ClearStatus();
    }

    private void ClearStatus()
    {
        authStatusText.text = "";

        if (authStatusBox != null)
        {
            authStatusBox.gameObject.SetActive(false);
        }
    }

    private void ShowStatus(string message)
    {
        authStatusText.text = message;

        if (authStatusBox != null)
        {
            authStatusBox.gameObject.SetActive(true);
        }

        else
        {
            Debug.LogWarning("[AuthManager] authStatusBox  is not assigned in the Inspector.");
        }
    }

    // called by Login button
    public void OnLoginButtonClicked()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;

        ClearStatus();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowStatus("Please enter both username and password.");
            return;
        }

        StartCoroutine(LoginCoroutine(username, password));
    }

    // called by Create Account button
    public void OnSignupButtonClicked()
    {
        string email = EmailInput.text;
        string username = CreateUsernameInput.text;
        string password = CreatePasswordInput.text;

        ClearStatus();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ShowStatus("Please fill in all required fields.");
            return;
        }

        StartCoroutine(SignupCoroutine(email, username, password));
    }

    // coroutine for login
    IEnumerator LoginCoroutine(string username, string password)
    {
        string url = $"{serverBaseUrl}/api/auth/login";
        Debug.Log($"[AuthManager] Login URL: {url}");
        Debug.Log($"[AuthManager] Username entered: '{username}' (len={username?.Length ?? 0})");

        if (string.IsNullOrWhiteSpace(serverBaseUrl))
        {
            Debug.LogError("[AuthManager] serverBaseUrl is EMPTY. " +
                        "Set it in the Inspector to " +
                        "\"https://digital-ty59.onrender.com\".");
            yield break;
        }

        LoginRequest data = new LoginRequest
        {
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(data);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("[AuthManager] Sending login request...");
            yield return req.SendWebRequest();

            string responseText = req.downloadHandler != null
                ? req.downloadHandler.text
                : "";

            Debug.Log($"[AuthManager] Login finished. " +
                    $"Result={req.result}, Code={req.responseCode}");
            Debug.Log($"[AuthManager] Login raw response: {responseText}");

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"[AuthManager] Login transport error: {req.error}");
                ShowStatus("Unable to reach server. Please try again.");
                yield break;
            }

            AuthApiResponse response = TryParseResponse(responseText);

            // 401/400/etc. should still show backend message if present.
            if (req.responseCode >= 400 || response == null || !response.ok)
            {
                string errorMessage = GetBestErrorMessage(
                    response,
                    "Incorrect username/password"
                );

                Debug.LogError("[AuthManager] Login failed: " + errorMessage);
                ShowStatus(errorMessage);
                yield break;
            }

            string returnedUsername = !string.IsNullOrWhiteSpace(response.username)
                ? response.username
                : username;

            Debug.Log("[AuthManager] Login successful, userId = " + response.userId);

            if (SessionManager.Instance != null)
            {
                SessionManager.Instance.SetUser(response.userId, returnedUsername);
            }

            else
            {
                Debug.LogWarning("[AuthManager] SessionManager.Instance is null in login scene.");
            }

            SceneManager.LoadScene(excavationSceneName);
        }
    }

    // coroutine for signup
    IEnumerator SignupCoroutine(string email,
                                string username,
                                string password)
    {
        // uses existing .js code for signup route
        string url = $"{serverBaseUrl}/api/auth/signup";
        Debug.Log($"Signup URL: {url}");

        if (string.IsNullOrWhiteSpace(serverBaseUrl))
        {
            Debug.LogError("[AuthManager] serverBaseUrl is EMPTY. Set it in the Inspector.");
            ShowStatus("Server URL is not configured.");
            yield break;
        }

        // create request data
        SignupRequest data = new SignupRequest
        {
            email = email,
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(data);
        byte[] body = Encoding.UTF8.GetBytes(json);

        // send POST request
        using (UnityWebRequest req =
               new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("[AuthManager] Sending signup request...");
            yield return req.SendWebRequest();

            string responseText = req.downloadHandler != null
                ? req.downloadHandler.text
                : "";

            Debug.Log($"[AuthManager] Signup finished. Result={req.result}, Code={req.responseCode}");
            Debug.Log($"[AuthManager] Signup raw response: {responseText}");

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"[AuthManager] Signup transport error: {req.error}");
                ShowStatus("Unable to reach server. Please try again.");
                yield break;
            }

            AuthApiResponse response = TryParseResponse(responseText);

            if (req.responseCode >= 400 || response == null || !response.ok)
            {
                string errorMessage = GetBestErrorMessage(
                    response,
                    "Email/Username already taken"
                );

                Debug.LogError("[AuthManager] Signup failed: " + errorMessage);
                ShowStatus(errorMessage);
                yield break;
            }

            if (response.user != null)
            {
                Debug.Log("[AuthManager] Signup successful for: " + response.user.username);

                if (SessionManager.Instance != null)
                {
                    SessionManager.Instance.SetUser(
                        response.user.user_id,
                        response.user.username
                    );
                }

                SceneManager.LoadScene(excavationSceneName);
            }

            else
            {
                Debug.LogError("[AuthManager] Signup succeeded but user payload was missing.");
                ShowStatus("Signup failed.");
            }
        }
    }

    private AuthApiResponse TryParseResponse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonUtility.FromJson<AuthApiResponse>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[AuthManager] JSON parse error: " + ex.Message);
            return null;
        }
    }

    private string GetBestErrorMessage(AuthApiResponse response, string fallback)
    {
        if (response != null)
        {
            if (!string.IsNullOrWhiteSpace(response.error))
            {
                return response.error;
            }

            if (!string.IsNullOrWhiteSpace(response.message))
            {
                return response.message;
            }
        }

        return fallback;
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class SignupRequest
    {
        public string email;
        public string username;
        public string password;
    }
}