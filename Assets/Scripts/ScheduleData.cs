using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleData", menuName = "NPC/ScheduleData", order = 1)]
public class ScheduleData : ScriptableObject
{
    [Serializable]
    public class ScheduleEntry
    {
        public string taskName;
        [Tooltip("Start time in hours (0-24)")]
        public float startTime;
        [Tooltip("Duration in hours")]
        public float duration;
        [Tooltip("Destination position for this task")]
        public Vector3 destination;
    }

    public ScheduleEntry[] entries;
}
