using UnityEngine;
using System.Collections.Generic;

public enum Gender { Male, Female, Other }

public class NPCIdentity : MonoBehaviour
{
    [Header("Basic Identity")]
    public string firstName;
    public string lastName;
    public string npcName;
    public int age;
    public Gender gender;

    [Header("Age Settings")]
    public int minAge = 18;
    public int maxAge = 70;

    // Family data is managed by NPCFamilyManager.
    [HideInInspector]
    public NPCFamilyManager familyManager;

    void Awake()
    {
        // Ensure a family manager exists.
        familyManager = GetComponent<NPCFamilyManager>();
        if (familyManager == null)
        {
            familyManager = gameObject.AddComponent<NPCFamilyManager>();
        }
    }

    /// <summary>
    /// Generates the NPC's identity. If parents are present in the family manager,
    /// the last name is chosen as either one parent's last name or a hyphenated combination.
    /// Otherwise, random names are selected from the NPCManager's name lists.
    /// </summary>
    public void GenerateIdentity()
    {
        if (NPCManager.Instance != null)
        {
            // Determine first name based on gender.
            if (gender == Gender.Male)
            {
                if (NPCManager.Instance.maleNames.Count > 0)
                    firstName = NPCManager.Instance.maleNames[Random.Range(0, NPCManager.Instance.maleNames.Count)];
                else
                    firstName = "Male";
            }
            else if (gender == Gender.Female)
            {
                if (NPCManager.Instance.femaleNames.Count > 0)
                    firstName = NPCManager.Instance.femaleNames[Random.Range(0, NPCManager.Instance.femaleNames.Count)];
                else
                    firstName = "Female";
            }
            else
            {
                List<string> combined = new List<string>();
                combined.AddRange(NPCManager.Instance.maleNames);
                combined.AddRange(NPCManager.Instance.femaleNames);
                if (combined.Count > 0)
                    firstName = combined[Random.Range(0, combined.Count)];
                else
                    firstName = "Other";
            }

            // Determine last name.
            if (familyManager != null && familyManager.parents != null && familyManager.parents.Count >= 1)
            {
                if (familyManager.parents.Count == 1)
                {
                    lastName = GetSingleLastName(familyManager.parents[0].lastName);
                }
                else
                {
                    // 50% chance: choose one parent's last name; 50% chance: hyphenate.
                    if (Random.value < 0.5f)
                    {
                        int index = Random.Range(0, familyManager.parents.Count);
                        lastName = GetSingleLastName(familyManager.parents[index].lastName);
                    }
                    else
                    {
                        // For hyphenation, select one part from each parent's last name.
                        string parentALast = GetSingleLastName(familyManager.parents[0].lastName);
                        string parentBLast = GetSingleLastName(familyManager.parents[1].lastName);
                        lastName = parentALast + "-" + parentBLast;
                    }
                }
            }
            else
            {
                if (NPCManager.Instance.lastNames.Count > 0)
                    lastName = NPCManager.Instance.lastNames[Random.Range(0, NPCManager.Instance.lastNames.Count)];
                else
                    lastName = "NoLastName";
            }
        }
        else
        {
            firstName = "Default";
            lastName = "Default";
        }
        npcName = firstName + " " + lastName;
        age = Random.Range(minAge, maxAge + 1);
        gameObject.name = npcName;
    }

    private string GetSingleLastName(string originalLastName)
    {
        if (string.IsNullOrEmpty(originalLastName))
            return "NoLastName";
        if (originalLastName.Contains("-"))
        {
            string[] parts = originalLastName.Split('-');
            int index = Random.Range(0, parts.Length);
            return parts[index];
        }
        return originalLastName;
    }
}
