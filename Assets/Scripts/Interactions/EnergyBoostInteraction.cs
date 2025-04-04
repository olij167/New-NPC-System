using UnityEngine;

[CreateAssetMenu(fileName = "NewEnergyBoostInteraction", menuName = "WorldInteraction/EnergyBoost Interaction", order = 3)]
public class EnergyBoostInteraction : InteractionAction
{
    public override void Execute(NPC npc)
    {
        if (npc == null) return;
        Debug.Log(npc.identity.npcName + " receives an energy boost using EnergyBoostInteraction.");

        if (npc.needsSystem != null)
        {
            // Assume the NPC has an "Energy" need.
            Need energyNeed = npc.needsSystem.needs.Find(n => n.needName == "Energy");
            if (energyNeed != null)
            {
                float reduction = satisfactionValue * 0.8f; // Example factor.
                energyNeed.currentValue = Mathf.Max(0, energyNeed.currentValue - reduction);
                Debug.Log(npc.identity.npcName + "'s Energy need reduced by " + reduction.ToString("F2"));
            }
        }
        npc.TriggerEvent(new GameEventData(GameEventType.PlayerInteraction,
            npc.identity.npcName + " gets an energy boost", satisfactionValue, npc.identity.npcName));
    }
}
