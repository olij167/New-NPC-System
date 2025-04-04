using UnityEngine;

public class InteractState : INPCState
{
    public string StateName { get { return "Interact"; } }

    public void Enter(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " enters Interact State.");
        NPCInteractionManager interactionManager = npc.GetComponent<NPCInteractionManager>();
        if (interactionManager != null)
        {
            interactionManager.InitiateInteraction();
        }
    }

    public void UpdateState(NPC npc)
    {
        // If the interaction session ends, transition back to Explore.
        NPCInteractionManager interactionManager = npc.GetComponent<NPCInteractionManager>();
        if (interactionManager != null && (interactionManager.currentSession == null || !interactionManager.currentSession.IsActive()))
        {
            npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
        }
    }

    public void Exit(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " exits Interact State.");
    }
}
