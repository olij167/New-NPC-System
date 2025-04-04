using UnityEngine;
using Pathfinding;

public class ExploreState : INPCState
{
    public string StateName { get { return "Explore"; } }
    private float lastDestinationUpdateTime = 0f;
    private float updateInterval = 3f; // Seconds to force a new destination if idle.

    public void Enter(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " enters Explore State.");
        lastDestinationUpdateTime = Time.time;
        SetNewDestination(npc);
    }

    public void UpdateState(NPC npc)
    {
        NavigationController nav = npc.GetComponent<NavigationController>();
        if (nav == null) return;

        // If NPC has reached destination or it’s been too long, choose a new destination.
        if (nav.HasArrived() || (Time.time - lastDestinationUpdateTime) >= updateInterval)
        {
            SetNewDestination(npc);
            lastDestinationUpdateTime = Time.time;
        }

        // Transition to Work state if scheduling indicates work.
        NPCScheduling scheduling = npc.GetComponent<NPCScheduling>();
        if (scheduling != null && scheduling.currentTask == "Work")
        {
            npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
            return;
        }

        // Transition to Sleep state if sleep need is critical.
        if (npc.needsSystem != null)
        {
            Need sleepNeed = npc.needsSystem.needs.Find(n => n.needName == "Sleep");
            if (sleepNeed != null && sleepNeed.currentValue < 0.2f)
            {
                npc.GetComponent<DecisionMakerStateMachine>().ChangeState(StateName);
                return;
            }
        }

        // (Optional) Transition to Interact state if high social stimuli detected.
        // This logic could check perception for nearby NPCs with high attention scores.
    }

    public void Exit(NPC npc)
    {
        Debug.Log(npc.identity.npcName + " exits Explore State.");
    }

    private void SetNewDestination(NPC npc)
    {
        NavigationController nav = npc.GetComponent<NavigationController>();
        if (nav == null) return;

        float range = 20f;
        Vector3 candidate = npc.transform.position + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        NNInfo nearest = AstarPath.active.GetNearest(candidate);
        Vector3 destination = nearest.position;
        nav.MoveTo(destination);
        Debug.Log(npc.identity.npcName + " sets new destination: " + destination);
    }
}
