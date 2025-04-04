using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionAction : ScriptableObject
{
    [Tooltip("Unique name of the interaction action.")]
    public string actionName;

    [Tooltip("Base satisfaction value for this interaction (higher means more effective).")]
    public float satisfactionValue = 1f;

    [Tooltip("Modifiers that affect NPC parameters when this action is performed. For example, 'Energy': +0.2, 'Hunger': -0.3.")]
    public Dictionary<string, float> modifiers = new Dictionary<string, float>();

    /// <summary>
    /// Executes the interaction action on the specified NPC.
    /// Subclasses should override this method to implement their specific effects.
    /// </summary>
    public abstract void Execute(NPC npc);
}
