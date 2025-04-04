using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkAction", menuName = "NPCAction/WorkAction")]
public class WorkAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        if (npc.factionMembership != null)
        {
            foreach (Faction f in npc.factionMembership.factions)
            {
                if (f.factionType == FactionType.Business)
                    return 0.5f;
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == npc.gameObject)
            Debug.Log($"{npc.identity.npcName} begins working.");
#endif
        // Implement working behavior.
    }
}
