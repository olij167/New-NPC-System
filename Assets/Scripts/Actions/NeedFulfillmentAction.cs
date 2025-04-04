using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // if using A* pathfinding

[CreateAssetMenu(fileName = "NewNeedFulfillmentAction", menuName = "NPCAction/NeedFulfillmentAction")]

public class NeedFulfillmentAction : NPCAction
{
    // How frequently (in seconds) to re-check for interactable candidates.
    private float checkInterval = 0.5f;
    private float timer = 0f;

    // Threshold: if a need's current value is below this, it is considered critical.
    private float criticalThreshold = 0.3f;

    // Cached candidate for interaction.
    private GameObject selectedInteractable = null;

    public NeedFulfillmentAction()
    {
        actionName = "NeedFulfillment";
    }

    public override float GetUtility(NPC npc)
    {
        // Ensure the NPC has a needs system.
        if (npc.needsSystem == null)
            return 0f;

        // Determine the most critical need (i.e. the one with the lowest current value).
        Need criticalNeed = null;
        foreach (Need n in npc.needsSystem.needs)
        {
            if (criticalNeed == null || n.currentValue < criticalNeed.currentValue)
                criticalNeed = n;
        }
        if (criticalNeed == null)
            return 0f;

        // Only act on needs that are below our critical threshold.
        if (criticalNeed.currentValue >= criticalThreshold)
            return 0f;

        // Use the NPC's perception system to look for WorldInteractable objects.
        PerceptionSystem ps = npc.perceptionSystem;
        if (ps == null)
            return 0f;

        List<GameObject> candidates = new List<GameObject>();
        foreach (GameObject obj in ps.perceivedObjects)
        {
            WorldInteractable interactable = obj.GetComponent<WorldInteractable>();
            if (interactable != null && interactable.targetNeed.Contains(criticalNeed.needName))
            {
                candidates.Add(obj);
            }
        }

        // If we found candidates, choose the best one.
        if (candidates.Count > 0)
        {
            GameObject bestCandidate = candidates[0];
            float bestScore = 0f;
            foreach (GameObject candidate in candidates)
            {
                WorldInteractable interactable = candidate.GetComponent<WorldInteractable>();
                float satisfaction = interactable.satisfactionValue;
                float distance = Vector3.Distance(npc.transform.position, candidate.transform.position);
                // Compute a score favoring high satisfaction and lower distance.
                float score = satisfaction / Mathf.Max(distance, 0.1f);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCandidate = candidate;
                }
            }
            selectedInteractable = bestCandidate;
            // Utility is high when the need is critical and the best candidate's score is high.
            float utility = (1f - criticalNeed.currentValue) * bestScore;
            return utility;
        }
        // Fallback: if no candidate is found, return a small utility.
        return 0.1f;
    }

    public override void ExecuteAction(NPC npc)
    {
        if (selectedInteractable != null)
        {
            NavigationController nav = npc.GetComponent<NavigationController>();
            if (nav != null)
            {
                // Move toward the interactable's position.
                nav.MoveTo(selectedInteractable.transform.position);

                // When arrived, trigger the interaction.
                WorldInteractable interactable = selectedInteractable.GetComponent<WorldInteractable>();
                if (interactable != null)
                {
                    interactable.EnterInteraction(npc);
                }
            }
        }
    }
}
