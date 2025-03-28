using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotentialOpinionsDatabase", menuName = "NPC/PotentialOpinionsDatabase", order = 1)]
public class PotentialOpinionsDatabase : ScriptableObject
{
    [Header("Potential Concept Opinions")]
    public List<ConceptOpinion> potentialConceptOpinions = new List<ConceptOpinion>();

    [Header("Potential Tangible Opinions")]
    public List<TangibleOpinion> potentialTangibleOpinions = new List<TangibleOpinion>();
}
