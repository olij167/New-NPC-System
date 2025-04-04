using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WorkRoster
{
    [Tooltip("Shift start time in hours (0-24)")]
    public float shiftStartTime = 9f;

    [Tooltip("Shift duration in hours")]
    public float shiftDuration = 8f;

    /// <summary>
    /// Returns true if the provided currentHour is within the defined shift.
    /// </summary>
    public bool IsWithinShift(float currentHour)
    {
        // Assumes shifts do not span midnight.
        return currentHour >= shiftStartTime && currentHour < (shiftStartTime + shiftDuration);
    }
}

public static class WorkRosterManager
{
    // Maps an NPC's instance ID to its assigned work roster.
    private static Dictionary<int, WorkRoster> npcWorkRosters = new Dictionary<int, WorkRoster>();

    /// <summary>
    /// Registers a work roster for a given NPC.
    /// </summary>
    public static void RegisterWorkRoster(NPC npc, WorkRoster roster)
    {
        if (npc == null || roster == null)
            return;
        npcWorkRosters[npc.GetInstanceID()] = roster;
    }

    /// <summary>
    /// Retrieves the work roster for the given NPC.
    /// Returns a default roster (9 AM to 5 PM) if none is registered.
    /// </summary>
    public static WorkRoster GetWorkRosterForNPC(NPC npc)
    {
        if (npc == null)
            return null;
        int key = npc.GetInstanceID();
        if (npcWorkRosters.ContainsKey(key))
            return npcWorkRosters[key];
        else
            return GetDefaultWorkRoster();
    }

    /// <summary>
    /// Provides a default work roster (9 AM to 5 PM).
    /// </summary>
    public static WorkRoster GetDefaultWorkRoster()
    {
        WorkRoster defaultRoster = new WorkRoster();
        defaultRoster.shiftStartTime = 9f;
        defaultRoster.shiftDuration = 8f;
        return defaultRoster;
    }
}
