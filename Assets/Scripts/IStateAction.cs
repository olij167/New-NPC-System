using UnityEngine;

public interface IStateAction
{
    float GetUtility(NPC npc);
    void ExecuteAction(NPC npc);
}
