using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "NPC/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("Friend Group Settings")]
    public float sentimentThreshold = 0.3f;
    public float familiarityThreshold = 0.5f;
    public int minimumFriendGroupSize = 3;

    [Header("Perception Weights")]
    public float visionWeight = 0.6f;
    public float audioWeight = 0.4f;
}
