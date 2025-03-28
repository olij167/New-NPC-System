using UnityEngine;

public class NPC : MonoBehaviour
{
    // Core identity and personality systems.
    public NPCIdentity identity;
    public Personality personality;
    public Sexuality sexuality;

    // Memory and relationship systems.
    public MemorySystem memorySystem;
    public RelationshipSystem relationshipSystem;

    // Perception and needs systems.
    public PerceptionSystem perceptionSystem;
    public NeedsSystem needsSystem;

    // Emotion system.
    public EmotionSystem emotionSystem;

    // Decision-making component.
    public DecisionMaker decisionMaker;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Automatically locate components if they haven't been assigned manually.
        if (identity == null) identity = GetComponent<NPCIdentity>();
        if (personality == null) personality = GetComponent<Personality>();
        if (sexuality == null) sexuality = GetComponent<Sexuality>();
        if (memorySystem == null) memorySystem = GetComponent<MemorySystem>();
        if (relationshipSystem == null) relationshipSystem = GetComponent<RelationshipSystem>();
        if (perceptionSystem == null) perceptionSystem = GetComponent<PerceptionSystem>();
        if (needsSystem == null) needsSystem = GetComponent<NeedsSystem>();
        if (emotionSystem == null) emotionSystem = GetComponent<EmotionSystem>();
        if (decisionMaker == null) decisionMaker = GetComponent<DecisionMaker>();
    }

    // Example method to trigger an in-game event.
    // This can be used by external systems or for debugging purposes.
    public void TriggerEvent(GameEventData eventData)
    {
        if (memorySystem != null)
        {
            memorySystem.RecordMemory(eventData, 1.0f);
        }
    }

    // Additional methods related to NPC behavior can be added here.
}
