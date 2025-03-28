using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    // Core identity and personality systems.
    public NPCIdentity identity;
    public Personality personality;
    public Sexuality sexuality;

    // Memory and relationship systems.
    public MemorySystem memorySystem;
    public RelationshipSystem relationshipSystem;

    // Additional systems (if needed)
    public NeedsSystem needsSystem;
    public EmotionSystem emotionSystem;

    void Awake()
    {
        if (identity == null) identity = GetComponent<NPCIdentity>();
        if (personality == null) personality = GetComponent<Personality>();
        if (sexuality == null) sexuality = GetComponent<Sexuality>();
        if (memorySystem == null) memorySystem = GetComponent<MemorySystem>();
        if (relationshipSystem == null) relationshipSystem = GetComponent<RelationshipSystem>();
        if (needsSystem == null) needsSystem = GetComponent<NeedsSystem>();
        if (emotionSystem == null) emotionSystem = GetComponent<EmotionSystem>();
    }
}
