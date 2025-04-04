using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MonthData
{
    public string monthName = "January";
    [Tooltip("Duration of this month in days.")]
    public int durationDays = 31;
    [Tooltip("Mid-month sun tilt in degrees. In the southern hemisphere, higher values indicate a higher sun (summer) and lower values a lower sun (winter).")]
    public float midMonthSunTilt = 85f;
}

[CreateAssetMenu(fileName = "TimeSettingsData", menuName = "Time/TimeSettingsData", order = 1)]
public class TimeSettingsData : ScriptableObject
{
    [Tooltip("List of months in order for the in-game calendar.")]
    public List<MonthData> months = new List<MonthData>()
    {
        // Default settings for the southern hemisphere.
        new MonthData() { monthName = "January", durationDays = 31, midMonthSunTilt = 85f },
        new MonthData() { monthName = "February", durationDays = 28, midMonthSunTilt = 85f },
        new MonthData() { monthName = "March", durationDays = 31, midMonthSunTilt = 75f },
        new MonthData() { monthName = "April", durationDays = 30, midMonthSunTilt = 70f },
        new MonthData() { monthName = "May", durationDays = 31, midMonthSunTilt = 65f },
        new MonthData() { monthName = "June", durationDays = 30, midMonthSunTilt = 30f },
        new MonthData() { monthName = "July", durationDays = 31, midMonthSunTilt = 30f },
        new MonthData() { monthName = "August", durationDays = 31, midMonthSunTilt = 35f },
        new MonthData() { monthName = "September", durationDays = 30, midMonthSunTilt = 60f },
        new MonthData() { monthName = "October", durationDays = 31, midMonthSunTilt = 70f },
        new MonthData() { monthName = "November", durationDays = 30, midMonthSunTilt = 75f },
        new MonthData() { monthName = "December", durationDays = 31, midMonthSunTilt = 85f }
    };
}

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Tooltip("Real seconds per in-game hour.")]
    public float timeScale = 60f;
    [Tooltip("The starting in-game date and time.")]
    public DateTime startTime = new DateTime(2025, 1, 1, 8, 0, 0);
    [Tooltip("Optional custom time settings asset. If not provided, default settings will be used.")]
    public TimeSettingsData timeSettingsData;
    [Tooltip("Reference to the directional light representing the sun.")]
    public Light sun;
    [Tooltip("Time of sunrise (hour, 0-24).")]
    public float sunriseTime = 6f;
    [Tooltip("Time of sunset (hour, 0-24).")]
    public float sunsetTime = 18f;

    private DateTime currentTime;
    private int currentDayOfYear;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally persist this manager between scenes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        currentTime = startTime;
        if (timeSettingsData == null)
        {
            Debug.LogWarning("No TimeSettingsData assigned; using default settings.");
        }
    }

    void Update()
    {
        // Advance time based on timeScale.
        float deltaHours = Time.deltaTime / timeScale;
        currentTime = currentTime.AddHours(deltaHours);
        currentDayOfYear = currentTime.DayOfYear;
        UpdateSunRotation();
    }

    void UpdateSunRotation()
    {
        if (sun == null)
            return;

        float hour = GetCurrentHour();

        // If within the defined sunrise-sunset interval, calculate sun elevation.
        if (hour >= sunriseTime && hour <= sunsetTime)
        {
            // Normalize time between sunrise and sunset.
            float normalizedTime = Mathf.InverseLerp(sunriseTime, sunsetTime, hour);
            // Sine-based daily elevation: 0° at sunrise, peaks at 90° at midday, returns to 0° at sunset.
            float dailyElevation = Mathf.Sin(normalizedTime * Mathf.PI) * 90f;

            // Retrieve seasonal sun tilt from custom time settings.
            float seasonalTilt = GetSeasonalSunTilt();

            // For a southern-hemisphere default, subtract the seasonal tilt.
            float finalElevation = dailyElevation - seasonalTilt;

            // Update the sun's rotation around the X-axis.
            sun.transform.rotation = Quaternion.Euler(finalElevation, 0, 0);
        }
        else
        {
            // At night, set sun to a low angle (below the horizon).
            sun.transform.rotation = Quaternion.Euler(-10f, 0, 0);
        }
    }

    float GetSeasonalSunTilt()
    {
        if (timeSettingsData != null && timeSettingsData.months != null && timeSettingsData.months.Count > 0)
        {
            int dayOfYear = currentTime.DayOfYear;
            int cumulativeDays = 0;
            MonthData currentMonth = null;
            foreach (var month in timeSettingsData.months)
            {
                cumulativeDays += month.durationDays;
                if (dayOfYear <= cumulativeDays)
                {
                    currentMonth = month;
                    break;
                }
            }
            if (currentMonth != null)
            {
                // Optionally, interpolate within the month for smoother transitions.
                return currentMonth.midMonthSunTilt;
            }
        }
        return 60f; // Fallback default.
    }

    // Helper methods:
    public DateTime GetCurrentTime()
    {
        return currentTime;
    }

    public float GetCurrentHour()
    {
        return currentTime.Hour + currentTime.Minute / 60f;
    }

    public DayOfWeek GetCurrentDayOfWeek()
    {
        return currentTime.DayOfWeek;
    }

    public int GetCurrentMonth()
    {
        return currentTime.Month;
    }

    public int GetCurrentYear()
    {
        return currentTime.Year;
    }

    public string GetCurrentDateString()
    {
        return currentTime.ToString("dd-MM-yyyy");
    }
}
