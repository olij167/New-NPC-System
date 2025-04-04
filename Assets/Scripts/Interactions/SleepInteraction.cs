using UnityEngine;

[CreateAssetMenu(fileName = "NewSleepInteraction", menuName = "WorldInteraction/Sleep Interaction", order = 2)]
public class SleepInteraction : InteractionAction
{
    public override void Execute(NPC npc)
    {
        if (npc == null) return;
        Debug.Log(npc.identity.npcName + " is sleeping using SleepInteraction.");

        if (npc.needsSystem != null)
        {
            // Find the "Sleep" need and reduce its value.
            Need sleepNeed = npc.needsSystem.needs.Find(n => n.needName == "Sleep");
            if (sleepNeed != null)
            {
                float reduction = satisfactionValue * 0.7f; // Example factor.
                sleepNeed.currentValue = Mathf.Max(0, sleepNeed.currentValue - reduction);
                Debug.Log(npc.identity.npcName + "'s Sleep need reduced by " + reduction.ToString("F2"));
            }
        }
        npc.TriggerEvent(new GameEventData(GameEventType.PlayerInteraction,
            npc.identity.npcName + " sleeps", satisfactionValue, npc.identity.npcName));
    }
}
