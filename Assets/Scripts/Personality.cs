using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Ideology
{
    [Range(0f, 1f)]
    public float freedom;
    [Range(0f, 1f)]
    public float privacy;
    [Range(0f, 1f)]
    public float authority;
    [Range(0f, 1f)]
    public float equality;

    public float GetSimilarity(Ideology other)
    {
        float freedomSim = 1f - Mathf.Abs(freedom - other.freedom);
        float privacySim = 1f - Mathf.Abs(privacy - other.privacy);
        float authoritySim = 1f - Mathf.Abs(authority - other.authority);
        float equalitySim = 1f - Mathf.Abs(equality - other.equality);
        return (freedomSim + privacySim + authoritySim + equalitySim) / 4f;
    }
}

public class Personality : MonoBehaviour
{
    [Header("Default Emotion Values")]
    [Range(0f, 1f)]
    public float defaultHappiness = 0.5f;
    [Range(0f, 1f)]
    public float defaultPassion = 0.5f;
    [Range(0f, 1f)]
    public float defaultConfidence = 0.5f;

    [Header("Personality Opinions")]
    public List<ConceptOpinion> conceptOpinions = new List<ConceptOpinion>();
    public List<TangibleOpinion> tangibleInterests = new List<TangibleOpinion>();
    public List<TangibleOpinion> tangibleFears = new List<TangibleOpinion>();

    [Header("Ideological Profile")]
    public Ideology ideology = new Ideology { freedom = 0.5f, privacy = 0.5f, authority = 0.5f, equality = 0.5f };

    [Header("Potential Opinions Database")]
    public PotentialOpinionsDatabase opinionsDatabase;

    // Inherit personality from a list of parent personalities.
    public void InheritFromParents(List<Personality> parentPersonalities)
    {
        if (parentPersonalities == null || parentPersonalities.Count == 0)
            return;

        int count = parentPersonalities.Count;
        float sumHappiness = 0f, sumPassion = 0f, sumConfidence = 0f;
        Ideology combinedIdeology = new Ideology { freedom = 0f, privacy = 0f, authority = 0f, equality = 0f };

        foreach (var parent in parentPersonalities)
        {
            sumHappiness += parent.defaultHappiness;
            sumPassion += parent.defaultPassion;
            sumConfidence += parent.defaultConfidence;
            combinedIdeology.freedom += parent.ideology.freedom;
            combinedIdeology.privacy += parent.ideology.privacy;
            combinedIdeology.authority += parent.ideology.authority;
            combinedIdeology.equality += parent.ideology.equality;

            foreach (var opinion in parent.conceptOpinions)
            {
                var existing = conceptOpinions.Find(o => o.conceptName.Equals(opinion.conceptName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    existing.intensity = (existing.intensity + opinion.intensity) / 2f;
                    existing.moralJudgement = (existing.moralJudgement + opinion.moralJudgement) / 2f;
                }
                else
                {
                    conceptOpinions.Add(new ConceptOpinion
                    {
                        conceptName = opinion.conceptName,
                        category = opinion.category,
                        intensity = opinion.intensity,
                        moralJudgement = opinion.moralJudgement,
                        description = opinion.description,
                        requiredIdeology = opinion.requiredIdeology
                    });
                }
            }
        }

        defaultHappiness = Mathf.Clamp01(sumHappiness / count + UnityEngine.Random.Range(-0.05f, 0.05f));
        defaultPassion = Mathf.Clamp01(sumPassion / count + UnityEngine.Random.Range(-0.05f, 0.05f));
        defaultConfidence = Mathf.Clamp01(sumConfidence / count + UnityEngine.Random.Range(-0.05f, 0.05f));

        ideology.freedom = Mathf.Clamp01(combinedIdeology.freedom / count);
        ideology.privacy = Mathf.Clamp01(combinedIdeology.privacy / count);
        ideology.authority = Mathf.Clamp01(combinedIdeology.authority / count);
        ideology.equality = Mathf.Clamp01(combinedIdeology.equality / count);
    }

    // Returns a dynamic modifier for an action based on personality traits.
    // Placeholder logic: return a modifier value based on the action name.
    public float GetDynamicModifierForAction(string actionName)
    {
        // Example logic: extroverted traits might boost social interactions.
        switch (actionName)
        {
            case "InteractWithNPC":
                return 0.2f;
            case "Explore":
                return 0.1f;
            case "Work":
                return 0.1f;
            default:
                return 0f;
        }
    }
}
