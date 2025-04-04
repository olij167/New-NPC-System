using UnityEngine;

[CreateAssetMenu(fileName = "NewSocialiseInteraction", menuName = "WorldInteraction/Socialise Interaction", order = 6)]
public class SocialiseInteraction : InteractionAction
{
    public override void Execute(NPC npc)
    {
        if (npc == null)
            return;

        Debug.Log(npc.identity.npcName + " initiates social interaction using SocialiseInteraction.");

        // Get the NPCInteractionManager from the NPC.
        NPCInteractionManager interactionManager = npc.GetComponent<NPCInteractionManager>();
        if (interactionManager == null)
        {
            Debug.LogWarning("NPCInteractionManager not found on " + npc.identity.npcName);
            return;
        }

        // Find a candidate NPC to interact with.
        NPC targetNPC = interactionManager.FindNearestSocialCandidate(npc);
        if (targetNPC == null)
        {
            Debug.Log(npc.identity.npcName + " found no one to interact with.");
            return;
        }

        // Check if the target NPC is available for social interaction.
        bool targetWilling = false;
        NPCInteractionManager targetManager = targetNPC.GetComponent<NPCInteractionManager>();
        if (targetManager != null)
        {
            targetWilling = targetManager.CanSocialise(targetNPC);
        }

        // Send a social interaction message, now including the target NPC.
        interactionManager.SendInteractionMessage(InteractionMessageType.Greeting, satisfactionValue, 0.5f, 0.5f, 0.5f, "Hello!", targetNPC);

        if (targetWilling)
        {
            Debug.Log(npc.identity.npcName + " and " + targetNPC.identity.npcName + " are now socialising.");
            // Update relationships positively.
            npc.relationshipSystem.UpdateRelationship(targetNPC, 0.2f, 0.1f, 0.1f);
            targetNPC.relationshipSystem.UpdateRelationship(npc, 0.2f, 0.1f, 0.1f);
            // Transition both NPCs to a social state here (state machine integration).
        }
        else
        {
            Debug.Log(npc.identity.npcName + " attempted social interaction with " + targetNPC.identity.npcName + " but got no response.");
            // Apply negative effects due to rejection.
            npc.emotionSystem.happiness = Mathf.Clamp01(npc.emotionSystem.happiness - 0.1f);
            targetNPC.emotionSystem.happiness = Mathf.Clamp01(targetNPC.emotionSystem.happiness - 0.1f);
            npc.relationshipSystem.UpdateRelationship(targetNPC, -0.2f, -0.1f, -0.1f);
            targetNPC.relationshipSystem.UpdateRelationship(npc, -0.2f, -0.1f, -0.1f);
        }
    }
}
