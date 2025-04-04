using System.Collections.Generic;
using UnityEngine;

public class FactionGenerator : MonoBehaviour
{
    [Header("Industry Settings")]
    [Tooltip("List of Industry objects to choose from when generating a business.")]
    public List<Industry> industries = new List<Industry>();

    [Header("Business Positions (Optional)")]
    [Tooltip("If no positions are defined in the selected BusinessTemplate, these default positions will be used.")]
    public List<CompanyPosition> businessPositions;

    [Header("Financial Settings")]
    [Tooltip("Initial capital for new businesses.")]
    public float initialCapital = 50000f;
    [Tooltip("Minimum profitability margin (e.g., 0.05 for 5%)")]
    public float minProfitMargin = 0.05f;
    [Tooltip("Maximum profitability margin (e.g., 0.20 for 20%)")]
    public float maxProfitMargin = 0.20f;

    // References to other managers.
    public FactionManager factionManager;
    public EconomicManager economicManager;
    public NPCSpawner npcSpawner;
    public NPCManager npcManager;

    // Existing methods...
    public Faction GenerateCommunityFaction()
    {
        Faction communityFaction = ScriptableObject.CreateInstance<Faction>();
        communityFaction.factionType = FactionType.Community;
        communityFaction.compatibilityThreshold = 0.8f;
        communityFaction.factionName = GenerateCommunityName();
        communityFaction.description = "A community faction named " + communityFaction.factionName + " representing a close-knit, locally rooted community.";
        communityFaction.name = communityFaction.factionName;
        factionManager.allFactions.Add(communityFaction);
        Debug.Log("[FactionGenerator] Created Community Faction: " + communityFaction.factionName);
        return communityFaction;
    }

    public Faction GenerateBusinessFaction()
    {
        Faction businessFaction = ScriptableObject.CreateInstance<Faction>();
        businessFaction.factionType = FactionType.Business;
        businessFaction.compatibilityThreshold = 0.3f;
        businessFaction.factionName = GenerateBusinessName();
        businessFaction.description = "A business faction operating in the economic and occupational sectors.";
        businessFaction.name = businessFaction.factionName;
        factionManager.allFactions.Add(businessFaction);
        Debug.Log("[FactionGenerator] Created Business Faction: " + businessFaction.factionName);
        return businessFaction;
    }

    public Faction GenerateBusinessFactionWithParameters(
        string customFactionName,
        string customBusinessName,
        float compatThreshold,
        float freedom,
        float privacy,
        float authority,
        float equality,
        Industry industry,
        BusinessTemplate businessTemplate)
    {
        Faction businessFaction = ScriptableObject.CreateInstance<Faction>();
        businessFaction.factionType = FactionType.Business;
        businessFaction.compatibilityThreshold = compatThreshold;
        businessFaction.factionName = string.IsNullOrEmpty(customFactionName) ? GenerateBusinessName() : customFactionName;
        businessFaction.description = string.IsNullOrEmpty(customBusinessName) ?
            "A business faction operating in the " + industry.industryName + " industry." :
            "A business faction named " + customBusinessName + " operating in the " + industry.industryName + " industry.";
        businessFaction.name = businessFaction.factionName;

        businessFaction.ideology.freedom = freedom;
        businessFaction.ideology.privacy = privacy;
        businessFaction.ideology.authority = authority;
        businessFaction.ideology.equality = equality;

        if (factionManager != null)
        {
            factionManager.allFactions.Add(businessFaction);
        }
        else
        {
            Debug.LogWarning("FactionManager not found!");
        }
        Debug.Log("[FactionGenerator] Created Business Faction with Parameters: " + businessFaction.factionName);

        BusinessInfo newBusiness = ScriptableObject.CreateInstance<BusinessInfo>();
        float profitMargin = Random.Range(minProfitMargin, maxProfitMargin);
        newBusiness.Initialize(businessFaction, businessPositions, industry, businessTemplate, initialCapital, profitMargin);
        if (economicManager != null)
        {
            economicManager.RegisterBusiness(newBusiness);
        }
        else
        {
            Debug.LogWarning("EconomicManager not found!");
        }
        return businessFaction;
    }

    public void GenerateCommunity(int numberOfIndividuals, int numberOfFamilies, int familySize, int generations)
    {
        // Clear existing dynamic factions.
        if (factionManager != null)
        {
            factionManager.allFactions.Clear();
            Debug.Log("[FactionGenerator] Cleared all dynamic factions from FactionManager.");
        }

        Faction communityFaction = GenerateCommunityFaction();

        // Spawn individuals.
        for (int i = 0; i < numberOfIndividuals; i++)
        {
            NPC npc = npcSpawner.SpawnNPC();
            factionManager.JoinFaction(npc, communityFaction);
            Debug.Log("[FactionGenerator] Added individual " + npc.identity.npcName + " to community " + communityFaction.factionName);
        }

        // Spawn family trees.
        for (int i = 0; i < numberOfFamilies; i++)
        {
            List<NPC> familyTree = npcSpawner.SpawnFamilyTree(familySize, generations);
            foreach (NPC member in familyTree)
            {
                factionManager.JoinFaction(member, communityFaction);
                Debug.Log("[FactionGenerator] Added family member " + member.identity.npcName + " to community " + communityFaction.factionName);
            }
        }
    }

    public string GenerateBusinessName()
    {
        return "Business_" + Random.Range(1000, 9999);
    }

    public string GenerateCommunityName()
    {
        return "Community_" + Random.Range(1000, 9999);
    }

    [System.Serializable]
    public class CompanyPosition
    {
        public string positionName;
        public string requiredSkillName;
        public float requiredSkillLevel;
        public NPC assignedNPC;

        [Tooltip("Work roster details for this position")]
        public WorkRoster workRoster; // Now uses WorkRoster from the separate file.
    }
}
