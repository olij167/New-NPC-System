using UnityEngine;
using Pathfinding; // For A* pathfinding

public class ExploreAction : NPCAction
{
    private Vector3 currentDestination;
    private float destinationTimer = 0f;
    // Duration (in seconds) before a new destination is chosen.
    private float destinationDuration = 10f; 

    public ExploreAction()
    {
        actionName = "Explore";
        currentDestination = Vector3.zero;
    }

    public override float GetUtility(NPC npc)
    {
        // Return a base utility for exploring.
        // This value could be adjusted based on how long the NPC has been idle.
        return 0.5f;
    }

    public override void ExecuteAction(NPC npc)
    {
        // Increment timer.
        destinationTimer += Time.deltaTime;

        // Recalculate destination only if it hasn't been set or the duration has passed.
        if (Vector3.Distance(npc.transform.position, currentDestination) <= 1f || destinationTimer >= destinationDuration)
        {
            currentDestination = GetRandomWalkablePoint(npc.transform.position);
            destinationTimer = 0f;
        }

        NavigationController nav = npc.GetComponent<NavigationController>();
        if (nav != null)
        {
            nav.MoveTo(currentDestination);
            Debug.Log(npc.identity.npcName + " is exploring to " + currentDestination);
        }
    }

    private Vector3 GetRandomWalkablePoint(Vector3 currentPos)
    {
        float range = 20f;
        Vector3 candidate = currentPos + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        // Snap candidate to the recast graph using A* Pathfinding Pro.
        NNInfo nearest = AstarPath.active.GetNearest(candidate);
        return nearest.position;
    }
}
