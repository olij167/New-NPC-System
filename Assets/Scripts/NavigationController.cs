using UnityEngine;
using Pathfinding;  // Ensure A* Pathfinding Pro is imported

[RequireComponent(typeof(RichAI))]
public class NavigationController : MonoBehaviour
{
    private RichAI richAI;
    private Vector3 lastDestination;
    private float pathUpdateInterval = 1f; // Update path every 1 second
    private float pathTimer = 0f;

    // Base speed value to be scaled by the time manager.
    public float baseSpeed = 3f;
    private GameTimeManager timeManager;  // Assumes GameTimeManager is set up as a singleton

    void Awake()
    {
        richAI = GetComponent<RichAI>();
        lastDestination = transform.position;
        timeManager = GameTimeManager.Instance; // GameTimeManager should implement a singleton pattern

        // Set initial speed based on baseSpeed using maxSpeed.
        richAI.maxSpeed = baseSpeed;
    }

    void Update()
    {
        // Update speed based on the current time scale.
        if (timeManager != null)
        {
            richAI.maxSpeed = baseSpeed * timeManager.timeScale;
        }

        // Update path recalculation at intervals.
        pathTimer += Time.deltaTime;
        if (pathTimer >= pathUpdateInterval)
        {
            if (Vector3.Distance(lastDestination, richAI.destination) > 1f)
            {
                // Trigger recalculation.
                richAI.destination = richAI.destination;
                lastDestination = richAI.destination;
            }
            pathTimer = 0f;
        }
    }

    /// <summary>
    /// Sets a new destination for the NPC.
    /// </summary>
    public void MoveTo(Vector3 destination)
    {
        NNInfo nearest = AstarPath.active.GetNearest(destination);
        Vector3 newDestination = nearest.position;

        if (Vector3.Distance(newDestination, lastDestination) > 1f)
        {
            richAI.destination = newDestination;
            lastDestination = newDestination;
        }
    }

    /// <summary>
    /// Returns true if the NPC is considered to have reached its destination.
    /// </summary>
    public bool HasArrived()
    {
        float threshold = (richAI.endReachedDistance > 0) ? richAI.endReachedDistance : 1f;
        return Vector3.Distance(transform.position, richAI.destination) <= threshold;
    }
}
