using UnityEngine;

[CreateAssetMenu(fileName = "NewIdleAction", menuName = "NPCAction/IdleAction")]
public class IdleAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        // Moderate utility when nothing urgent is detected.
        return 0.3f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)

            Debug.Log($"{npc.identity.npcName} is idling.");
#endif
        Animator animator = npc.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Idle");
        }
        npc.TriggerEvent(new GameEventData(GameEventType.EnvironmentalChange,
            $"{npc.identity.npcName} is idling", 0.1f, npc.identity.npcName));
    }
}
