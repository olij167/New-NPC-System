using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    private NPC npc;
    private PerceptionSystem perceptionSystem;

    [Header("Actions")]
    public List<NPCAction> actions = new List<NPCAction>();

    // Decision persistence variables.
    private NPCAction currentAction = null;
    private float decisionTimer = 0f;

    [Header("Decision Timing Settings")]
    [Tooltip("Minimum duration (in seconds) an action should persist before re-evaluation.")]
    public float decisionDuration = 2f;
    [Tooltip("Maximum duration (in seconds) an action may persist before forcing a re-evaluation.")]
    public float maxActionDuration = 5f;
    [Tooltip("Minimum utility difference required to override the current action before max duration is reached.")]
    public float overrideUtilityDifference = 0.5f;

    void Awake()
    {
        npc = GetComponent<NPC>();
        perceptionSystem = GetComponent<PerceptionSystem>();

        // Populate with default actions if none exist.
        if (actions.Count == 0)
        {
            // Add the new need fulfillment action.
            actions.Add(new NeedFulfillmentAction());
            // Existing actions.
            actions.Add(new ExploreAction());
            actions.Add(new PatrolAction());
            actions.Add(new FleeAction());
            actions.Add(new InteractWithObjectAction());
            actions.Add(new InteractWithNPCAction());
            actions.Add(new SleepAction());
            actions.Add(new EatAction());
            actions.Add(new DrinkAction());
            actions.Add(new WorkAction());
            actions.Add(new WanderAction());
        }
    }

    void Update()
    {
        float bestUtility = -Mathf.Infinity;
        NPCAction bestAction = null;

        // Evaluate utility for each available action.
        foreach (NPCAction action in actions)
        {
            float utility = action.GetUtility(npc);
            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestAction = action;
            }
        }

        // Decision persistence logic.
        if (currentAction == null || decisionTimer >= decisionDuration || decisionTimer >= maxActionDuration)
        {
            if (bestAction != null)
            {
                currentAction = bestAction;
                decisionTimer = 0f;
            }
        }
        else
        {
            float currentUtility = (currentAction != null) ? currentAction.GetUtility(npc) : 0f;
            if (bestAction != null && (bestUtility - currentUtility) > overrideUtilityDifference)
            {
                currentAction = bestAction;
                decisionTimer = 0f;
            }
        }

        // Execute the current action.
        if (currentAction != null)
        {
            currentAction.ExecuteAction(npc);
        }

        decisionTimer += Time.deltaTime;
        Debug.Log($"CurrentAction: {currentAction?.actionName ?? "None"}, Timer: {decisionTimer:F2}, BestUtility: {bestUtility:F2}");
    }
}
