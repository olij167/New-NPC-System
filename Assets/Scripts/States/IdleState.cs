using UnityEngine;

public class IdleState : INPCState
{
    public string StateName { get { return "Idle"; } }

    public void Enter(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " enters Idle State.");
        // Optionally, trigger idle animations.
    }

    public void UpdateState(NPC npc)
    {
        // Remain idle; if a stimulus is detected, transition to Explore.
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception != null && perception.GetPrioritizedPerceivedObjects().Count > 0)
        {
            npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
        }
    }

    public void Exit(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " exits Idle State.");
    }
}
