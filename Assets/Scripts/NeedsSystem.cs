using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Need
{
    [Tooltip("The name of the need (e.g., 'Hunger', 'Thirst', 'Sleep').")]
    public string needName;
    [Range(0f, 1f)]
    [Tooltip("Current level of the need (1 means fully satisfied, 0 means critical need).")]
    public float currentValue = 1f;
    [Tooltip("Rate at which the need decreases over time.")]
    public float rateOfDecrease = 0.01f;
}

public class NeedsSystem : MonoBehaviour
{
    [Header("Needs Settings")]
    public List<Need> needs = new List<Need>();

    /// <summary>
    /// Initializes default needs if none are set.
    /// </summary>
    public void InitializeNeeds()
    {
        if (needs == null || needs.Count == 0)
        {
            needs = new List<Need>()
            {
                new Need() { needName = "Hunger", currentValue = 1f, rateOfDecrease = 0.01f },
                new Need() { needName = "Stimulation", currentValue = 1f, rateOfDecrease = 0.015f },
                new Need() { needName = "Sleep", currentValue = 1f, rateOfDecrease = 0.02f }
                // Add more needs as necessary.
            };
        }
    }

    void Update()
    {
        // Decrease each need over time.
        foreach (Need need in needs)
        {
            need.currentValue = Mathf.Clamp01(need.currentValue - need.rateOfDecrease * Time.deltaTime);
        }
    }

    /// <summary>
    /// For a given WorldInteractable, returns a multiplier that increases attention if the NPC's corresponding need is low.
    /// The WorldInteractable should specify a target need via the 'targetNeed' field.
    /// The multiplier is increased inversely to the need's current value and scaled by the best interaction's satisfactionValue.
    /// If no target need is specified or the need is not found, returns a default multiplier of 1.
    /// </summary>
    public float GetInteractionAttentionMultiplier(WorldInteractable interactable)
    {
        float multiplier = 1f;
        if (interactable == null)
            return multiplier;

        // Check if the interactable specifies a target need.
        if (!string.IsNullOrEmpty(interactable.targetNeed))
        {
            // Assume the NPC has a NeedsSystem component.
            NeedsSystem needsSystem = GetComponent<NeedsSystem>();
            if (needsSystem != null)
            {
                Need need = needsSystem.needs.Find(n => n.needName == interactable.targetNeed);
                if (need != null)
                {
                    // Retrieve the best interaction action for this interactable.
                    InteractionAction bestAction = interactable.GetInteractionAction();
                    float satisfaction = bestAction != null ? bestAction.satisfactionValue : 1f;
                    // A lower current need value means the need is more urgent.
                    multiplier += (1f - need.currentValue) * satisfaction;
                }
            }
        }
        return multiplier;
    }
}
