using System.Collections.Generic;
using UnityEngine;

public class OccupationAssignmentManager : MonoBehaviour
{
    public static OccupationAssignmentManager Instance { get; private set; }

    // Initial tolerance for near-threshold candidates (10%)
    private float initialTolerance = 0.1f;
    // Maximum tolerance allowed (50%)
    private float maxTolerance = 0.5f;
    // Tolerance increment (5%)
    private float toleranceIncrement = 0.05f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally persist across scenes: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Iterates over all job positions in the provided BusinessInfo and assigns available NPCs based on their skills.
    /// </summary>
    public void AssignOccupations(BusinessInfo business)
    {
        List<NPC> allNPCs = NPCManager.Instance.GetAllNPCs();
        Debug.Log("[OccupationAssignmentManager] Total NPCs found: " + allNPCs.Count);


        foreach (var pos in business.positions)
        {
            NPC bestCandidate = null;
            float bestSkillValue = -1f;
            Debug.Log($"[OccupationAssignmentManager] Evaluating position: {pos.positionName} (Requires {pos.requiredSkillName} >= {pos.requiredSkillLevel})");

            // Log all NPCs that have the target skill.
            foreach (NPC npc in allNPCs)
            {
                if (npc.npcSkills != null && npc.npcSkills.skills != null)
                {
                    foreach (Skill skill in npc.npcSkills.skills)
                    {
                        if (skill.skillName.Equals(pos.requiredSkillName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Log($"[OccupationAssignmentManager] {npc.identity.npcName} has {skill.skillName} level {skill.level}");
                        }
                    }
                }
            }

            // First pass: strict check for candidates meeting or exceeding the required level.
            foreach (NPC npc in allNPCs)
            {
                if (AlreadyAssignedToBusiness(npc))
                {
                    Debug.Log($"[OccupationAssignmentManager] Skipping {npc.identity.npcName} (already in a business faction)");
                    continue;
                }

                NPCSkills skills = npc.npcSkills;
                if (skills != null && skills.skills != null)
                {
                    foreach (Skill skill in skills.skills)
                    {
                        if (skill.skillName.Equals(pos.requiredSkillName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (skill.level >= pos.requiredSkillLevel && skill.level > bestSkillValue)
                            {
                                bestSkillValue = skill.level;
                                bestCandidate = npc;
                            }
                        }
                    }
                }
            }

            // Second pass: progressive tolerance if no candidate met the strict threshold.
            if (bestCandidate == null)
            {
                float progressiveTolerance = initialTolerance;
                Debug.Log($"[OccupationAssignmentManager] No strict candidate found for {pos.positionName}. Beginning progressive tolerance search.");
                while (progressiveTolerance <= maxTolerance && bestCandidate == null)
                {
                    Debug.Log($"[OccupationAssignmentManager] Trying with tolerance: {progressiveTolerance:F2}");
                    foreach (NPC npc in allNPCs)
                    {
                        if (AlreadyAssignedToBusiness(npc))
                            continue;

                        NPCSkills skills = npc.npcSkills;
                        if (skills != null && skills.skills != null)
                        {
                            foreach (Skill skill in skills.skills)
                            {
                                if (skill.skillName.Equals(pos.requiredSkillName, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    float threshold = pos.requiredSkillLevel * (1 - progressiveTolerance);
                                    if (skill.level >= threshold && skill.level > bestSkillValue)
                                    {
                                        Debug.Log($"[OccupationAssignmentManager] {npc.identity.npcName} qualifies under tolerance {progressiveTolerance:F2}: Skill {skill.level} (threshold {threshold})");
                                        bestSkillValue = skill.level;
                                        bestCandidate = npc;
                                    }
                                }
                            }
                        }
                    }
                    if (bestCandidate == null)
                        progressiveTolerance += toleranceIncrement;
                }
                if (bestCandidate != null)
                {
                    Debug.Log($"[OccupationAssignmentManager] Fallback candidate found at tolerance {progressiveTolerance:F2}: {bestCandidate.identity.npcName} with value {bestSkillValue}");
                }
                else
                {
                    Debug.LogWarning($"[OccupationAssignmentManager] No candidate available for position: {pos.positionName}");
                }
            }

            if (bestCandidate != null)
            {
                pos.assignedNPC = bestCandidate;
                FactionManager.Instance.JoinFaction(bestCandidate, business.businessFaction);
                Debug.Log($"[OccupationAssignmentManager] Assigned {bestCandidate.identity.npcName} to {pos.positionName} (Required {pos.requiredSkillName} {pos.requiredSkillLevel}, candidate value {bestSkillValue})");
            }
        }
    }

    /// <summary>
    /// Checks whether the given NPC is already assigned to a business faction.
    /// Allows NPCs in community factions.
    /// </summary>
    private bool AlreadyAssignedToBusiness(NPC npc)
    {
        if (npc.factionMembership != null)
        {
            foreach (Faction faction in npc.factionMembership.factions)
            {
                if (faction.factionType == FactionType.Business)
                {
                    Debug.Log($"[OccupationAssignmentManager] {npc.identity.npcName} is already in business faction: {faction.factionName}");
                    return true;
                }
            }
        }
        return false;
    }
}
