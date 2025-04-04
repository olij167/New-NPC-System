using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractWithNPCAction", menuName = "NPCAction/InteractWithNPCAction")]
public class InteractWithNPCAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        if (perception == null)
            return 0f;

        var sorted = perception.GetPrioritizedPerceivedObjects();
        foreach (GameObject obj in sorted)
        {
            NPC other = obj.GetComponent<NPC>();
            if (other != null && other != npc)
            {
                float score = perception.GetAttentionScore(obj);
                if (score > 1.0f)
                {
#if UNITY_EDITOR
                    if (UnityEditor.Selection.activeGameObject == npc.gameObject)
                        Debug.Log($"{npc.identity.npcName} found potential interaction with {other.identity.npcName} (score: {score:F2})");
#endif
                    return score;
                }
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        var sorted = perception.GetPrioritizedPerceivedObjects();
        NPC targetNPC = null;
        foreach (GameObject obj in sorted)
        {
            NPC other = obj.GetComponent<NPC>();
            if (other != null && other != npc && perception.GetAttentionScore(obj) > 1.0f)
            {
                targetNPC = other;
                break;
            }
        }
        if (targetNPC != null)
        {
            Debug.Log($"{npc.identity.npcName} interacts with NPC: {targetNPC.identity.npcName}");
            npc.TriggerEvent(new GameEventData(GameEventType.PlayerInteraction,
                $"{npc.identity.npcName} interacts with {targetNPC.identity.npcName}",
                0.5f, targetNPC.identity.npcName));
            npc.relationshipSystem.UpdateRelationship(targetNPC, 0.3f, 0.2f, 0.1f);
            targetNPC.TriggerEvent(new GameEventData(GameEventType.PlayerInteraction,
                $"{targetNPC.identity.npcName} interacts with {npc.identity.npcName}",
                0.5f, npc.identity.npcName));
            targetNPC.relationshipSystem.UpdateRelationship(npc, 0.3f, 0.2f, 0.1f);
        }
    }
}
