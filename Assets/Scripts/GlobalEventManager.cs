using UnityEngine;
using System;

public class GlobalEventManager : MonoBehaviour
{
    public static GlobalEventManager Instance { get; private set; }

    // Event fired when an NPC is spawned/initialized.
    public event Action<NPC> OnNPCSpawned;
    // Event fired when a relationship is updated.
    public event Action<NPC, NPC, RelationshipInfo> OnRelationshipUpdated;
    // Event fired when perception is updated (or other events as needed).
    public event Action<GameObject> OnPerceptionUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void RaiseNPCSpawned(NPC npc)
    {
        OnNPCSpawned?.Invoke(npc);
    }

    public void RaiseRelationshipUpdated(NPC owner, NPC target, RelationshipInfo info)
    {
        OnRelationshipUpdated?.Invoke(owner, target, info);
    }

    public void RaisePerceptionUpdated(GameObject obj)
    {
        OnPerceptionUpdated?.Invoke(obj);
    }
}
