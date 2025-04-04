using UnityEngine;

public abstract class NPCAction : ScriptableObject, IStateAction
{
    // A name for the action (for debugging and display purposes)
    public string actionName;

    /// <summary>
    /// Returns a utility value for performing this action given the current NPC state.
    /// Higher values mean the action is more desirable.
    /// </summary>
    public abstract float GetUtility(NPC npc);

    /// <summary>
    /// Executes this action on the provided NPC.
    /// </summary>
    public abstract void ExecuteAction(NPC npc);
}
