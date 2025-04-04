using UnityEngine;
using System.Collections.Generic;

public enum FactionType
{
    Community,
    Business,
    Family,
    FriendGroup
}

[CreateAssetMenu(fileName = "NewFaction", menuName = "NPC/Faction", order = 1)]
public class Faction : ScriptableObject
{
    [Header("Basic Information")]
    public string factionName;
    [TextArea]
    public string description;
    public FactionType factionType;
    public float compatibilityThreshold = 0.7f;

    [Header("Ideological Profile")]
    public Ideology ideology = new Ideology { freedom = 0.5f, privacy = 0.5f, authority = 0.5f, equality = 0.5f };

    [Header("Hierarchy")]
    public Faction parentFaction;
    public List<Faction> subFactions = new List<Faction>();

    [Header("Membership")]
    [HideInInspector]
    public List<NPC> members = new List<NPC>();

    [Header("Friend Group Specific")]
    [HideInInspector]
    public Dictionary<string, float> groupSentiments = new Dictionary<string, float>();

    // New fields for dynamic faction relationships.
    [Header("Dynamic Relationships")]
    public List<Faction> allies = new List<Faction>();
    public List<Faction> rivals = new List<Faction>();

    public float GetIdeologicalCompatibility(Faction other)
    {
        if (other == null)
            return 0f;
        return ideology.GetSimilarity(other.ideology);
    }

    public string GetRelationshipStatus(Faction other, float conflictDelta = 0.2f)
    {
        float similarity = GetIdeologicalCompatibility(other);
        if (similarity >= compatibilityThreshold)
            return "Ally";
        else if (similarity < (compatibilityThreshold - conflictDelta))
            return "Rival";
        else
            return "Neutral";
    }

    public void AddSubFaction(Faction subFaction)
    {
        if (subFaction == null)
            return;
        if (!subFactions.Contains(subFaction))
        {
            subFaction.parentFaction = this;
            subFactions.Add(subFaction);
        }
    }

    public List<Faction> GetHierarchy()
    {
        List<Faction> hierarchy = new List<Faction>();
        Faction current = this;
        while (current != null)
        {
            hierarchy.Insert(0, current);
            current = current.parentFaction;
        }
        return hierarchy;
    }

    public void AddMember(NPC npc)
    {
        if (npc != null && !members.Contains(npc))
            members.Add(npc);
    }

    public void RemoveMember(NPC npc)
    {
        if (npc != null && members.Contains(npc))
            members.Remove(npc);
    }
}
