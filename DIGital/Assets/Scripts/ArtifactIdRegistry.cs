using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DIGital/Artifact ID Registry")]

public class ArtifactIdRegistry : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string artifactId;   // e.g. "1"
        public string displayName;  // optional: "Pottery Shard"
    }

    public List<Entry> entries = new List<Entry>();

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
}