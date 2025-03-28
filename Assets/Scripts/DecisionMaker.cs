using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    private NPC npc;
    private PerceptionSystem perceptionSystem;

    [Header("Actions")]
    [Tooltip("List of available actions for this NPC. If empty, default actions will be added.")]
    public List<NPCAction> actions = new List<NPCAction>();

    void Awake()
    {
        npc = GetComponent<NPC>();
        perceptionSystem = GetComponent<PerceptionSystem>();

        // If no actions have been assigned via the Inspector, add default sample actions.
        if (actions.Count == 0)
        {
            actions.Add(new IdleAction());
            actions.Add(new PatrolAction());
            actions.Add(new FleeAction());
            actions.Add(new InteractWithObjectAction());
            actions.Add(new InteractWithNPCAction());
        }
    }

    void Update()
    {
        // Retrieve the list of perceived objects from the integrated PerceptionSystem.
        List<GameObject> prioritizedObjects = perceptionSystem.GetPrioritizedPerceivedObjects();
        if (prioritizedObjects.Count > 0)
        {
            float topScore = perceptionSystem.GetCombinedPerceptionScore(prioritizedObjects[0]);
            Debug.Log(npc.identity.npcName + " top combined perception score: " + topScore.ToString("F2"));
        }

        // Evaluate utility for each action based on the current state and perceptions.
        float bestUtility = -Mathf.Infinity;
        NPCAction bestAction = null;
        foreach (NPCAction action in actions)
        {
            float utility = action.GetUtility(npc);
            // Debug.Log(npc.identity.npcName + " evaluated " + action.actionName + " with utility " + utility);
            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestAction = action;
            }
        }

        // Execute the best action if its utility is positive.
        if (bestAction != null && bestUtility > 0)
        {
            bestAction.Execute(npc);
        }
        else
        {
            Debug.Log(npc.identity.npcName + " finds no urgent action, idling.");
        }
    }
}
