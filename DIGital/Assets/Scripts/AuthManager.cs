using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Server Settings")]
    public string serverBaseUrl = "https://digital-ty59.onrender.com";

    [Header("Scenes")]
    public string excavationSceneName = "Walk&Excavate";

    // called by Login button
    public void OnLoginButtonClicked()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;

        StartCoroutine(LoginCoroutine(username, password));
    }

    // called by Create Account button
    public void OnSignupButtonClicked()
    {
        string email = EmailInput.text;
        string username = CreateUsernameInput.text;
        string password = CreatePasswordInput.text;

        StartCoroutine(SignupCoroutine(email, username, password));
    }

    // coroutine for login
    IEnumerator LoginCoroutine(string username, string password)
    {
        string url = $"{serverBaseUrl}/api/auth/login";
        Debug.Log($"[AuthManager] Login URL: {url}");
        Debug.Log($"[AuthManager] Username entered: '{username}' (len={username?.Length ?? 0})");

        // Extra safety: if serverBaseUrl is empty in Inspector, warn loudly
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
            req.uploadHandler   = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("[AuthManager] Sending login request...");

            // NO try/catch around yield — this avoids CS1626
            yield return req.SendWebRequest();

            Debug.Log($"[AuthManager] Request finished. " +
                    $"Result={req.result}, Code={req.responseCode}");

            string responseText = req.downloadHandler != null
                ? req.downloadHandler.text
                : "(no downloadHandler)";

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AuthManager] Login error: {req.responseCode} - {req.error}");
                Debug.LogError($"[AuthManager] Server response text: {responseText}");
                yield break;
            }

            Debug.Log("[AuthManager] Login raw response: " + responseText);

            LoginResponse response = null;
            try
            {
                response = JsonUtility.FromJson<LoginResponse>(responseText);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[AuthManager] EXCEPTION parsing JSON: " + ex);
                yield break;
            }

            if (response != null && response.ok)
            {
                Debug.Log("[AuthManager] Login successful, userId = " + response.userId);

                if (SessionManager.Instance != null)
                {
                    SessionManager.Instance.SetUser(response.userId, username);
                }
                else
                {
                    Debug.LogWarning("[AuthManager] SessionManager.Instance is null in login scene.");
                }

                Debug.Log("[AuthManager] Loading excavation scene: " + excavationSceneName);
                SceneManager.LoadScene(excavationSceneName);
            }
            else
            {
                Debug.LogError("[AuthManager] Login failed: " +
                            (response != null ? response.error : "Invalid or null JSON"));
            }
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

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Signup error: {req.responseCode} - {req.error}");
                Debug.LogError($"Server response: {req.downloadHandler.text}");
                yield break;
            }

            Debug.Log("Signup raw response: " + req.downloadHandler.text);

            SignupResponse response = 
                          JsonUtility.FromJson<SignupResponse>(req.downloadHandler.text);

            if( response != null && response.ok )
            {
                Debug.Log("Signup successful for: " + response.user.username);

                if (SessionManager.Instance != null )
                {
                    SessionManager.Instance.SetUser(response.user.user_id,
                                                    response.user.username);
                }

                // auto login AKA go to scene
                SceneManager.LoadScene(excavationSceneName);
            }

            else
            {
                Debug.LogError("Signup failed: " +
                              (response != null ? response.error : "Invalid JSON"));
            }
        }
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

    [System.Serializable]
    public class LoginResponse
    {
        public bool ok;
        public string userId;
        public string error;
    }

    [System.Serializable]
    public class SignupResponse
    {
        public bool ok;
        public SignupUser user;
        public string error;
    }

    [System.Serializable]
    public class SignupUser
    {
        // use same field names Node sends: user_id, email, username, created_at
        public string user_id;
        public string email;
        public string username;
        public string created_at;
    }
}