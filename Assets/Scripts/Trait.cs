using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Trait
{
    public string traitName;
    [TextArea]
    public string description;
    public string category; // e.g., "Skill", "Behavior", "Mental", "Social", etc.
    public Dictionary<string, float> modifiers = new Dictionary<string, float>();

    // Clone method for copying traits when inheriting
    public Trait Clone()
    {
        Trait clone = new Trait();
        clone.traitName = this.traitName;
        clone.description = this.description;
        clone.category = this.category;
        clone.modifiers = new Dictionary<string, float>(this.modifiers);
        return clone;
    }
}
