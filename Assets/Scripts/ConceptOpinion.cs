using System;
using UnityEngine;

[Serializable]
public class ConceptOpinion
{
    [Tooltip("Name of the concept, e.g. 'PropertyOwnership'")]
    public string conceptName;

    [Tooltip("Category of the concept opinion (e.g., Belief or Moral)")]
    public AttributeCategory category;

    [Tooltip("Default intensity (importance) of this concept opinion (0 to 1)")]
    [Range(0f, 1f)]
    public float intensity;

    [Tooltip("Default moral judgement for this concept (-1 for strongly wrong, 1 for strongly right)")]
    [Range(-1f, 1f)]
    public float moralJudgement;

    [Tooltip("Additional description or context for this opinion")]
    [TextArea]
    public string description;
}

public enum AttributeCategory
{
    Belief,
    Moral
}
