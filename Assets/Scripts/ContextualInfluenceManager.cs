using UnityEngine;

public class ContextualInfluenceManager : MonoBehaviour
{
    public static ContextualInfluenceManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Persist across scenes if needed.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Sums "DecisionInfluence" modifiers from the NPC's traits.
    public float GetTraitInfluence(NPC npc)
    {
        float totalInfluence = 0f;
        if (npc.traits != null)
        {
            foreach (var trait in npc.traits)
            {
                if (trait.modifiers.ContainsKey("DecisionInfluence"))
                {
                    totalInfluence += trait.modifiers["DecisionInfluence"];
                }
            }
        }
        return totalInfluence;
    }

    // Uses the NPC's defaultConfidence to represent personality influence.
    // A defaultConfidence of 0.5 is neutral.
    public float GetPersonalityInfluence(NPC npc)
    {
        return npc.personality.defaultConfidence - 0.5f;
    }

    // Determines skill influence based on action type.
    public float GetSkillInfluence(NPC npc, string actionName)
    {
        float influence = 0f;
        if (npc.npcSkills != null)
        {
            if (actionName == "Patrol")
            {
                Skill athletics = npc.npcSkills.GetSkill("Athletics");
                if (athletics != null)
                    influence = (athletics.level - 50f) / 100f;
            }
            else if (actionName == "Flee")
            {
                Skill strength = npc.npcSkills.GetSkill("Strength");
                if (strength != null)
                    influence = (50f - strength.level) / 100f;
            }
            else if (actionName == "InteractWithNPC")
            {
                Skill charisma = npc.npcSkills.GetSkill("Charisma");
                if (charisma != null)
                    influence = (charisma.level - 50f) / 100f;
            }
            // Additional action types can be mapped as needed.
        }
        return influence;
    }

    // Provides a relationship influence bonus based on friend-type relationships.
    // If a target NPC is specified, it looks up that specific relationship.
    public float GetRelationshipInfluence(NPC npc, NPC target = null)
    {
        float bonus = 0f;
        if (npc.relationshipSystem != null)
        {
            if (target != null)
            {
                if (npc.relationshipSystem.relationshipInfo.ContainsKey(target))
                {
                    RelationshipInfo info = npc.relationshipSystem.relationshipInfo[target];
                    if (info.category == RelationshipCategory.Friend || info.category == RelationshipCategory.CloseFriend)
                    {
                        bonus = 0.2f;
                    }
                }
            }
            else
            {
                // Sum small bonuses for each friendly relationship.
                foreach (var kvp in npc.relationshipSystem.relationshipInfo)
                {
                    if (kvp.Value.category == RelationshipCategory.Friend || kvp.Value.category == RelationshipCategory.CloseFriend)
                        bonus += 0.05f;
                }
            }
        }
        return bonus;
    }

    // Aggregate method that returns a total contextual influence value.
    public float GetTotalContextualInfluence(NPC npc, string actionName, NPC target = null)
    {
        float traitInf = GetTraitInfluence(npc);
        float personalityInf = GetPersonalityInfluence(npc);
        float skillInf = GetSkillInfluence(npc, actionName);
        float relationshipInf = GetRelationshipInfluence(npc, target);
        return traitInf + personalityInf + skillInf + relationshipInf;
    }
}
