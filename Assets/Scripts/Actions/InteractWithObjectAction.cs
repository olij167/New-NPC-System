using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractWithObjectAction", menuName = "NPCAction/InteractWithObjectAction")]
public class InteractWithObjectAction : NPCAction
{
    public override float GetUtility(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        if (perception == null)
            return 0f;

        var sorted = perception.GetPrioritizedPerceivedObjects();
        foreach (GameObject obj in sorted)
        {
            if (obj.GetComponent<InteractableObject>() != null)
            {
                float score = perception.GetAttentionScore(obj);
                if (score > 1.0f)
                {
#if UNITY_EDITOR
                    if (UnityEditor.Selection.activeGameObject == npc.gameObject)
                        Debug.Log($"{npc.identity.npcName} found potential object interaction (score: {score:F2})");
#endif
                    return score;
                }
            }
        }
        return 0f;
    }

    public override void ExecuteAction(NPC npc)
    {
        PerceptionSystem perception = npc.perceptionSystem;
        var sorted = perception.GetPrioritizedPerceivedObjects();
        GameObject target = null;
        foreach (GameObject obj in sorted)
        {
            if (obj.GetComponent<InteractableObject>() != null)
            {
                target = obj;
                break;
            }
        }
        if (target != null)
        {
            Debug.Log($"{npc.identity.npcName} interacts with object: {target.name}");
            InteractableObject interactable = target.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Interact(npc);
                Debug.Log($"Interaction executed for object {target.name}");
            }
        }
    }
}
