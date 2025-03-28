using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("A brief description of this interactable object.")]
    public string description;

    public void Interact(NPC interactor)
    {
        Debug.Log(interactor.name + " interacts with " + gameObject.name + ": " + description);
        // Implement additional behavior here (e.g., picking up an item, opening a door).
    }
}
