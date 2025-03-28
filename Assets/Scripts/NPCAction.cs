using UnityEngine;

// Base class for NPC actions.
public abstract class NPCAction
{
    public string actionName;

    /// <summary>
    /// Returns a utility value (higher means more desirable) for performing this action,
    /// based on the current state of the NPC.
    /// </summary>
    public abstract float GetUtility(NPC npc);

    /// <summary>
    /// Executes the action.
    /// </summary>
    public abstract void Execute(NPC npc);
}

// Updated IdleAction that checks if nothing of high attention is around.
public class IdleAction : NPCAction
{
    public IdleAction() { actionName = "Idle"; }

    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception != null)
        {
            // If there are no perceived objects, idling is fine.
            if (perception.perceivedObjects.Count == 0)
                return 0.5f;

            var sorted = perception.GetPrioritizedPerceivedObjects();
            float highestScore = perception.GetAttentionScore(sorted[0]);

            // If nothing is drawing significant attention (e.g. score below 1.0), idling is acceptable.
            if (highestScore < 1.0f)
                return 0.5f;
            else
                return 0f;
        }
        return 0.3f;
    }

    public override void Execute(NPC npc)
    {
        Debug.Log(npc.name + " is idling.");
        // Implement additional idle behavior (animation, waiting, etc.) as needed.
    }
}

// Updated PatrolAction that uses the attention score of nearby objects.
public class PatrolAction : NPCAction
{
    public PatrolAction() { actionName = "Patrol"; }

    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception != null)
        {
            var sorted = perception.GetPrioritizedPerceivedObjects();
            if (sorted.Count > 0)
            {
                float highestScore = perception.GetAttentionScore(sorted[0]);
                // If an object draws moderate attention (e.g. between 1.0 and 1.5), patrol may be triggered to investigate.
                if (highestScore >= 1.0f && highestScore < 1.5f)
                    return 0.7f;
            }
        }
        return 0.2f;
    }

    public override void Execute(NPC npc)
    {
        Debug.Log(npc.name + " is patrolling.");
        // Implement patrol behavior (e.g. moving along waypoints, investigating) here.
    }
}

// Updated FleeAction that looks for very high attention scores, assuming danger.
public class FleeAction : NPCAction
{
    public FleeAction() { actionName = "Flee"; }

    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception != null)
        {
            var sorted = perception.GetPrioritizedPerceivedObjects();
            if (sorted.Count > 0)
            {
                float highestScore = perception.GetAttentionScore(sorted[0]);
                // If the top perceived object has a very high attention score (e.g. > 1.5), consider that as a danger signal.
                if (highestScore > 1.5f)
                    return 1.0f;
            }
        }
        return 0f;
    }

    public override void Execute(NPC npc)
    {
        Debug.Log(npc.name + " is fleeing.");
        // Implement fleeing behavior (e.g. moving quickly away from the threat) here.
    }
}
