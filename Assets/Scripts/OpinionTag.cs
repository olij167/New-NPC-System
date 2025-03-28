using System.Collections.Generic;
using UnityEngine;

public enum OpinionTagType
{
    Concept,   // Used for abstract concepts (e.g., "PropertyOwnership", "Equality", "Racism")
    Tangible   // Used for tangible objects, locations, or actions (e.g., "Sword", "Dark Alley", "Running")
}

public class OpinionTag : MonoBehaviour
{
    [Header("Tag Type")]
    [Tooltip("Select whether this tag relates to an abstract concept or a tangible entity.")]
    public OpinionTagType tagType;

    [Header("Tags")]
    [Tooltip("Add one or more tags that identify the abstract or tangible concept of this object.")]
    public List<string> tags = new List<string>();
}
