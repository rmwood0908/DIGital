using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSite : MonoBehaviour
{
    public void LoadSite(string siteName)
    {
        Debug.Log("Loading site: " + siteName);
        // SceneManager.LoadScene(siteName);
    }
}
