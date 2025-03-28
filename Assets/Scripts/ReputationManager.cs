using System.Collections.Generic;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    // Dictionary mapping entity identifiers to reputation scores.
    // Reputation values range from -1 (very negative) to 1 (very positive).
    private Dictionary<string, float> reputationScores = new Dictionary<string, float>();

    /// <summary>
    /// Updates the reputation score for a given entity.
    /// </summary>
    /// <param name="entityId">A unique identifier for the entity (e.g., "Player", "NPC_123")</param>
    /// <param name="delta">The change to apply to the reputation score.</param>
    public void UpdateReputation(string entityId, float delta)
    {
        if (reputationScores.ContainsKey(entityId))
        {
            reputationScores[entityId] = Mathf.Clamp(reputationScores[entityId] + delta, -1f, 1f);
        }
        else
        {
            reputationScores[entityId] = Mathf.Clamp(delta, -1f, 1f);
        }
    }

    /// <summary>
    /// Retrieves the current reputation for a given entity.
    /// </summary>
    public float GetReputation(string entityId)
    {
        if (reputationScores.ContainsKey(entityId))
            return reputationScores[entityId];
        return 0f; // Neutral reputation if none is recorded.
    }

    /// <summary>
    /// Aggregates reputation for a given target entity from the memory systems of all NPCs.
    /// Assumes that each memory's GameEventData includes a targetId property.
    /// </summary>
    public void UpdateReputationFromMemories(List<NPC> npcs, string targetEntityId)
    {
        float total = 0f;
        int count = 0;

        foreach (NPC npc in npcs)
        {
            MemorySystem memorySystem = npc.GetComponent<MemorySystem>();
            if (memorySystem != null)
            {
                // Here we filter memories that pertain to the target entity.
                // (Assuming GameEventData has a property "targetId".)
                foreach (var mem in memorySystem.memories)
                {
                    // Only consider memories relevant to the target.
                    if (mem.eventData.targetId == targetEntityId)
                    {
                        // Multiply intensity by significance as a simple weighting.
                        total += mem.significance * mem.eventData.intensity;
                        count++;
                    }
                }
            }
        }

        if (count > 0)
        {
            float average = total / count;
            UpdateReputation(targetEntityId, average);
        }
    }
}
