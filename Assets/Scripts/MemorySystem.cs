using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum GameEventType
{
    PlayerInteraction,
    WitnessedAction,
    EnvironmentalChange
}

[System.Serializable]
public class GameEventData
{
    public GameEventType eventType;
    public string description;
    public float intensity;
    public float timestamp;
    public string targetId;

    /// <summary>
    /// Constructs a new GameEventData with an event type, description, intensity, and target ID.
    /// </summary>
    public GameEventData(GameEventType type, string desc, float intens, string target)
    {
        eventType = type;
        description = desc;
        intensity = intens;
        timestamp = Time.time;
        targetId = target;
    }
}

[Serializable]
public class Memory
{
    public GameEventData eventData;
    public float timeRecorded;
    public float significance;

    public Memory(GameEventData eventData, float significance)
    {
        this.eventData = eventData;
        this.significance = significance;
        timeRecorded = Time.time;
    }
}

public class MemorySystem : MonoBehaviour
{
    public List<Memory> memories = new List<Memory>();

    public void RecordMemory(GameEventData eventData, float significance = 1.0f)
    {
        Memory newMemory = new Memory(eventData, significance);
        memories.Add(newMemory);
    }

    public void DecayMemories(float memoryLifetime)
    {
        memories.RemoveAll(memory => (Time.time - memory.timeRecorded) > memoryLifetime);
    }

    // Returns a modifier for an action based on recent memories.
    // Placeholder logic: sums contributions from memories that mention the action name,
    // decayed exponentially over time.
    public float GetMemoryModifierForAction(string actionName)
    {
        float modifier = 0f;
        foreach (Memory memory in memories)
        {
            if (!string.IsNullOrEmpty(memory.eventData.description) && memory.eventData.description.Contains(actionName))
            {
                float elapsed = Time.time - memory.timeRecorded;
                float decay = Mathf.Exp(-elapsed); // Exponential decay.
                modifier += memory.significance * decay;
            }
        }
        return modifier;
    }
}