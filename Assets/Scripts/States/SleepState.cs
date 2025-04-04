using UnityEngine;

public class SleepState : INPCState
{
    public string StateName { get { return "Sleep"; } }

    public void Enter(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " enters Sleep State.");
        NPCScheduling scheduling = npc.GetComponent<NPCScheduling>();
        if (scheduling != null)
        {
            NavigationController nav = npc.GetComponent<NavigationController>();
            if (nav != null)
            {
                Vector3 sleepLocation = scheduling.GetDestination();
                nav.MoveTo(sleepLocation);
                Debug.Log(npc.identity.npcName + " moving to sleep location: " + sleepLocation);
            }
        }
    }

    public void UpdateState(NPC npc)
    {
        // Check if sleep need is now satisfied; then transition back to Explore.
        if (npc.needsSystem != null)
        {
            Need sleepNeed = npc.needsSystem.needs.Find(n => n.needName == "Sleep");
            if (sleepNeed != null && sleepNeed.currentValue >= 0.8f)
            {
                npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
            }
        }
    }

    public void Exit(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " exits Sleep State.");
    }
}
