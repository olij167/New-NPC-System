using System.Collections.Generic;
using UnityEngine;

public class FriendGroupManager : MonoBehaviour
{
    [Header("Friendship Settings")]
    public float sentimentThreshold = 0.3f;
    public float familiarityThreshold = 0.5f;
    public int minimumGroupSize = 3;

    public List<Faction> activeFriendGroups = new List<Faction>();

    // Check friend group membership every 3 seconds.
    private float updateInterval = 3f;
    private float lastUpdateTime = 0f;

    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            List<NPC> allNPCs = NPCManager.Instance.GetAllNPCs();
            ProcessGroupInteraction(allNPCs);
            lastUpdateTime = Time.time;
        }
    }

    public void ProcessGroupInteraction(List<NPC> interactingNPCs)
    {
        if (interactingNPCs == null || interactingNPCs.Count < minimumGroupSize)
            return;

        List<NPC> candidates = new List<NPC>();
        foreach (NPC npc in interactingNPCs)
        {
            int friendlyCount = 0;
            foreach (NPC other in interactingNPCs)
            {
                if (other == npc)
                    continue;
                float sentiment = npc.relationshipSystem.GetOverallSentimentScore(other);
                float familiarity = npc.relationshipSystem.GetFamiliarityScore(other);
                if (sentiment >= sentimentThreshold && familiarity >= familiarityThreshold)
                    friendlyCount++;
            }
            if (friendlyCount >= 2)
                candidates.Add(npc);
        }

        if (candidates.Count >= minimumGroupSize)
        {
            Faction friendGroup = FindFriendGroupForCandidates(candidates);
            if (friendGroup == null)
            {
                friendGroup = ScriptableObject.CreateInstance<Faction>();
                friendGroup.factionType = FactionType.FriendGroup;
                friendGroup.factionName = "FriendGroup_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                friendGroup.description = "An automatically generated friend group.";
                friendGroup.name = friendGroup.factionName;
                friendGroup.groupSentiments = new Dictionary<string, float>();

                FactionManager.Instance.allFactions.Add(friendGroup);
                activeFriendGroups.Add(friendGroup);

                Debug.Log("[FriendGroupManager] Created new friend group: " + friendGroup.factionName);
            }

            foreach (NPC npc in candidates)
            {
                if (!npc.factionMembership.factions.Contains(friendGroup))
                {
                    FactionManager.Instance.JoinFaction(npc, friendGroup);
                    Debug.Log("[FriendGroupManager] " + npc.identity.npcName + " added to friend group: " + friendGroup.factionName);
                }
            }
        }

        UpdateFriendGroupMemberships();
        ComputeGroupSentiments();
    }

    private void UpdateFriendGroupMemberships()
    {
        List<Faction> groupsToDisband = new List<Faction>();
        foreach (Faction fg in activeFriendGroups)
        {
            List<NPC> members = fg.members;
            List<NPC> membersToRemove = new List<NPC>();
            foreach (NPC member in members)
            {
                int internalCount = 0;
                foreach (NPC other in members)
                {
                    if (other == member)
                        continue;
                    float sentiment = member.relationshipSystem.GetOverallSentimentScore(other);
                    float familiarity = member.relationshipSystem.GetFamiliarityScore(other);
                    if (sentiment >= sentimentThreshold && familiarity >= familiarityThreshold)
                        internalCount++;
                }
                if (internalCount < 2)
                    membersToRemove.Add(member);
            }
            foreach (NPC removeMe in membersToRemove)
            {
                FactionManager.Instance.RemoveFaction(removeMe, fg);
                Debug.Log("[FriendGroupManager] " + removeMe.identity.npcName + " removed from friend group: " + fg.factionName);
            }
            if (fg.members.Count < minimumGroupSize)
                groupsToDisband.Add(fg);
        }

        foreach (Faction fg in groupsToDisband)
        {
            foreach (NPC npc in new List<NPC>(fg.members))
            {
                FactionManager.Instance.RemoveFaction(npc, fg);
            }
            FactionManager.Instance.allFactions.Remove(fg);
            activeFriendGroups.Remove(fg);
            Debug.Log("[FriendGroupManager] Disbanded friend group: " + fg.factionName);
        }
    }

    private void ComputeGroupSentiments()
    {
        List<NPC> allNPCs = NPCManager.Instance.GetAllNPCs();
        foreach (Faction fg in activeFriendGroups)
        {
            fg.groupSentiments.Clear();
            foreach (NPC external in allNPCs)
            {
                if (fg.members.Contains(external))
                    continue;
                float totalWeight = 0f;
                float weightedSum = 0f;
                foreach (NPC member in fg.members)
                {
                    float sentiment = member.relationshipSystem.GetOverallSentimentScore(external);
                    totalWeight += sentiment;
                    weightedSum += sentiment * sentiment;
                }
                if (totalWeight != 0f)
                {
                    float groupSentiment = weightedSum / totalWeight;
                    fg.groupSentiments[external.identity.npcName] = groupSentiment;
                    Debug.Log("[FriendGroupManager] Group " + fg.factionName + " sentiment toward " + external.identity.npcName +
                              ": " + groupSentiment.ToString("F2"));
                }
            }
        }
    }

    private Faction FindFriendGroupForCandidates(List<NPC> candidates)
    {
        foreach (NPC candidate in candidates)
        {
            if (candidate.factionMembership != null)
            {
                foreach (Faction f in candidate.factionMembership.factions)
                {
                    if (f.factionType == FactionType.FriendGroup)
                        return f;
                }
            }
        }
        return null;
    }
}
