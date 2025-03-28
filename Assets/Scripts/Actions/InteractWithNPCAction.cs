using UnityEngine;
using System.Collections.Generic;

public class InteractWithNPCAction : NPCAction
{
    public InteractWithNPCAction() { actionName = "InteractWithNPC"; }

    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        if (perception == null)
            return 0f;

        // Look for another NPC in the prioritized perceived objects.
        List<GameObject> sorted = perception.GetPrioritizedPerceivedObjects();
        foreach (GameObject obj in sorted)
        {
            NPC other = obj.GetComponent<NPC>();
            if (other != null && other != npc)
            {
                float score = perception.GetAttentionScore(obj);
                // Use a threshold to decide if the interaction is worth pursuing.
                if (score > 1.0f)
                    return score;
            }
        }
        return 0f;
    }

    public override void Execute(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        List<GameObject> sorted = perception.GetPrioritizedPerceivedObjects();
        NPC targetNPC = null;

        // Find the first NPC in the prioritized list.
        foreach (GameObject obj in sorted)
        {
            NPC other = obj.GetComponent<NPC>();
            if (other != null && other != npc)
            {
                if (perception.GetAttentionScore(obj) > 1.0f)
                {
                    targetNPC = other;
                    break;
                }
            }
        }

        if (targetNPC != null)
        {
            Debug.Log(npc.identity.npcName + " interacts with NPC: " + targetNPC.identity.npcName);

            // Record a memory in the initiating NPC's memory system.
            MemorySystem memSys = npc.memorySystem;
            if (memSys != null)
            {
                // Use targetNPC.identity.npcName as the target string.
                GameEventData evt = new GameEventData(GameEventType.PlayerInteraction,
                                                      npc.identity.npcName + " interacts with " + targetNPC.identity.npcName,
                                                      0.5f, targetNPC.identity.npcName);
                memSys.RecordMemory(evt, 1.0f);
            }

            // Update the relationship system to reflect a friendly interaction.
            if (npc.relationshipSystem != null)
            {
                npc.relationshipSystem.UpdateRelationship(targetNPC, 0.3f, 0.2f, 0.1f);
            }

            // For reciprocal effect, record a memory and update relationship on the target NPC.
            MemorySystem targetMemSys = targetNPC.memorySystem;
            if (targetMemSys != null)
            {
                GameEventData evt2 = new GameEventData(GameEventType.PlayerInteraction,
                                                       targetNPC.identity.npcName + " interacts with " + npc.identity.npcName,
                                                       0.5f, npc.identity.npcName);
                targetMemSys.RecordMemory(evt2, 1.0f);
            }
            if (targetNPC.relationshipSystem != null)
            {
                targetNPC.relationshipSystem.UpdateRelationship(npc, 0.3f, 0.2f, 0.1f);
            }
        }
    }
}
