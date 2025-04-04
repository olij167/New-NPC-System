using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // Optional: if you need A* queries for positioning.

public class WorldInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Required interaction range (in world units) for an NPC to interact with this object.")]
    public float interactionRange = 2f;

    [Tooltip("Optional transform specifying the exact point where NPCs should interact. If null, the object's position is used.")]
    public Transform interactionPoint;

    [Tooltip("Maximum number of NPCs that can interact with this object concurrently.")]
    public int capacity = 1;

    // Current NPCs interacting with this object.
    public List<NPC> currentUsers = new List<NPC>();

    [Header("Interaction Quality")]
    [Tooltip("A measure of how satisfying this interactable is (higher values indicate a more satisfying interaction).")]
    public float satisfactionValue = 1.0f;

    [Header("Available Interaction Actions")]
    [Tooltip("List of possible interaction actions for this object. Designers can add one or more actions (e.g., Sleep, Eat, Energy Boost).")]
    public List<InteractionAction> availableActions = new List<InteractionAction>();

    [Header("Need Influence")]
    [Tooltip("The name of the need that this interactable primarily influences (e.g., 'Hunger', 'Sleep', etc.).")]
    public string targetNeed;

    /// <summary>
    /// Returns the world position where NPCs should interact with this object.
    /// </summary>
    public Vector3 GetInteractionPosition()
    {
        if (interactionPoint != null)
            return interactionPoint.position;
        return transform.position;
    }

    /// <summary>
    /// Checks whether the interactable has free capacity for another NPC.
    /// </summary>
    public bool IsAvailable()
    {
        return currentUsers.Count < capacity;
    }

    /// <summary>
    /// Returns an interaction action from the availableActions list.
    /// If a specific action name is provided, it returns that action (if available);
    /// otherwise, it returns the action with the highest satisfactionValue.
    /// </summary>
    public InteractionAction GetInteractionAction(string specificAction = "")
    {
        if (!string.IsNullOrEmpty(specificAction))
        {
            InteractionAction action = availableActions.Find(a => a.actionName == specificAction);
            if (action != null)
                return action;
        }
        // Return the action with the highest satisfaction value.
        InteractionAction bestAction = null;
        float bestValue = -Mathf.Infinity;
        foreach (var action in availableActions)
        {
            if (action.satisfactionValue > bestValue)
            {
                bestValue = action.satisfactionValue;
                bestAction = action;
            }
        }
        return bestAction;
    }

    /// <summary>
    /// Attempts to add the specified NPC to the list of current users.
    /// Returns true if successful.
    /// </summary>
    public bool EnterInteraction(NPC npc)
    {
        if (IsAvailable() && !currentUsers.Contains(npc))
        {
            currentUsers.Add(npc);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the specified NPC from the list of current users.
    /// </summary>
    public void ExitInteraction(NPC npc)
    {
        if (currentUsers.Contains(npc))
        {
            currentUsers.Remove(npc);
        }
    }
}
