#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(GameTimeManager))]
public class GameTimeManagerEditor : Editor
{
    // String fields for manual input (date and time separately)
    private string newDateStr = "";
    private string newTimeStr = "";

    // Sliders for time-of-day and time scale.
    private float timeOfDaySlider = 0f;
    private float timeScaleSlider = 0f;

    public override void OnInspectorGUI()
    {
        GameTimeManager manager = (GameTimeManager)target;
        EditorGUILayout.Space();

        // === Core Information ===
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Current In-Game Date", EditorStyles.boldLabel);
        DateTime currentTime = manager.GetCurrentTime();
        EditorGUILayout.LabelField(currentTime.ToString("dddd, dd MMMM yyyy"));
        EditorGUILayout.LabelField("Current In-Game Time", currentTime.ToString("HH:mm:ss"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // === Extra Parameters ===
        EditorGUILayout.BeginVertical("box");
        manager.timeScale = EditorGUILayout.FloatField("Time Scale (sec/hr)", manager.timeScale);
        manager.sunriseTime = EditorGUILayout.FloatField("Sunrise Time (hour)", manager.sunriseTime);
        manager.sunsetTime = EditorGUILayout.FloatField("Sunset Time (hour)", manager.sunsetTime);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // === Controls: Date & Time Adjustments ===
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Adjust Date", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Regress Month"))
        {
            try
            {
                DateTime dt = manager.startTime;
                dt = dt.AddMonths(-1);
                manager.startTime = dt;
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error regressing month: " + e.Message);
            }
        }
        if (GUILayout.Button("Regress Day"))
        {
            try
            {
                DateTime dt = manager.startTime;
                dt = dt.AddDays(-1);
                manager.startTime = dt;
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error regressing day: " + e.Message);
            }
        }
        if (GUILayout.Button("Progress Day"))
        {
            try
            {
                DateTime dt = manager.startTime;
                dt = dt.AddDays(1);
                manager.startTime = dt;
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error progressing day: " + e.Message);
            }
        }
        if (GUILayout.Button("Progress Month"))
        {
            try
            {
                DateTime dt = manager.startTime;
                dt = dt.AddMonths(1);
                manager.startTime = dt;
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error progressing month: " + e.Message);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // === Controls: Time-of-Day & Time Scale Sliders ===
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Set Time of Day", EditorStyles.boldLabel);
        // Slider from 0 to 23.99 hours.
        timeOfDaySlider = EditorGUILayout.Slider("Hour", manager.startTime.Hour + manager.startTime.Minute / 60f, 0f, 23.99f);
        if (GUILayout.Button("Apply Time of Day", GUILayout.Height(25)))
        {
            try
            {
                int newHour = Mathf.FloorToInt(timeOfDaySlider);
                int newMinute = Mathf.RoundToInt((timeOfDaySlider - newHour) * 60f);
                DateTime dt = manager.startTime;
                manager.startTime = new DateTime(dt.Year, dt.Month, dt.Day, newHour, newMinute, dt.Second);
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error applying time of day: " + e.Message);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Set Time Scale", EditorStyles.boldLabel);
        timeScaleSlider = EditorGUILayout.Slider("Time Scale (sec/hr)", manager.timeScale, 1f, 300f);
        if (GUILayout.Button("Apply Time Scale", GUILayout.Height(25)))
        {
            manager.timeScale = timeScaleSlider;
            EditorUtility.SetDirty(manager);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // === Controls: Manual Text Input for Date and Time ===
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Manual Date/Time Input", EditorStyles.boldLabel);
        // Use separate text fields for date and time.
        newDateStr = EditorGUILayout.TextField("New Date (dd-MM-yyyy)", manager.startTime.ToString("dd-MM-yyyy"));
        newTimeStr = EditorGUILayout.TextField("New Time (HH:mm:ss)", manager.startTime.ToString("HH:mm:ss"));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply New Date", GUILayout.Height(25)))
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(newDateStr, "dd-MM-yyyy", null);
                DateTime dt = manager.startTime;
                manager.startTime = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, dt.Hour, dt.Minute, dt.Second);
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing new date: " + e.Message);
            }
        }
        if (GUILayout.Button("Apply New Time", GUILayout.Height(25)))
        {
            try
            {
                DateTime parsedTime = DateTime.ParseExact(newTimeStr, "HH:mm:ss", null);
                DateTime dt = manager.startTime;
                manager.startTime = new DateTime(dt.Year, dt.Month, dt.Day, parsedTime.Hour, parsedTime.Minute, parsedTime.Second);
                EditorUtility.SetDirty(manager);
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing new time: " + e.Message);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        // Draw the remaining default inspector properties.
        DrawDefaultInspector();
    }
}
#endif
