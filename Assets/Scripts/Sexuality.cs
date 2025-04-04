using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BodyPartAttractionPreference
{
    public BodyPart bodyPart;
    public StrengthClassification preferredStrength;
    public WidthClassification preferredWidth;
    public DexterityClassification preferredDexterity;
    public AestheticClassification preferredAesthetic;
}

public class Sexuality : MonoBehaviour
{
    [Header("Attraction Preferences")]
    public List<BodyPartAttractionPreference> attractionPreferences = new List<BodyPartAttractionPreference>();

    /// <summary>
    /// Evaluates the attraction score toward a target based on its Physical Appearance.
    /// </summary>
    public float EvaluateAttraction(NPCPhysicalAppearance targetAppearance)
    {
        float totalScore = 0f;
        int criteriaCount = 0;
        foreach (var preference in attractionPreferences)
        {
            BodyPartFeature targetFeature = targetAppearance.bodyPartFeatures.Find(f => f.bodyPart == preference.bodyPart);
            if (targetFeature != null)
            {
                float score = 0f;
                if (targetFeature.strength == preference.preferredStrength) { score += 1f; }
                if (targetFeature.width == preference.preferredWidth) { score += 1f; }
                if (targetFeature.dexterity == preference.preferredDexterity) { score += 1f; }
                if (targetFeature.aesthetic == preference.preferredAesthetic) { score += 1f; }
                totalScore += score / 4f;
                criteriaCount++;
            }
        }
        return (criteriaCount > 0) ? totalScore / criteriaCount : 0f;
    }

    /// <summary>
    /// Initializes sexuality preferences if none have been defined.
    /// </summary>
    public void InitializeSexuality()
    {
        if (attractionPreferences == null || attractionPreferences.Count == 0)
        {
            RandomizeAttractionPreferences();
        }
    }

    /// <summary>
    /// Randomizes the attraction preferences by selecting a random subset of body parts
    /// and assigning each a set of random preferred attributes.
    /// </summary>
    public void RandomizeAttractionPreferences()
    {
        attractionPreferences = new List<BodyPartAttractionPreference>();

        // Get all possible body parts.
        System.Array bodyParts = System.Enum.GetValues(typeof(BodyPart));
        int totalBodyParts = bodyParts.Length;

        // Decide how many preferences to generate (e.g., between 1 and 3).
        int numPreferences = Random.Range(1, Mathf.Min(3, totalBodyParts) + 1);

        // Create a list of available body parts to avoid duplicates.
        List<BodyPart> availableBodyParts = new List<BodyPart>();
        foreach (var bp in bodyParts)
        {
            availableBodyParts.Add((BodyPart)bp);
        }

        for (int i = 0; i < numPreferences; i++)
        {
            int index = Random.Range(0, availableBodyParts.Count);
            BodyPart selectedBodyPart = availableBodyParts[index];
            availableBodyParts.RemoveAt(index); // Prevent duplicate selections

            BodyPartAttractionPreference pref = new BodyPartAttractionPreference();
            pref.bodyPart = selectedBodyPart;

            // Randomize the preferred attributes.
            System.Array strengthValues = System.Enum.GetValues(typeof(StrengthClassification));
            pref.preferredStrength = (StrengthClassification)strengthValues.GetValue(Random.Range(0, strengthValues.Length));

            System.Array widthValues = System.Enum.GetValues(typeof(WidthClassification));
            pref.preferredWidth = (WidthClassification)widthValues.GetValue(Random.Range(0, widthValues.Length));

            System.Array dexterityValues = System.Enum.GetValues(typeof(DexterityClassification));
            pref.preferredDexterity = (DexterityClassification)dexterityValues.GetValue(Random.Range(0, dexterityValues.Length));

            System.Array aestheticValues = System.Enum.GetValues(typeof(AestheticClassification));
            pref.preferredAesthetic = (AestheticClassification)aestheticValues.GetValue(Random.Range(0, aestheticValues.Length));

            attractionPreferences.Add(pref);
        }
    }
}
