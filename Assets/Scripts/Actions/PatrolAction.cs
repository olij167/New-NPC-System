using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewPatrolAction", menuName = "NPCAction/PatrolAction")]
public class PatrolAction : NPCAction
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
                if (highestScore >= 1.0f && highestScore < 1.5f)
                    return 0.7f;
            }
        }
        return 0.2f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)
            Debug.Log($"{npc.identity.npcName} is patrolling.");
#endif
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += npc.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        npc.TriggerEvent(new GameEventData(GameEventType.EnvironmentalChange,
            $"{npc.identity.npcName} is patrolling", 0.2f, npc.identity.npcName));
    }
}
