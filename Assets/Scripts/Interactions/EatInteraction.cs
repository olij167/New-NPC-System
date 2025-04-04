using UnityEngine;

[CreateAssetMenu(fileName = "NewEatInteraction", menuName = "WorldInteraction/Eat Interaction", order = 1)]
public class EatInteraction : InteractionAction
{
    public override void Execute(NPC npc)
    {
        if (npc == null) return;
        Debug.Log(npc.identity.npcName + " is eating using EatInteraction.");

        if (npc.needsSystem != null)
        {
            // Find the "Hunger" need and reduce its value.
            Need hungerNeed = npc.needsSystem.needs.Find(n => n.needName == "Hunger");
            if (hungerNeed != null)
            {
                float reduction = satisfactionValue * 0.5f; // Example factor.
                hungerNeed.currentValue = Mathf.Max(0, hungerNeed.currentValue - reduction);
                Debug.Log(npc.identity.npcName + "'s Hunger reduced by " + reduction.ToString("F2"));
            }
        }
        npc.TriggerEvent(new GameEventData(GameEventType.PlayerInteraction,
            npc.identity.npcName + " eats food", satisfactionValue, npc.identity.npcName));
    }
}
