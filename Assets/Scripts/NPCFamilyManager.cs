using UnityEngine;
using System.Collections.Generic;

public class NPCFamilyManager : MonoBehaviour
{
    // Direct parents and children.
    public List<NPCIdentity> parents = new List<NPCIdentity>();
    public List<NPCIdentity> children = new List<NPCIdentity>();

    public NPCFamilyManager spouse;

    public void AddParent(NPCIdentity parent)
    {
        if (parent == null || parents.Contains(parent))
            return;
        parents.Add(parent);
    }

    public void AddChild(NPCIdentity child)
    {
        if (child == null || children.Contains(child))
            return;
        children.Add(child);

        NPCIdentity self = GetComponent<NPCIdentity>();
        if (self != null)
        {
            NPCFamilyManager childFamily = child.familyManager;
            if (childFamily != null && !childFamily.parents.Contains(self))
            {
                childFamily.parents.Add(self);
            }
        }
    }

    public void SetSpouse(NPCFamilyManager partner)
    {
        spouse = partner;
        if (partner != null)
            partner.spouse = this;
    }
}
