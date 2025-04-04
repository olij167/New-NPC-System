using UnityEngine;

public class NPCScheduling : MonoBehaviour
{
    [Tooltip("Assign a ScheduleData asset to define this NPC's routine.")]
    public ScheduleData scheduleData;

    // Current in-game time in hours (0-24).
    private float currentTime;
    public string currentTask = "Idle";

    void Start()
    {
        if (scheduleData == null)
        {
            Debug.LogWarning("No schedule data assigned; using default schedule.");
            scheduleData = CreateDefaultSchedule();
        }
    }

    void Update()
    {
        currentTime = (Time.time / 60f) % 24f;
        UpdateSchedule();
    }

    private void UpdateSchedule()
    {
        if (scheduleData == null || scheduleData.entries.Length == 0)
            return;

        foreach (var entry in scheduleData.entries)
        {
            if (currentTime >= entry.startTime && currentTime < entry.startTime + entry.duration)
            {
                currentTask = entry.taskName;
                return;
            }
        }
        currentTask = "Idle";
    }

    private ScheduleData CreateDefaultSchedule()
    {
        ScheduleData defaultSchedule = ScriptableObject.CreateInstance<ScheduleData>();
        defaultSchedule.entries = new ScheduleData.ScheduleEntry[2];
        defaultSchedule.entries[0] = new ScheduleData.ScheduleEntry()
        {
            taskName = "Work",
            startTime = 9f,
            duration = 8f,
            destination = new Vector3(50, 0, 50)
        };
        defaultSchedule.entries[1] = new ScheduleData.ScheduleEntry()
        {
            taskName = "Sleep",
            startTime = 22f,
            duration = 8f,
            destination = new Vector3(0, 0, 0)
        };
        return defaultSchedule;
    }

    public Vector3 GetDestination()
    {
        if (scheduleData != null && scheduleData.entries != null)
        {
            foreach (var entry in scheduleData.entries)
            {
                if (currentTime >= entry.startTime && currentTime < entry.startTime + entry.duration)
                {
                    return entry.destination;
                }
            }
        }
        return transform.position; // Default to current position if no schedule entry is active.
    }
}
