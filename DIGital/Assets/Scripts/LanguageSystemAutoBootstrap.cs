using UnityEngine;

public static class LanguageSystemAutoBootstrap
{
    private const string PrefabPath = "Prefabs/LanguageSystem";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureLanguageSystem()
    {
        // if language system already present (e.g., scene has it or it persisted), do nothing
        if (LanguageManager.Instance != null) return;

        var prefab = Resources.Load<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[LanguageSystemAutoBootstrap] Could not load prefab at Resources/{PrefabPath}.prefab");
            return;
        }

        Object.Instantiate(prefab);
        Debug.Log("[LanguageSystemAutoBootstrap] Spawned LanguageSystem.");
    }
}
