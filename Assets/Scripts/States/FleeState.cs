using UnityEngine;

public class FleeState : INPCState
{
    public string StateName { get { return "Flee"; } }
    private Vector3 fleeDestination;

    public void Enter(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " enters Flee State.");
        // Determine threat and choose flee destination.
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception != null && perception.perceivedObjects.Count > 0)
        {
            GameObject threat = perception.perceivedObjects[0];
            Vector3 directionAway = (npc.transform.position - threat.transform.position).normalized;
            fleeDestination = npc.transform.position + directionAway * 10f;
            NavigationController nav = npc.GetComponent<NavigationController>();
            if (nav != null)
            {
                nav.MoveTo(fleeDestination);
                Debug.Log(npc.identity.npcName + " flees to " + fleeDestination);
            }
        }
    }

    public void UpdateState(NPC npc)
    {
        // Remain in Flee until threat is no longer detected.
        PerceptionSystem perception = npc.GetComponent<PerceptionSystem>();
        if (perception != null && perception.GetPrioritizedPerceivedObjects().Count == 0)
        {
            npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
        }
    }

    public void Exit(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " exits Flee State.");
    }
}
