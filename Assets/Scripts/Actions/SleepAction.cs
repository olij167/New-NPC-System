using UnityEngine;

[CreateAssetMenu(fileName = "NewSleepAction", menuName = "NPCAction/SleepAction")]
public class SleepAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        if (npc.needsSystem != null)
        {
            Need sleepNeed = npc.needsSystem.needs.Find(n => n.needName == "Sleep");
            if (sleepNeed != null)
            {
                return 1f - sleepNeed.currentValue; // Higher value means more urgent need.
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)
            Debug.Log($"{npc.identity.npcName} goes to sleep.");
#endif
        // Implement sleep behavior, e.g., play sleep animation or change state.
    }
}
