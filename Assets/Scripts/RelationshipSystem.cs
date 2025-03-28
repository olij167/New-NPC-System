using System.Collections.Generic;
using UnityEngine;

public enum FamilyRelationshipType
{
    None,
    Parent,
    Child,
    Sibling,
    Spouse,
    Cousin,
    UncleAunt,
    NieceNephew
}

public enum GeneralRelationshipType
{
    None,
    Friend,
    Enemy,
    Acquaintance,
    Mentor,
    Student
}

[System.Serializable]
public class Relationship
{
    public NPC targetNPC;
    // Relationship scales range from -1 (negative) to 1 (positive)
    public float loveHate;
    public float respectContempt;
    public float comfortFear;

    // General relationship type (Friend, Enemy, etc.)
    public GeneralRelationshipType generalRelationshipType;

    // Family-specific fields
    public bool isFamily;
    public FamilyRelationshipType familyType;

    public Relationship(NPC target, GeneralRelationshipType relType)
    {
        targetNPC = target;
        generalRelationshipType = relType;
        loveHate = 0f;
        respectContempt = 0f;
        comfortFear = 0f;
        isFamily = false;
        familyType = FamilyRelationshipType.None;
    }
}

public class RelationshipSystem : MonoBehaviour
{
    [Header("Compatibility Weights")]
    [Tooltip("Weight for abstract opinion compatibility (default: 0.7).")]
    public float opinionWeight = 0.7f;
    [Tooltip("Weight for sexual compatibility (default: 0.3).")]
    public float sexualWeight = 0.3f;

    public List<Relationship> relationships = new List<Relationship>();

    // Reference to the NPC owning this RelationshipSystem.
    private NPC owner;

    void Awake()
    {
        owner = GetComponent<NPC>();
    }

    /// <summary>
    /// Adds a new relationship for the given NPC if one doesn't already exist.
    /// </summary>
    public void AddRelationship(NPC other, GeneralRelationshipType relType, bool isFamily = false, FamilyRelationshipType familyType = FamilyRelationshipType.None)
    {
        if (relationships.Find(r => r.targetNPC == other) == null)
        {
            Relationship rel = new Relationship(other, relType);
            rel.isFamily = isFamily;
            rel.familyType = familyType;
            relationships.Add(rel);
        }
    }

    /// <summary>
    /// Updates relationship values for the given NPC.
    /// </summary>
    public void UpdateRelationship(NPC other, float loveHateDelta, float respectContemptDelta, float comfortFearDelta)
    {
        Relationship rel = relationships.Find(r => r.targetNPC == other);
        if (rel != null)
        {
            rel.loveHate = Mathf.Clamp(rel.loveHate + loveHateDelta, -1f, 1f);
            rel.respectContempt = Mathf.Clamp(rel.respectContempt + respectContemptDelta, -1f, 1f);
            rel.comfortFear = Mathf.Clamp(rel.comfortFear + comfortFearDelta, -1f, 1f);
        }
    }

    /// <summary>
    /// Checks abstract opinion compatibility between the owner NPC and another NPC.
    /// Returns a value between 0 (completely incompatible) and 1 (identical opinions).
    /// </summary>
    public float CheckOpinionCompatibility(NPC other)
    {
        Personality pA = owner.personality;
        Personality pB = other.personality;
        float totalScore = 0f;
        int count = 0;
        foreach (ConceptOpinion opinionA in pA.conceptOpinions)
        {
            ConceptOpinion opinionB = pB.GetConceptOpinion(opinionA.conceptName);
            if (opinionB != null)
            {
                // Similarity is higher when the difference in moral judgments is smaller.
                float similarity = 1f - Mathf.Abs(opinionA.moralJudgement - opinionB.moralJudgement);
                totalScore += similarity;
                count++;
            }
        }
        return (count > 0) ? totalScore / count : 0.5f; // Default compatibility if no common opinions exist.
    }

    /// <summary>
    /// Checks sexual compatibility between the owner NPC and another NPC.
    /// Returns 1 for mutual attraction, 0.5 if one-way attraction, and 0 for none.
    /// </summary>
    public float CheckSexualCompatibility(NPC other)
    {
        Sexuality sA = owner.sexuality;
        Sexuality sB = other.sexuality;
        bool aAttracted = sA.IsAttractedTo(sB);
        bool bAttracted = sB.IsAttractedTo(sA);

        if (aAttracted && bAttracted)
            return 1f;
        else if (aAttracted || bAttracted)
            return 0.5f;
        else
            return 0f;
    }

    /// <summary>
    /// Combines opinion and sexual compatibility into an overall compatibility score.
    /// </summary>
    public float CheckOverallCompatibility(NPC other)
    {
        float opinionComp = CheckOpinionCompatibility(other);
        float sexualComp = CheckSexualCompatibility(other);
        return (opinionWeight * opinionComp) + (sexualWeight * sexualComp);
    }

    /// <summary>
    /// Evaluates the relationship between the owner NPC and another NPC.
    /// Forms a relationship (Friend, Enemy, or Acquaintance) based on overall compatibility.
    /// Also ensures the relationship is symmetric by updating both NPCs' RelationshipSystems.
    /// </summary>
    public void EvaluateRelationship(NPC other)
    {
        float compatibility = CheckOverallCompatibility(other);

        // Determine relationship type and value adjustments based on compatibility thresholds.
        if (compatibility > 0.7f)
        {
            // High compatibility: form as Friends.
            AddRelationship(other, GeneralRelationshipType.Friend);
            UpdateRelationship(other, 0.5f, 0.3f, 0.2f);
        }
        else if (compatibility < 0.3f)
        {
            // Low compatibility: form as Enemies.
            AddRelationship(other, GeneralRelationshipType.Enemy);
            UpdateRelationship(other, -0.5f, -0.3f, 0.3f);
        }
        else
        {
            // Moderate compatibility: form as Acquaintances.
            AddRelationship(other, GeneralRelationshipType.Acquaintance);
            UpdateRelationship(other, 0.2f, 0.1f, 0.0f);
        }

        // Ensure the other NPC's relationship system also forms the corresponding relationship.
        RelationshipSystem otherRelSys = other.relationshipSystem;
        if (otherRelSys != null)
        {
            // Only update if a relationship doesn't already exist to avoid duplication.
            if (otherRelSys.relationships.Find(r => r.targetNPC == owner) == null)
            {
                if (compatibility > 0.7f)
                {
                    otherRelSys.AddRelationship(owner, GeneralRelationshipType.Friend);
                    otherRelSys.UpdateRelationship(owner, 0.5f, 0.3f, 0.2f);
                }
                else if (compatibility < 0.3f)
                {
                    otherRelSys.AddRelationship(owner, GeneralRelationshipType.Enemy);
                    otherRelSys.UpdateRelationship(owner, -0.5f, -0.3f, 0.3f);
                }
                else
                {
                    otherRelSys.AddRelationship(owner, GeneralRelationshipType.Acquaintance);
                    otherRelSys.UpdateRelationship(owner, 0.2f, 0.1f, 0.0f);
                }
            }
        }
    }

    /// <summary>
    /// Checks if the owner can partner with the other NPC (i.e. they are not related).
    /// </summary>
    public bool CanPartnerWith(NPC other)
    {
        NPCIdentity myId = owner.GetComponent<NPCIdentity>();
        NPCIdentity otherId = other.GetComponent<NPCIdentity>();
        if (myId == null || otherId == null)
            return false;
        return myId.CanDate(otherId);
    }

    /// <summary>
    /// Evaluates whether the owner can form a partnership (spouse relationship) with the other NPC.
    /// If both are eligible (and not already partnered), sets them as spouses.
    /// </summary>
    public void EvaluatePartnership(NPC other)
    {
        if (CanPartnerWith(other))
        {
            NPCIdentity myId = owner.GetComponent<NPCIdentity>();
            NPCIdentity otherId = other.GetComponent<NPCIdentity>();
            if (myId.spouse == null && otherId.spouse == null)
            {
                myId.SetSpouse(otherId);
                Debug.Log(myId.npcName + " and " + otherId.npcName + " are now partners.");
            }
        }
        else
        {
            Debug.Log(owner.GetComponent<NPCIdentity>().npcName + " cannot partner with " + other.GetComponent<NPCIdentity>().npcName + " due to kinship restrictions.");
        }
    }

    public float GetRelationshipValue(NPC other)
    {
        Relationship rel = relationships.Find(r => r.targetNPC == other);
        return (rel != null) ? rel.loveHate : 0f;
    }
}
