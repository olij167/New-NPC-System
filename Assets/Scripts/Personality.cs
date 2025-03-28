using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour
{
    [Header("Default Emotion Values")]
    [Range(0f, 1f)]
    public float defaultHappiness = 0.5f;
    [Range(0f, 1f)]
    public float defaultPassion = 0.5f;
    [Range(0f, 1f)]
    public float defaultConfidence = 0.5f;

    [Header("Personality Opinions")]
    [Tooltip("List of concept opinions currently held by the NPC.")]
    public List<ConceptOpinion> conceptOpinions = new List<ConceptOpinion>();

    [Tooltip("List of tangible interests currently held by the NPC.")]
    public List<TangibleOpinion> tangibleInterests = new List<TangibleOpinion>();

    [Tooltip("List of tangible fears currently held by the NPC.")]
    public List<TangibleOpinion> tangibleFears = new List<TangibleOpinion>();

    [Header("Potential Opinions Database")]
    [Tooltip("Reference to the global database of potential opinions.")]
    public PotentialOpinionsDatabase opinionsDatabase;

    /// <summary>
    /// Retrieves the abstract opinion for a given concept name.
    /// </summary>
    public ConceptOpinion GetConceptOpinion(string conceptName)
    {
        return conceptOpinions.Find(opinion => opinion.conceptName.Equals(conceptName, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Retrieves the tangible interest for a given tangible name.
    /// </summary>
    public TangibleOpinion GetTangibleInterest(string tangibleName)
    {
        return tangibleInterests.Find(opinion => opinion.tangibleName.Equals(tangibleName, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Retrieves the tangible fear for a given tangible name.
    /// </summary>
    public TangibleOpinion GetTangibleFear(string tangibleName)
    {
        return tangibleFears.Find(opinion => opinion.tangibleName.Equals(tangibleName, System.StringComparison.OrdinalIgnoreCase));
    }
}
