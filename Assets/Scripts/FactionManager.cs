using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager Instance { get; private set; }
    public List<Faction> allFactions = new List<Faction>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method periodically or on relationship updates.
    public void UpdateFactionAlignments(float conflictDelta = 0.2f)
    {
        // Clear previous dynamic relationships.
        foreach (Faction faction in allFactions)
        {
            faction.allies.Clear();
            faction.rivals.Clear();
        }

        // Iterate over all pairs of factions.
        for (int i = 0; i < allFactions.Count; i++)
        {
            for (int j = i + 1; j < allFactions.Count; j++)
            {
                Faction factionA = allFactions[i];
                Faction factionB = allFactions[j];
                float similarity = factionA.ideology.GetSimilarity(factionB.ideology);
                if (similarity >= factionA.compatibilityThreshold)
                {
                    factionA.allies.Add(factionB);
                    factionB.allies.Add(factionA);
                }
                else if (similarity < (factionA.compatibilityThreshold - conflictDelta))
                {
                    factionA.rivals.Add(factionB);
                    factionB.rivals.Add(factionA);
                }
            }
        }
    }

    public void JoinFaction(NPC npc, Faction faction)
    {
        if (npc == null || faction == null)
            return;

        faction.AddMember(npc);

        FactionMembership membership = npc.GetComponent<FactionMembership>();
        if (membership == null)
        {
            membership = npc.gameObject.AddComponent<FactionMembership>();
        }
        if (!membership.factions.Contains(faction))
        {
            membership.factions.Add(faction);
            Debug.Log("[FactionManager] " + npc.identity.npcName + " joined faction: " + faction.factionName);
        }

        // For community factions, blend NPC ideology with faction ideology.
        if (faction.factionType == FactionType.Community && npc.personality != null)
        {
            Ideology current = npc.personality.ideology;
            Ideology factionIdeology = faction.ideology;
            npc.personality.ideology.freedom = (current.freedom + factionIdeology.freedom) / 2f;
            npc.personality.ideology.privacy = (current.privacy + factionIdeology.privacy) / 2f;
            npc.personality.ideology.authority = (current.authority + factionIdeology.authority) / 2f;
            npc.personality.ideology.equality = (current.equality + factionIdeology.equality) / 2f;
            Debug.Log("[FactionManager] Updated ideology for NPC " + npc.identity.npcName + " after joining faction " + faction.factionName);
        }
    }

    public void RemoveFaction(NPC npc, Faction faction)
    {
        if (npc == null || faction == null)
            return;

        faction.RemoveMember(npc);

        FactionMembership membership = npc.GetComponent<FactionMembership>();
        if (membership != null && membership.factions.Contains(faction))
        {
            membership.factions.Remove(faction);
            Debug.Log("[FactionManager] " + npc.identity.npcName + " left faction: " + faction.factionName);
        }
    }
}
