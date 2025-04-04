using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject npcPrefab;
    public Transform spawnParent;
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;

    [Header("Family Generation Defaults")]
    public int childrenPerCouple = 3;
    public int descendantGenerations = 3;

    public Faction defaultCommunityFaction;

    /// <summary>
    /// Spawns a generic NPC at a random position.
    /// </summary>
    public NPC SpawnNPC()
    {
        Vector3 spawnPos = spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );
        GameObject npcObj = Instantiate(npcPrefab, spawnPos, Quaternion.identity, spawnParent);
        NPC npc = npcObj.GetComponent<NPC>();
        if (npc != null)
        {
            // Ensure NPC is registered in the NPCManager
            NPCManager.Instance.RegisterNPC(npc);

            NPCIdentity identity = npc.GetComponent<NPCIdentity>();
            if (identity != null)
            {
                identity.GenerateIdentity();
                npcObj.name = identity.npcName;
            }
        }
        return npc;
    }

    /// <summary>
    /// Spawns a family tree starting with a root couple and their descendants.
    /// Every spawned NPC is registered with the NPCManager.
    /// </summary>
    public List<NPC> SpawnFamilyTree(int familySize, int generations)
    {
        List<NPC> allNPCs = new List<NPC>();

        // Spawn root couple.
        NPC root1 = SpawnNPC();
        NPC root2 = SpawnNPC();
        allNPCs.Add(root1);
        allNPCs.Add(root2);
        Debug.Log("[NPCSpawner] Spawned root couple: " + root1.identity.npcName + " & " + root2.identity.npcName);

        // Use the root couple as the starting generation.
        List<NPC> currentGeneration = new List<NPC> { root1, root2 };

        // Spawn descendant generations.
        for (int gen = 1; gen <= generations; gen++)
        {
            List<NPC> generationChildren = new List<NPC>();
            // Process current generation in couples.
            for (int i = 0; i < currentGeneration.Count; i += 2)
            {
                if (i + 1 >= currentGeneration.Count)
                    break;
                NPC parentA = currentGeneration[i];
                NPC parentB = currentGeneration[i + 1];
                // Spawn children for this couple.
                for (int c = 0; c < familySize; c++)
                {
                    NPC child = SpawnChildNPC(parentA, parentB);
                    if (child != null)
                        generationChildren.Add(child);
                }
            }
            currentGeneration = generationChildren;
            allNPCs.AddRange(generationChildren);
            Debug.Log("[NPCSpawner] Spawned generation " + gen + " with " + generationChildren.Count + " NPCs.");
        }
        return allNPCs;
    }

    /// <summary>
    /// Spawns a child NPC based on two parent NPCs.
    /// </summary>
    public NPC SpawnChildNPC(NPC parentA, NPC parentB)
    {
        // Calculate a spawn position roughly between parents with a small random offset.
        Vector3 midPoint = (parentA.transform.position + parentB.transform.position) / 2f;
        Vector3 offset = new Vector3(Random.Range(-spawnAreaSize.x / 4f, spawnAreaSize.x / 4f),
                                     0,
                                     Random.Range(-spawnAreaSize.z / 4f, spawnAreaSize.z / 4f));
        Vector3 spawnPos = midPoint + offset;

        GameObject npcObj = Instantiate(npcPrefab, spawnPos, Quaternion.identity, spawnParent);
        NPC childNPC = npcObj.GetComponent<NPC>();
        if (childNPC == null)
            return null;

        // Set child's identity.
        NPCIdentity identity = childNPC.GetComponent<NPCIdentity>();
        if (identity != null)
        {
            identity.firstName = "Child"; // Simple naming; can be enhanced.
            // For last name, if a parent has a hyphenated name, choose one side randomly.
            string lastName = "";
            if (parentA.identity != null && parentB.identity != null)
            {
                lastName = (Random.value < 0.5f)
                           ? GetFirstPart(parentA.identity.lastName)
                           : GetFirstPart(parentB.identity.lastName);
            }
            else
            {
                lastName = "Default";
            }
            identity.lastName = lastName;
            identity.npcName = identity.firstName + " " + identity.lastName;
            npcObj.name = identity.npcName;
        }

        // Set up family relationships.
        if (childNPC.familyManager != null)
        {
            childNPC.familyManager.parents = new List<NPCIdentity>();
            if (parentA.identity != null)
            {
                childNPC.familyManager.parents.Add(parentA.identity);
                parentA.familyManager.AddChild(childNPC.identity);
            }
            if (parentB.identity != null)
            {
                childNPC.familyManager.parents.Add(parentB.identity);
                parentB.familyManager.AddChild(childNPC.identity);
            }
        }

        // Apply genetic inheritance if present.
        if (childNPC.npcGenetics != null && parentA.npcGenetics != null && parentB.npcGenetics != null)
        {
            NPCGenetics.ApplyGeneticInheritance(childNPC, parentA, parentB);
        }

        // Initialize subsystems.
        childNPC.InitializeIdentity();

        // Register the child NPC.
        NPCManager.Instance.RegisterNPC(childNPC);

        return childNPC;
    }

    /// <summary>
    /// Helper method to get the first part of a hyphenated last name.
    /// If the name is not hyphenated, returns the full name.
    /// </summary>
    private string GetFirstPart(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            return "";
        string[] parts = lastName.Split('-');
        return parts[0];
    }
}
