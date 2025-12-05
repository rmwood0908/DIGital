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
        // uses existing .js code for login route
        string url = $"{serverBaseUrl}/api/auth/login";
        Debug.Log($"Login URL: {url}");

        // create request data 
        LoginRequest data = new LoginRequest
        {
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
                Debug.LogError($"Login error: {req.responseCode} - {req.error}");
                Debug.LogError($"Server response: {req.downloadHandler.text}");
                yield break;
            }

            Debug.Log("Login raw response: " + req.downloadHandler.text);

            // parse JSON
            LoginResponse response = 
                          JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);

            if (response != null && response.ok)
            {
                Debug.Log("Login successful, userId = " + response.userId);
                // TODO: store userId for later user

                // go to excavation scene
                SceneManager.LoadScene(excavationSceneName); 
            }

            else
            {
                Debug.LogError("Login failed: " + 
                              (response != null ? response.error : "Invalid JSON"));
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
        public int userId;
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
        public int user_id;
        public string email;
        public string username;
        public string created_at;
    }
}