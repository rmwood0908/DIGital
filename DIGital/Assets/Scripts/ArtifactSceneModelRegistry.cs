using System;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSceneModelRegistry : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string artifactId;      // e.g. "1"
        public GameObject modelObject; // scene object (disabled at start)
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();

    public void HideAll()
    {
        foreach (var entry in entries)
        {
            if (entry?.modelObject != null)
                entry.modelObject.SetActive(false);
        }
    }

    public bool ContainsId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        id = id.Trim();

        foreach (var entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.artifactId)) continue;
            if (string.Equals(entry.artifactId.Trim(), id, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public GameObject GetModel(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        id = id.Trim();

        foreach (var entry in entries)
        {
            if (entry == null || entry.modelObject == null || string.IsNullOrWhiteSpace(entry.artifactId)) continue;
            if (string.Equals(entry.artifactId.Trim(), id, StringComparison.OrdinalIgnoreCase))
                return entry.modelObject;
        }

        return null;
    }
}