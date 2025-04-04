using UnityEngine;
using System.Collections.Generic;

public class NPCGenetics : MonoBehaviour
{
    public string geneticID;
    public HashSet<string> ancestorIDs = new HashSet<string>();

    // Inherited perception multipliers.
    public float sightMultiplier = 1.0f;
    public float hearingMultiplier = 1.0f;

    // Inheritance breakdown percentages (values between 0 and 1, which sum to 1).
    public float parentAInfluence = 0.45f;
    public float parentBInfluence = 0.45f;
    public float mutationInfluence = 0.10f;

    void Awake()
    {
        geneticID = System.Guid.NewGuid().ToString();
        // Initialize with random perception multipliers as defaults.
        sightMultiplier = Random.Range(0.8f, 1.2f);
        hearingMultiplier = Random.Range(0.8f, 1.2f);
    }

    /// <summary>
    /// Inherits genetic information from two parents, including perception multipliers.
    /// </summary>
    public void InheritFrom(NPCGenetics parentA, NPCGenetics parentB)
    {
        if (parentA != null)
        {
            ancestorIDs.UnionWith(parentA.ancestorIDs);
            ancestorIDs.Add(parentA.geneticID);
        }
        if (parentB != null)
        {
            ancestorIDs.UnionWith(parentB.ancestorIDs);
            ancestorIDs.Add(parentB.geneticID);
        }
        if (parentA != null && parentB != null)
        {
            // Use the breakdown percentages to blend perception multipliers.
            sightMultiplier = ((parentA.sightMultiplier * parentAInfluence) + (parentB.sightMultiplier * parentBInfluence)) + Random.Range(-0.05f, 0.05f);
            hearingMultiplier = ((parentA.hearingMultiplier * parentAInfluence) + (parentB.hearingMultiplier * parentBInfluence)) + Random.Range(-0.05f, 0.05f);
        }
    }

    /// <summary>
    /// Applies genetic inheritance from two parent NPCs to a child.
    /// </summary>
    public static void ApplyGeneticInheritance(NPC child, NPC parentA, NPC parentB)
    {
        NPCGenetics childGenes = child.GetComponent<NPCGenetics>();
        NPCGenetics genesA = parentA.GetComponent<NPCGenetics>();
        NPCGenetics genesB = parentB.GetComponent<NPCGenetics>();

        if (childGenes != null)
        {
            childGenes.InheritFrom(genesA, genesB);
        }
    }

    /// <summary>
    /// Checks whether this NPC shares any common ancestor with another NPC.
    /// </summary>
    public bool SharesCommonAncestor(NPCGenetics other)
    {
        foreach (string id in ancestorIDs)
        {
            if (other.ancestorIDs.Contains(id))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns a formatted string showing the inheritance breakdown for this NPC.
    /// </summary>
    public string GetInheritanceBreakdown()
    {
        float parentAPerc = parentAInfluence * 100f;
        float parentBPerc = parentBInfluence * 100f;
        float mutationPerc = mutationInfluence * 100f;
        return $"Parent A: {parentAPerc:F1}%   Parent B: {parentBPerc:F1}%   Mutation: {mutationPerc:F1}%";
    }
}
