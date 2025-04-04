using UnityEngine;
using System.Collections.Generic;

public enum RelationshipCategory
{
    Stranger,
    Acquaintance,
    Friend,
    CloseFriend,
    Rival,
    Enemy
}

public struct RelationshipInfo
{
    public RelationshipCategory category;
    public string affiliationLabel;
}

public class RelationshipSystem : MonoBehaviour
{
    private NPC owner;

    public Dictionary<NPC, float> loveHateScores = new Dictionary<NPC, float>();
    public Dictionary<NPC, float> respectContemptScores = new Dictionary<NPC, float>();
    public Dictionary<NPC, float> comfortFearScores = new Dictionary<NPC, float>();
    public Dictionary<NPC, float> familiarityScores = new Dictionary<NPC, float>();
    public Dictionary<NPC, RelationshipInfo> relationshipInfo = new Dictionary<NPC, RelationshipInfo>();

    void Awake()
    {
        owner = GetComponent<NPC>();
    }

    public float GetFamiliarityScore(NPC other)
    {
        if (familiarityScores.ContainsKey(other))
            return familiarityScores[other];
        return 0f;
    }

    public float GetOverallSentimentScore(NPC other)
    {
        float lh = loveHateScores.ContainsKey(other) ? loveHateScores[other] : 0f;
        float rc = respectContemptScores.ContainsKey(other) ? respectContemptScores[other] : 0f;
        float cf = comfortFearScores.ContainsKey(other) ? comfortFearScores[other] : 0f;
        return (lh + rc + cf) / 3f;
    }

    public void UpdateRelationship(NPC other, float loveHateDelta, float respectContemptDelta, float comfortFearDelta)
    {
        if (other == null || owner == null)
            return;

        if (loveHateScores.ContainsKey(other))
            loveHateScores[other] = Mathf.Clamp(loveHateScores[other] + loveHateDelta, -1f, 1f);
        else
            loveHateScores.Add(other, Mathf.Clamp(loveHateDelta, -1f, 1f));

        if (respectContemptScores.ContainsKey(other))
            respectContemptScores[other] = Mathf.Clamp(respectContemptScores[other] + respectContemptDelta, -1f, 1f);
        else
            respectContemptScores.Add(other, Mathf.Clamp(respectContemptDelta, -1f, 1f));

        if (comfortFearScores.ContainsKey(other))
            comfortFearScores[other] = Mathf.Clamp(comfortFearScores[other] + comfortFearDelta, -1f, 1f);
        else
            comfortFearScores.Add(other, Mathf.Clamp(comfortFearDelta, -1f, 1f));

        float currentFamiliarity = GetFamiliarityScore(other);
        float familiarityDelta = 0.05f;
        float newFamiliarity = Mathf.Clamp(currentFamiliarity + familiarityDelta, 0f, 1f);
        if (familiarityScores.ContainsKey(other))
            familiarityScores[other] = newFamiliarity;
        else
            familiarityScores.Add(other, newFamiliarity);

        float compositeScore = GetOverallSentimentScore(other);
        RelationshipInfo info = UpdateRelationshipCategory(other, compositeScore);

        if (relationshipInfo.ContainsKey(other))
            relationshipInfo[other] = info;
        else
            relationshipInfo.Add(other, info);
    }

    private RelationshipInfo UpdateRelationshipCategory(NPC other, float compositeScore)
    {
        RelationshipInfo info = new RelationshipInfo();

        if (compositeScore >= 0.6f)
            info.category = RelationshipCategory.CloseFriend;
        else if (compositeScore >= 0.3f)
            info.category = RelationshipCategory.Friend;
        else if (compositeScore > -0.3f)
            info.category = RelationshipCategory.Acquaintance;
        else if (compositeScore > -0.6f)
            info.category = RelationshipCategory.Rival;
        else
            info.category = RelationshipCategory.Enemy;

        info.affiliationLabel = "";
        FactionMembership myFactions = owner.GetComponent<FactionMembership>();
        FactionMembership otherFactions = other.GetComponent<FactionMembership>();
        if (myFactions != null && otherFactions != null)
        {
            foreach (Faction faction in myFactions.factions)
            {
                if (otherFactions.factions.Contains(faction))
                {
                    switch (faction.factionType)
                    {
                        case FactionType.Family:
                            info.affiliationLabel = "Family";
                            break;
                        case FactionType.FriendGroup:
                            info.affiliationLabel = "Friend Group";
                            break;
                        case FactionType.Business:
                            info.affiliationLabel = "Colleague";
                            break;
                        default:
                            info.affiliationLabel = "Shared Faction";
                            break;
                    }
                    break;
                }
            }
        }
        return info;
    }

    // Returns a social modifier for an action based on the number of friendly relationships.
    public float GetSocialModifierForAction(string actionName)
    {
        int friendCount = 0;
        foreach (NPC other in relationshipInfo.Keys)
        {
            RelationshipInfo info = relationshipInfo[other];
            if (info.category == RelationshipCategory.Friend || info.category == RelationshipCategory.CloseFriend)
                friendCount++;
        }
        float socialModifier = Mathf.Clamp01(friendCount / 5f);
        if (actionName == "InteractWithNPC")
        {
            return socialModifier;
        }
        return 0f;
    }
}
