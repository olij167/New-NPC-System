using UnityEngine;

[CreateAssetMenu(fileName = "NewDrinkAction", menuName = "NPCAction/DrinkAction")]
public class DrinkAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        if (npc.needsSystem != null)
        {
            Need thirstNeed = npc.needsSystem.needs.Find(n => n.needName == "Thirst");
            if (thirstNeed != null)
            {
                return 1f -thirstNeed.currentValue;
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject) 
            Debug.Log($"{npc.identity.npcName} starts drinking.");
#endif
        // Implement drinking behavior.
    }
}
