using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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
    public string serverBaseUrl = "http://localhost:4000";

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
        string url = serverBaseUrl + "/api/auth/login";

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
                Debug.LogError("Login error: " + 
                                req.responseCode + " " + req.error);
                Debug.LogError("Server says: " + req);
            }
            else
            {
                Debug.Log("Login response: " + req.downloadHandler.text);
                // TODO: parse JSON and move to next scene if ok
            }
        }
    }

    // coroutine for signup
    IEnumerator SignupCoroutine(string email,
                                string username,
                                string password)
    {
        // uses existing .js code for signup route
        string url = serverBaseUrl + "/api/auth/signup";

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
                Debug.LogError("Signup error: " + req.error);
            }
            else
            {
                Debug.Log("Signup response: " +
                          req.downloadHandler.text);
                // TODO: show "Account created" message in UI
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
}