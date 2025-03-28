using UnityEngine;
using System.Collections.Generic;

public enum Gender { Male, Female, Other }

public class NPCIdentity : MonoBehaviour
{
    [Header("Basic Identity")]
    [Tooltip("First name assigned based on gender and NPCManager name lists.")]
    public string firstName;
    [Tooltip("Last name assigned from NPCManager last names list.")]
    public string lastName;
    [Tooltip("Full name: first name + last name.")]
    public string npcName;
    [Tooltip("Randomly generated age.")]
    public int age;
    public Gender gender;

    [Header("Age Settings")]
    public int minAge = 18;
    public int maxAge = 70;

    [Header("Family Relationships")]
    public List<NPCIdentity> parents = new List<NPCIdentity>();
    public List<NPCIdentity> siblings = new List<NPCIdentity>();
    public List<NPCIdentity> children = new List<NPCIdentity>();
    public NPCIdentity spouse;

    /// <summary>
    /// Generates identity values (age, first name, last name) and combines them into npcName.
    /// If lastName is already set (e.g. for family members), it is preserved.
    /// </summary>
    public void GenerateIdentity()
    {
        // Generate a random age.
        age = Random.Range(minAge, maxAge + 1);

        if (NPCManager.Instance != null)
        {
            // Choose first name based on gender.
            switch (gender)
            {
                case Gender.Male:
                    firstName = (NPCManager.Instance.maleNames.Count > 0) ?
                        NPCManager.Instance.maleNames[Random.Range(0, NPCManager.Instance.maleNames.Count)] :
                        "Unnamed";
                    break;
                case Gender.Female:
                    firstName = (NPCManager.Instance.femaleNames.Count > 0) ?
                        NPCManager.Instance.femaleNames[Random.Range(0, NPCManager.Instance.femaleNames.Count)] :
                        "Unnamed";
                    break;
                default:
                    if (NPCManager.Instance.otherNames.Count > 0)
                        firstName = NPCManager.Instance.otherNames[Random.Range(0, NPCManager.Instance.otherNames.Count)];
                    else
                    {
                        List<string> combinedNames = new List<string>();
                        combinedNames.AddRange(NPCManager.Instance.maleNames);
                        combinedNames.AddRange(NPCManager.Instance.femaleNames);
                        firstName = (combinedNames.Count > 0) ?
                            combinedNames[Random.Range(0, combinedNames.Count)] : "Unnamed";
                    }
                    break;
            }

            // Generate a new last name only if one isn't already set.
            if (string.IsNullOrEmpty(lastName))
            {
                if (NPCManager.Instance.lastNames.Count > 0)
                    lastName = NPCManager.Instance.lastNames[Random.Range(0, NPCManager.Instance.lastNames.Count)];
                else
                    lastName = "NoLastName";
            }
        }
        else
        {
            firstName = "NoFirstName";
            lastName = "NoLastName";
        }

        npcName = firstName + " " + lastName;
    }

    // Family relationship helper methods.
    public void AddParent(NPCIdentity parent)
    {
        if (!parents.Contains(parent))
        {
            parents.Add(parent);
            parent.AddChild(this);
        }
    }

    public void AddChild(NPCIdentity child)
    {
        if (!children.Contains(child))
        {
            children.Add(child);
            child.AddParent(this);
        }
    }

    public void AddSibling(NPCIdentity sibling)
    {
        if (!siblings.Contains(sibling) && sibling != this)
        {
            siblings.Add(sibling);
            if (!sibling.siblings.Contains(this))
                sibling.siblings.Add(this);
        }
    }

    public void SetSpouse(NPCIdentity partner)
    {
        spouse = partner;
        partner.spouse = this;
        // Share the last name between spouses.
        if (string.IsNullOrEmpty(partner.lastName))
        {
            partner.lastName = this.lastName;
            partner.npcName = partner.firstName + " " + partner.lastName;
        }
        else if (string.IsNullOrEmpty(this.lastName))
        {
            this.lastName = partner.lastName;
            this.npcName = firstName + " " + lastName;
        }
        else
        {
            // Optionally enforce one last name; here we choose partner's.
            this.lastName = partner.lastName;
            npcName = firstName + " " + lastName;
        }
    }

    /// <summary>
    /// Recursively collects all ancestors of this NPC.
    /// </summary>
    public HashSet<NPCIdentity> GetAllAncestors()
    {
        HashSet<NPCIdentity> ancestors = new HashSet<NPCIdentity>();
        foreach (var parent in parents)
        {
            if (parent != null)
            {
                ancestors.Add(parent);
                foreach (var ancestor in parent.GetAllAncestors())
                    ancestors.Add(ancestor);
            }
        }
        return ancestors;
    }

    /// <summary>
    /// Determines if this NPC is related to another by checking common ancestry.
    /// </summary>
    public bool IsRelatedTo(NPCIdentity other)
    {
        if (other == null) return false;
        if (other == this) return true;

        HashSet<NPCIdentity> myAncestors = GetAllAncestors();
        HashSet<NPCIdentity> otherAncestors = other.GetAllAncestors();
        if (myAncestors.Contains(other) || otherAncestors.Contains(this))
            return true;

        foreach (var ancestor in myAncestors)
        {
            if (otherAncestors.Contains(ancestor))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether this NPC can date (form a romantic partnership with) another.
    /// Returns false if they are related.
    /// </summary>
    public bool CanDate(NPCIdentity other)
    {
        return !IsRelatedTo(other);
    }
}
