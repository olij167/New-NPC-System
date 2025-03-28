using UnityEngine;

[System.Serializable]
public class Need
{
    [Tooltip("The name of the need (e.g., 'Hunger', 'Thirst', 'Sleep').")]
    public string needName;

    [Tooltip("Current level of the need (0 means fully satisfied, 1 means critical need).")]
    [Range(0f, 1f)]
    public float currentValue;

    [Tooltip("Rate at which this need increases over time.")]
    public float rateOfIncrease = 0.01f;
}
