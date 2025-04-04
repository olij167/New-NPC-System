using UnityEngine;

[CreateAssetMenu(fileName = "NewEatAction", menuName = "NPCAction/EatAction")]
public class EatAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        if (npc.needsSystem != null)
        {
            Need hungerNeed = npc.needsSystem.needs.Find(n => n.needName == "Hunger");
            if (hungerNeed != null)
            {
                return 1f - hungerNeed.currentValue;
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)
            Debug.Log($"{npc.identity.npcName} starts eating.");
#endif
        // Implement eating behavior.
    }
}
