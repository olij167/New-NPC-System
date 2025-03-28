using System.Collections.Generic;
using UnityEngine;

public class NeedsSystem : MonoBehaviour
{
    [Header("Needs Settings")]
    [Tooltip("List of needs for this NPC.")]
    public List<Need> needs = new List<Need>();

    void Update()
    {
        // Increase each need over time.
        foreach (Need need in needs)
        {
            need.currentValue = Mathf.Clamp01(need.currentValue + need.rateOfIncrease * Time.deltaTime);
        }
    }

    /// <summary>
    /// For a given NeedProvider, returns a multiplier that increases attention if the NPC’s corresponding need is high.
    /// For each need that the provider satisfies, the multiplier is increased proportionally to the need's current value.
    /// </summary>
    public float GetNeedAttentionMultiplier(NeedProvider provider)
    {
        float multiplier = 1f;
        foreach (string needName in provider.satisfiesNeeds)
        {
            Need need = needs.Find(n => n.needName == needName);
            if (need != null)
            {
                // A higher current value means the need is more urgent.
                multiplier += need.currentValue;
            }
        }
        return multiplier;
    }
}
