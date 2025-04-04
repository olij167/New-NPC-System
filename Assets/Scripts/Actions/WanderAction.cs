using UnityEngine;

[CreateAssetMenu(fileName = "NewWanderAction", menuName = "NPCAction/WanderAction")]
public class WanderAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        return 0.3f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)
            Debug.Log($"{npc.identity.npcName} wanders around.");
#endif
        // Implement wandering behavior.
    }
}
