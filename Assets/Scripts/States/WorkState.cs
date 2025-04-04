using UnityEngine;

public class WorkState : INPCState
{
    public string StateName { get { return "Work"; } }

    public void Enter(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " enters Work State.");
        NPCScheduling scheduling = npc.GetComponent<NPCScheduling>();
        if (scheduling != null)
        {
            NavigationController nav = npc.GetComponent<NavigationController>();
            if (nav != null)
            {
                Vector3 workLocation = scheduling.GetDestination();
                nav.MoveTo(workLocation);
                Debug.Log(npc.identity.npcName + " moving to work at: " + workLocation);
            }
        }
    }

    public void UpdateState(NPC npc)
    {
        // If scheduling indicates that work is over, transition back to Explore.
        NPCScheduling scheduling = npc.GetComponent<NPCScheduling>();
        if (scheduling != null && scheduling.currentTask != "Work")
        {
            npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
        }
    }

    public void Exit(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " exits Work State.");
    }
}
