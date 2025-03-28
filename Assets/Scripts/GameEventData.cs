using UnityEngine;

[System.Serializable]
public enum GameEventType { PlayerInteraction, WitnessedAction, EnvironmentalChange }

[System.Serializable]
public class GameEventData
{
    public GameEventType eventType;
    public string description;
    public float intensity;
    public float timestamp;
    public string targetId;

    public GameEventData(GameEventType type, string desc, float intens, string target)
    {
        eventType = type;
        description = desc;
        intensity = intens;
        timestamp = Time.time;
        targetId = target;
    }
}
