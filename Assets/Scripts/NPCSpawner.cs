using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject npcPrefab;            // Prefab including NPC, NPCIdentity, Personality, Sexuality, and RelationshipSystem.
    public Transform spawnParent;           // Optional parent transform.
    public Vector3 spawnAreaCenter;         // Center of the spawn area.
    public Vector3 spawnAreaSize;           // Size of the spawn area.

    [Header("Compatibility Weights")]
    [Range(0f, 1f)]
    public float opinionWeight = 0.7f;      // (Handled by RelationshipSystem.)
    [Range(0f, 1f)]
    public float sexualWeight = 0.3f;       // (Handled by RelationshipSystem.)

    // Spawns a single NPC.
    public NPC SpawnNPC()
    {
        Vector3 spawnPos = spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        GameObject npcObj = Instantiate(npcPrefab, spawnPos, Quaternion.identity, spawnParent);
        NPC npc = npcObj.GetComponent<NPC>();

        // Generate identity and set GameObject's name.
        NPCIdentity identity = npc.GetComponent<NPCIdentity>();
        if (identity != null)
        {
            identity.GenerateIdentity();
            npcObj.name = identity.npcName;
        }

        FormRelationships(npc);
        return npc;
    }

    // Spawns a family with proper relationships.
    // If count == 1: Spawns one NPC.
    // If count == 2: Spawns two NPCs as spouses.
    // If count >= 3: Spawns two parents and (count - 2) children, linking parents, children, and siblings.
    public List<NPC> SpawnFamily(int count)
    {
        List<NPC> familyMembers = new List<NPC>();

        if (count <= 1)
        {
            familyMembers.Add(SpawnNPC());
            return familyMembers;
        }

        // Determine a family center.
        Vector3 familyCenter = spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        // Select a shared last name for this family.
        string familyLastName = "Family";
        if (NPCManager.Instance != null && NPCManager.Instance.lastNames.Count > 0)
        {
            familyLastName = NPCManager.Instance.lastNames[Random.Range(0, NPCManager.Instance.lastNames.Count)];
        }

        if (count == 2)
        {
            // Spawn two NPCs and set as spouses.
            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 2f;
                GameObject npcObj = Instantiate(npcPrefab, familyCenter + offset, Quaternion.identity, spawnParent);
                NPC npc = npcObj.GetComponent<NPC>();

                NPCIdentity identity = npc.GetComponent<NPCIdentity>();
                if (identity != null)
                {
                    // Assign the shared last name before generating identity.
                    identity.lastName = familyLastName;
                    identity.GenerateIdentity();
                    npcObj.name = identity.npcName;
                }
                familyMembers.Add(npc);
            }
            NPCIdentity identityA = familyMembers[0].GetComponent<NPCIdentity>();
            NPCIdentity identityB = familyMembers[1].GetComponent<NPCIdentity>();
            identityA.SetSpouse(identityB);
        }
        else // count >= 3
        {
            // Spawn 2 parents.
            List<NPC> parents = new List<NPC>();
            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 2f;
                GameObject npcObj = Instantiate(npcPrefab, familyCenter + offset, Quaternion.identity, spawnParent);
                NPC npc = npcObj.GetComponent<NPC>();

                NPCIdentity identity = npc.GetComponent<NPCIdentity>();
                if (identity != null)
                {
                    identity.lastName = familyLastName;
                    identity.GenerateIdentity();
                    npcObj.name = identity.npcName;
                }
                parents.Add(npc);
                familyMembers.Add(npc);
            }
            NPCIdentity parentA = parents[0].GetComponent<NPCIdentity>();
            NPCIdentity parentB = parents[1].GetComponent<NPCIdentity>();
            parentA.SetSpouse(parentB);

            // Spawn children.
            List<NPC> children = new List<NPC>();
            for (int i = 0; i < count - 2; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 2f;
                GameObject npcObj = Instantiate(npcPrefab, familyCenter + offset, Quaternion.identity, spawnParent);
                NPC npc = npcObj.GetComponent<NPC>();

                NPCIdentity identity = npc.GetComponent<NPCIdentity>();
                if (identity != null)
                {
                    identity.lastName = familyLastName;
                    identity.GenerateIdentity();
                    npcObj.name = identity.npcName;
                }
                children.Add(npc);
                familyMembers.Add(npc);
            }

            // Establish parent–child and sibling relationships.
            foreach (NPC child in children)
            {
                NPCIdentity childIdentity = child.GetComponent<NPCIdentity>();
                foreach (NPC parent in parents)
                {
                    NPCIdentity parentIdentity = parent.GetComponent<NPCIdentity>();
                    parentIdentity.AddChild(childIdentity);
                }
            }
            for (int i = 0; i < children.Count; i++)
            {
                NPCIdentity childA = children[i].GetComponent<NPCIdentity>();
                for (int j = i + 1; j < children.Count; j++)
                {
                    NPCIdentity childB = children[j].GetComponent<NPCIdentity>();
                    childA.AddSibling(childB);
                }
            }
        }

        // Form relationships with NPCs outside the family.
        foreach (NPC familyMember in familyMembers)
        {
            FormRelationships(familyMember);
        }

        return familyMembers;
    }

    // Forms relationships between newNPC and all existing NPCs.
    void FormRelationships(NPC newNPC)
    {
        List<NPC> existingNPCs = NPCManager.Instance.GetAllNPCs();
        foreach (NPC other in existingNPCs)
        {
            if (other != newNPC)
            {
                newNPC.relationshipSystem.EvaluateRelationship(other);
            }
        }
    }
}
