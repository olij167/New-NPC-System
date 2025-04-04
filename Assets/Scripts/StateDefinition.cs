using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStateDefinition", menuName = "NPC/State Definition", order = 1)]
public class StateDefinition : ScriptableObject
{
    [Tooltip("Unique name of the state.")]
    public string stateName;

    [Tooltip("Minimum duration (in seconds) that the NPC should remain in this state before considering a transition.")]
    public float minDuration = 2f;

    [Tooltip("Optional priority value influencing transition evaluations (higher means more dominant).")]
    public float priority = 1f;

    [Tooltip("List of allowed transitions from this state. Direct references to other StateDefinition assets.")]
    public List<StateDefinition> allowedTransitions = new List<StateDefinition>();

    [Header("State Actions")]
    [Tooltip("List of NPCActions that are executed when the state is entered.")]
    public List<NPCAction> actions = new List<NPCAction>();

    /// <summary>
    /// Determines whether a transition to a new state is allowed.
    /// </summary>
    public bool IsTransitionAllowed(StateDefinition newState)
    {
        return allowedTransitions != null && allowedTransitions.Contains(newState);
    }

    /// <summary>
    /// Called when the NPC enters this state.
    /// Executes all NPCActions assigned to this state.
    /// </summary>
    public void EnterState(NPC npc)
    {
        if (actions != null)
        {
            foreach (NPCAction action in actions)
            {
                if (action != null)
                {
                    action.ExecuteAction(npc);
                }
            }
        }
    }
}
