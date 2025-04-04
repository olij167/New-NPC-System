using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewFleeAction", menuName = "NPCAction/FleeAction")]
public class FleeAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        if (perception != null)
        {
            var sorted = perception.GetPrioritizedPerceivedObjects();
            if (sorted.Count > 0)
            {
                float highestScore = perception.GetAttentionScore(sorted[0]);
                if (highestScore > 1.5f)
                    return 1.0f;
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)
            Debug.Log($"{npc.identity.npcName} is fleeing.");
#endif
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        if (agent != null && npc.perceptionSystem.perceivedObjects.Count > 0)
        {
            GameObject threat = npc.perceptionSystem.perceivedObjects[0];
            Vector3 directionAway = (npc.transform.position - threat.transform.position).normalized;
            Vector3 fleeDestination = npc.transform.position + directionAway * 10f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeDestination, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        npc.TriggerEvent(new GameEventData(GameEventType.WitnessedAction,
            $"{npc.identity.npcName} flees from threat", 0.7f, npc.identity.npcName));
    }
}
