using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Skill
{
    public string skillName;
    [Range(0f, 100f)]
    public float level; // Level ranges from 0 to 100

    public Skill(string name, float level)
    {
        this.skillName = name;
        this.level = level;
    }

    /// <summary>
    /// Inherit a skill from two parents by averaging and adding some random variation.
    /// </summary>
    public static Skill Inherit(Skill parentA, Skill parentB)
    {
        float childLevel = (parentA.level + parentB.level) / 2f + Random.Range(-5f, 5f);
        childLevel = Mathf.Clamp(childLevel, 0f, 100f);
        return new Skill(parentA.skillName, childLevel);
    }
}

public class NPCSkills : MonoBehaviour
{
    [Header("NPC Skills")]
    public List<Skill> skills = new List<Skill>();

    /// <summary>
    /// Initializes default skills if none exist.
    /// </summary>
    public void InitializeSkills()
    {
        if (skills == null || skills.Count == 0)
        {
            skills = new List<Skill>()
            {
                new Skill("Strength", Random.Range(30f, 70f)),
                new Skill("Charisma", Random.Range(30f, 70f)),
                new Skill("Mathematics", Random.Range(30f, 70f)),
                new Skill("Linguistics", Random.Range(30f, 70f)),
                new Skill("Logic", Random.Range(40f, 80f)),
                new Skill("Athletics", Random.Range(20f, 60f)),
                new Skill("Creativity", Random.Range(30f, 70f)),
                new Skill("Craftsmanship", Random.Range(30f, 70f)) // Corrected spelling.
            };
        }
    }

    void Awake()
    {
        if (skills.Count == 0)
        {
            InitializeSkills();
        }
    }

    /// <summary>
    /// Adds a new skill with a given name and level.
    /// </summary>
    public void AddSkill(string name, float level)
    {
        skills.Add(new Skill(name, level));
    }

    /// <summary>
    /// Retrieves a skill by name.
    /// </summary>
    public Skill GetSkill(string name)
    {
        return skills.Find(s => s.skillName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Inherit skills from two parents.
    /// For each unique skill in the parents' skill sets, blend the levels.
    /// </summary>
    public static List<Skill> InheritSkills(List<Skill> parentASkills, List<Skill> parentBSkills)
    {
        List<Skill> childSkills = new List<Skill>();
        HashSet<string> skillNames = new HashSet<string>();
        foreach (Skill s in parentASkills)
            skillNames.Add(s.skillName);
        foreach (Skill s in parentBSkills)
            skillNames.Add(s.skillName);

        foreach (string name in skillNames)
        {
            Skill skillA = parentASkills.Find(s => s.skillName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
            Skill skillB = parentBSkills.Find(s => s.skillName.Equals(name, System.StringComparison.OrdinalIgnoreCase));

            if (skillA != null && skillB != null)
            {
                childSkills.Add(Skill.Inherit(skillA, skillB));
            }
            else if (skillA != null)
            {
                float childLevel = skillA.level + Random.Range(-5f, 5f);
                childLevel = Mathf.Clamp(childLevel, 0f, 100f);
                childSkills.Add(new Skill(name, childLevel));
            }
            else if (skillB != null)
            {
                float childLevel = skillB.level + Random.Range(-5f, 5f);
                childLevel = Mathf.Clamp(childLevel, 0f, 100f);
                childSkills.Add(new Skill(name, childLevel));
            }
        }

        return childSkills;
    }
}
