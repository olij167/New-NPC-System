using UnityEngine;
using System.Collections.Generic;

public class InteractWithObjectAction : NPCAction
{
    public InteractWithObjectAction() { actionName = "InteractWithObject"; }

    public override float GetUtility(NPC npc)
    {
        // Get the NPC's perception system.
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception == null)
            return 0f;

        // Look through the prioritized list for an interactable object.
        List<GameObject> sorted = perception.GetPrioritizedPerceivedObjects();
        foreach (GameObject obj in sorted)
        {
            // Check if the object has an InteractableObject component.
            if (obj.GetComponent<InteractableObject>() != null)
            {
                // Use the object's attention score as the base utility.
                float score = perception.GetAttentionScore(obj);
                // If the score exceeds a threshold, consider this action.
                if (score > 1.0f)
                    return score;
            }
        }
        return 0f;
    }

    public override void Execute(NPC npc)
    {
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        List<GameObject> sorted = perception.GetPrioritizedPerceivedObjects();
        GameObject target = null;

        // Find the first interactable object.
        foreach (GameObject obj in sorted)
        {
            if (obj.GetComponent<InteractableObject>() != null)
            {
                target = obj;
                break;
            }
        }

        if (target != null)
        {
            Debug.Log(npc.name + " interacts with object: " + target.name);
            // Call the object's interaction method.
            InteractableObject interactable = target.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Interact(npc);
            }
        }
    }
}
