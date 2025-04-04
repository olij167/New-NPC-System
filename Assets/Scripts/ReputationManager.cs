using System.Collections.Generic;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    private Dictionary<string, float> reputationScores = new Dictionary<string, float>();

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
        Debug.Log("[ReputationManager] Updated reputation for " + entityId + " to " + reputationScores[entityId].ToString("F2"));
    }

    public float GetReputation(string entityId)
    {
        if (reputationScores.ContainsKey(entityId))
            return reputationScores[entityId];
        return 0f;
    }

    public void UpdateReputationFromMemories(List<NPC> npcs, string targetEntityId)
    {
        float total = 0f;
        int count = 0;
        foreach (NPC npc in npcs)
        {
            MemorySystem memorySystem = npc.GetComponent<MemorySystem>();
            if (memorySystem != null)
            {
                foreach (var memory in memorySystem.memories)
                {
                    if (memory.eventData.targetId == targetEntityId)
                    {
                        total += memory.significance * memory.eventData.intensity;
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
