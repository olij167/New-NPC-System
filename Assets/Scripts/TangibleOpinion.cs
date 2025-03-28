using System;
using UnityEngine;

[Serializable]
public class TangibleOpinion
{
    [Tooltip("Name of the tangible object or concept, e.g. 'Sword' or 'Dark Alley'")]
    public string tangibleName;

    [Tooltip("Strength of this opinion (0 for no interest/dislike, up to 1 for high interest/dislike)")]
    [Range(0f, 1f)]
    public float intensity;

    [Tooltip("Optional description for further details")]
    [TextArea]
    public string description;
}
