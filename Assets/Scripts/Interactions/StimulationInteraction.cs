using UnityEngine;

[CreateAssetMenu(fileName = "NewStimulateInteraction", menuName = "WorldInteraction/Stimulate Interaction", order = 5)]
public class StimulateInteraction : InteractionAction
{
    public override void Execute(NPC npc)
    {
        if (npc == null) return;
        Debug.Log(npc.identity.npcName + " is being stimulated using StimulateInteraction.");

        if (npc.needsSystem != null)
        {
            // Assume the NPC has a "Stimulate" need.
            Need stimulateNeed = npc.needsSystem.needs.Find(n => n.needName == "Stimulate");
            if (stimulateNeed != null)
            {
                float reduction = satisfactionValue * 0.6f; // Example factor.
                stimulateNeed.currentValue = Mathf.Max(0, stimulateNeed.currentValue - reduction);
                Debug.Log(npc.identity.npcName + "'s Stimulate need reduced by " + reduction.ToString("F2"));
            }
        }
        npc.TriggerEvent(new GameEventData(GameEventType.PlayerInteraction,
            npc.identity.npcName + " gets stimulated", satisfactionValue, npc.identity.npcName));
    }
}
