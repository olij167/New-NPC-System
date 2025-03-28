using System.Collections.Generic;
using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    [System.Serializable]
    public class Memory
    {
        [Tooltip("The event data recorded as a memory.")]
        public GameEventData eventData;
        [Tooltip("The time the event was recorded.")]
        public float timeRecorded;
        [Tooltip("A multiplier that represents how significant this event was for the NPC.")]
        public float significance;

        public Memory(GameEventData eventData, float significance)
        {
            this.eventData = eventData;
            this.significance = significance;
            this.timeRecorded = Time.time;
        }
    }

    [Header("Memory Log")]
    public List<Memory> memories = new List<Memory>();

    /// <summary>
    /// Record a new memory with an associated significance.
    /// </summary>
    public void RecordMemory(GameEventData eventData, float significance = 1.0f)
    {
        memories.Add(new Memory(eventData, significance));
    }

    /// <summary>
    /// Optionally remove memories older than a specified lifetime.
    /// </summary>
    public void DecayMemories(float memoryLifetime)
    {
        memories.RemoveAll(memory => (Time.time - memory.timeRecorded) > memoryLifetime);
    }

    /// <summary>
    /// Retrieve all memories that match a custom predicate.
    /// </summary>
    public List<Memory> GetMemories(System.Func<Memory, bool> predicate)
    {
        // Wrap the Func in a lambda that matches System.Predicate<Memory>
        return memories.FindAll(x => predicate(x));
    }
}
