using UnityEngine;
using System.Collections.Generic;

public class CommunitySpawner : MonoBehaviour
{
    [Header("General Spawn Settings")]
    [Tooltip("If true, spawn individual NPCs.")]
    public bool spawnNPCs = true;
    [Tooltip("Total number of individual NPCs to spawn.")]
    public int totalNPCCount = 50;

    [Header("Family Spawn Options")]
    [Tooltip("If true, spawn family trees along with individual NPCs.")]
    public bool spawnFamilies = false;
    [Tooltip("Number of families to spawn.")]
    public int numberOfFamilies = 5;
    [Tooltip("Number of NPCs per family (in one generation).")]
    public int familySize = 3;
    [Tooltip("Number of generations in each family tree.")]
    public int familyGenerations = 2;

    [Header("Community (Faction) Settings")]
    [Tooltip("If true, assign spawned NPCs to a community faction.")]
    public bool assignToFaction = true;
    [Tooltip("If set, use this root community for all NPCs. Otherwise, one will be created automatically.")]
    public Faction rootCommunity; // Reference to the community faction

    [Header("Business / Civilization Settings")]
    [Tooltip("If true, spawn businesses in the community.")]
    public bool spawnBusinesses = false;
    [Tooltip("Number of businesses to spawn.")]
    public int numberOfBusinesses = 3;
    [Tooltip("Optional list of industries to choose from. If empty, a random industry from the FactionGenerator will be used.")]
    public List<Industry> specifiedIndustries;
    [Tooltip("Optional list of business templates to choose from. If empty, the chosen industry's defaults will be used.")]
    public List<BusinessTemplate> specifiedBusinessTemplates;

    [Header("References")]
    public NPCSpawner npcSpawner;
    public FactionGenerator factionGenerator;
    public BusinessInfoGenerator businessGenerator;
    public NPCManager npcManager;
    public FactionManager factionManager;

    /// <summary>
    /// Spawns individual NPCs and assigns them to the root community if enabled.
    /// </summary>
    public void SpawnNPCs()
    {
        if (npcSpawner == null || npcManager == null)
        {
            Debug.LogError("Missing references for NPC spawning.");
            return;
        }

        // Ensure a root community exists if assignment is enabled.
        if (assignToFaction)
        {
            if (rootCommunity == null)
            {
                rootCommunity = factionGenerator.GenerateCommunityFaction();
            }
        }

        for (int i = 0; i < totalNPCCount; i++)
        {
            NPC npc = npcSpawner.SpawnNPC();
            npcManager.RegisterNPC(npc);
            if (assignToFaction && factionManager != null && rootCommunity != null)
            {
                factionManager.JoinFaction(npc, rootCommunity);
            }
        }
        Debug.Log("Spawned " + totalNPCCount + " individual NPCs.");
    }

    /// <summary>
    /// Spawns family trees and assigns them to the root community if enabled.
    /// </summary>
    public void SpawnFamilies()
    {
        if (npcSpawner == null || npcManager == null)
        {
            Debug.LogError("Missing references for Family spawning.");
            return;
        }

        // Ensure a root community exists if assignment is enabled.
        if (assignToFaction)
        {
            if (rootCommunity == null)
            {
                rootCommunity = factionGenerator.GenerateCommunityFaction();
            }
        }

        for (int i = 0; i < numberOfFamilies; i++)
        {
            List<NPC> family = npcSpawner.SpawnFamilyTree(familySize, familyGenerations);
            foreach (NPC npc in family)
            {
                npcManager.RegisterNPC(npc);
                if (assignToFaction && factionManager != null && rootCommunity != null)
                {
                    factionManager.JoinFaction(npc, rootCommunity);
                }
            }
            Debug.Log("Spawned family tree " + (i + 1));
        }
    }

    /// <summary>
    /// Spawns a new community faction on demand.
    /// </summary>
    public void SpawnFactions()
    {
        if (factionGenerator == null)
        {
            Debug.LogError("Missing FactionGenerator reference.");
            return;
        }
        // Spawn a new community faction and set it as the root.
        rootCommunity = factionGenerator.GenerateCommunityFaction();
        Debug.Log("Spawned Community Faction: " + rootCommunity.factionName);
    }

    /// <summary>
    /// Spawns businesses using the provided industry and template information.
    /// </summary>
    public void SpawnBusinesses()
    {
        if (factionGenerator == null || businessGenerator == null)
        {
            Debug.LogError("Missing references for Business spawning.");
            return;
        }
        for (int i = 0; i < numberOfBusinesses; i++)
        {
            Industry chosenIndustry = null;
            BusinessTemplate chosenTemplate = null;

            if (specifiedIndustries != null && specifiedIndustries.Count > 0)
            {
                chosenIndustry = specifiedIndustries[Random.Range(0, specifiedIndustries.Count)];
            }
            else if (factionGenerator.industries != null && factionGenerator.industries.Count > 0)
            {
                chosenIndustry = factionGenerator.industries[Random.Range(0, factionGenerator.industries.Count)];
            }

            if (specifiedBusinessTemplates != null && specifiedBusinessTemplates.Count > 0)
            {
                chosenTemplate = specifiedBusinessTemplates[Random.Range(0, specifiedBusinessTemplates.Count)];
            }
            else if (chosenIndustry != null && chosenIndustry.businessTemplates != null && chosenIndustry.businessTemplates.Count > 0)
            {
                chosenTemplate = chosenIndustry.businessTemplates[Random.Range(0, chosenIndustry.businessTemplates.Count)];
            }

            // Generate a business faction.
            Faction businessFaction = factionGenerator.GenerateBusinessFaction();
            // Maintain previous naming logic.
            string templateName = chosenTemplate != null ? chosenTemplate.templateName : "Default";
            businessFaction.factionName += " (" + templateName + ")";
            businessFaction.name = businessFaction.factionName;
            BusinessInfo business = businessGenerator.GenerateBusinessInfo();
            Debug.Log("Spawned Business: " + business.businessName);
        }
    }

    /// <summary>
    /// High-level method that spawns the entire community.
    /// </summary>
    public void SpawnCommunity()
    {
        SpawnNPCs();
        if (spawnFamilies)
            SpawnFamilies();
        if (spawnBusinesses)
            SpawnBusinesses();
    }
}
