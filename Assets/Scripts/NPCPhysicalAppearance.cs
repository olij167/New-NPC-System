using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PhysicalCharacteristics
{
    [Header("Basic Appearance")]
    public Color skinColor = Color.white;
    public Color hairColor = Color.black;
    public Color eyeColor = Color.blue;

    [Header("Measurements")]
    public float height = 1.75f; // in meters
    public float weight = 70f;   // in kilograms

    [Header("Notable Features")]
    public List<string> notableFeatures = new List<string>(); // e.g., "Freckles", "Scar", "Tattoo"

    /// <summary>
    /// Inherit physical characteristics from two parents.
    /// Uses a simple average with slight random variation.
    /// </summary>
    public static PhysicalCharacteristics Inherit(PhysicalCharacteristics parentA, PhysicalCharacteristics parentB, float mutationChance = 0.05f)
    {
        PhysicalCharacteristics child = new PhysicalCharacteristics();
        child.skinColor = Color.Lerp(parentA.skinColor, parentB.skinColor, 0.5f);
        child.hairColor = Color.Lerp(parentA.hairColor, parentB.hairColor, 0.5f);
        child.eyeColor = Color.Lerp(parentA.eyeColor, parentB.eyeColor, 0.5f);
        child.height = (parentA.height + parentB.height) / 2f + Random.Range(-0.05f, 0.05f);
        child.weight = (parentA.weight + parentB.weight) / 2f + Random.Range(-2f, 2f);

        // Inherit common notable features.
        child.notableFeatures = new List<string>();
        foreach (string feature in parentA.notableFeatures)
        {
            if (parentB.notableFeatures.Contains(feature))
            {
                child.notableFeatures.Add(feature);
            }
        }
        // Optionally introduce a mutation: chance to add a new, random feature.
        if (Random.value < mutationChance)
        {
            child.notableFeatures.Add("Mutation Feature");
        }
        return child;
    }
}

public enum VisualPresentation { Masculine, Feminine }

public class NPCPhysicalAppearance : MonoBehaviour
{
    public PhysicalCharacteristics characteristics = new PhysicalCharacteristics();

    [Header("Detailed Body Part Features")]
    public List<BodyPartFeature> bodyPartFeatures = new List<BodyPartFeature>();

    [Header("Sexual Presentation")]
    [Range(0f, 1f)]
    public float masculinity; // 0 means very feminine, 1 means very masculine.
    public VisualPresentation visualPresentation;

    private void OnValidate()
    {
        // Ensure an element exists for every defined body part.
        foreach (BodyPart part in System.Enum.GetValues(typeof(BodyPart)))
        {
            bool exists = bodyPartFeatures.Exists(feature => feature.bodyPart == part);
            if (!exists)
            {
                BodyPartFeature newFeature = new BodyPartFeature { bodyPart = part };
                bodyPartFeatures.Add(newFeature);
            }
        }
    }

    public void RandomizeAppearance()
    {
        characteristics.skinColor = RandomSkinColor();
        characteristics.hairColor = RandomHairColor();
        characteristics.eyeColor = RandomEyeColor();
        characteristics.height = Random.Range(1.5f, 2.0f);
        characteristics.weight = Random.Range(50f, 100f);
        masculinity = Random.Range(0f, 1f);
        visualPresentation = (masculinity >= 0.5f) ? VisualPresentation.Masculine : VisualPresentation.Feminine;
    }

    /// <summary>
    /// Returns a semi-realistic, stylized skin color.
    /// Presets include:
    /// 1. Light skin: Hue 0.05-0.12, Sat 0.3-0.5, Val 0.85-0.95.
    /// 2. Darker skin: Hue 0.04-0.10, Sat 0.4-0.6, Val 0.5-0.7.
    /// 3. Stylized variant: Purple-ish skin: Hue 0.75-0.78, Sat 0.15-0.3, Val 0.7-0.85.
    /// 4. Stylized variant: Yellow-ish skin: Hue 0.12-0.15, Sat 0.15-0.3, Val 0.8-0.9.
    /// </summary>
    private Color RandomSkinColor()
    {
        int preset = Random.Range(0, 4);
        float hue = 0f, sat = 0f, val = 0f;
        switch (preset)
        {
            case 0: // Light skin
                hue = Random.Range(0.05f, 0.12f);
                sat = Random.Range(0.3f, 0.5f);
                val = Random.Range(0.85f, 0.95f);
                break;
            case 1: // Darker skin
                hue = Random.Range(0.04f, 0.10f);
                sat = Random.Range(0.4f, 0.6f);
                val = Random.Range(0.5f, 0.7f);
                break;
            case 2: // Stylized Purple-ish skin
                hue = Random.Range(0.75f, 0.78f);
                sat = Random.Range(0.15f, 0.3f);
                val = Random.Range(0.7f, 0.85f);
                break;
            case 3: // Stylized Yellow-ish skin
                hue = Random.Range(0.12f, 0.15f);
                sat = Random.Range(0.15f, 0.3f);
                val = Random.Range(0.8f, 0.9f);
                break;
        }
        return Color.HSVToRGB(hue, sat, val);
    }

    /// <summary>
    /// Returns a semi-realistic, stylized hair color.
    /// Presets include:
    /// 1. Dark/Brown: Hue 0.0-0.15, Sat 0.6-0.8, Val 0.15-0.35.
    /// 2. Blonde: Hue 0.1-0.15, Sat 0.2-0.4, Val 0.7-0.9.
    /// 3. Red: Hue 0.95-1.0 (or 0-0.05), Sat 0.3-0.5, Val 0.4-0.6.
    /// 4. Stylized Purple: Hue 0.68-0.72, Sat 0.2-0.4, Val 0.3-0.5.
    /// 5. Stylized Blue: Hue 0.55-0.65, Sat 0.2-0.4, Val 0.3-0.5.
    /// </summary>
    private Color RandomHairColor()
    {
        int preset = Random.Range(0, 5);
        float hue = 0f, sat = 0f, val = 0f;
        switch (preset)
        {
            case 0: // Dark/Brown hair.
                hue = Random.Range(0.0f, 0.15f);
                sat = Random.Range(0.6f, 0.8f);
                val = Random.Range(0.15f, 0.35f);
                break;
            case 1: // Blonde hair.
                hue = Random.Range(0.1f, 0.15f);
                sat = Random.Range(0.2f, 0.4f);
                val = Random.Range(0.7f, 0.9f);
                break;
            case 2: // Red hair.
                // We can choose red either near 0 or near 1.
                hue = (Random.value < 0.5f) ? Random.Range(0f, 0.05f) : Random.Range(0.95f, 1f);
                sat = Random.Range(0.3f, 0.5f);
                val = Random.Range(0.4f, 0.6f);
                break;
            case 3: // Stylized Purple hair.
                hue = Random.Range(0.68f, 0.72f);
                sat = Random.Range(0.2f, 0.4f);
                val = Random.Range(0.3f, 0.5f);
                break;
            case 4: // Stylized Blue hair.
                hue = Random.Range(0.55f, 0.65f);
                sat = Random.Range(0.2f, 0.4f);
                val = Random.Range(0.3f, 0.5f);
                break;
        }
        return Color.HSVToRGB(hue, sat, val);
    }

    /// <summary>
    /// Returns a semi-realistic, stylized eye color.
    /// Presets include:
    /// 1. Blue: Hue 0.55-0.65, Sat 0.3-0.5, Val 0.4-0.8.
    /// 2. Green: Hue 0.25-0.40, Sat 0.3-0.5, Val 0.4-0.8.
    /// 3. Brown: Hue 0.05-0.10, Sat 0.7-0.9, Val 0.3-0.6.
    /// 4. Stylized Grey: Hue can be arbitrary, Sat 0.0-0.2, Val 0.4-0.7.
    /// 5. Stylized Purple: Hue 0.68-0.72, Sat 0.2-0.4, Val 0.4-0.8.
    /// </summary>
    private Color RandomEyeColor()
    {
        int preset = Random.Range(0, 5);
        float hue = 0f, sat = 0f, val = 0f;
        switch (preset)
        {
            case 0: // Blue eyes.
                hue = Random.Range(0.55f, 0.65f);
                sat = Random.Range(0.3f, 0.5f);
                val = Random.Range(0.4f, 0.8f);
                break;
            case 1: // Green eyes.
                hue = Random.Range(0.25f, 0.40f);
                sat = Random.Range(0.3f, 0.5f);
                val = Random.Range(0.4f, 0.8f);
                break;
            case 2: // Brown eyes.
                hue = Random.Range(0.05f, 0.10f);
                sat = Random.Range(0.7f, 0.9f);
                val = Random.Range(0.3f, 0.6f);
                break;
            case 3: // Stylized Grey eyes.
                hue = Random.Range(0f, 1f); // Hue is less relevant for grey.
                sat = Random.Range(0f, 0.2f);
                val = Random.Range(0.4f, 0.7f);
                break;
            case 4: // Stylized Purple eyes.
                hue = Random.Range(0.68f, 0.72f);
                sat = Random.Range(0.2f, 0.4f);
                val = Random.Range(0.4f, 0.8f);
                break;
        }
        return Color.HSVToRGB(hue, sat, val);
    }

    public void InheritAppearance(NPCPhysicalAppearance parentA, NPCPhysicalAppearance parentB)
    {
        characteristics = PhysicalCharacteristics.Inherit(parentA.characteristics, parentB.characteristics);
        masculinity = (parentA.masculinity + parentB.masculinity) / 2f;
        visualPresentation = (masculinity >= 0.5f) ? VisualPresentation.Masculine : VisualPresentation.Feminine;
        bodyPartFeatures.Clear();
        foreach (BodyPart part in System.Enum.GetValues(typeof(BodyPart)))
        {
            BodyPartFeature featureA = parentA.bodyPartFeatures.Find(f => f.bodyPart == part);
            BodyPartFeature featureB = parentB.bodyPartFeatures.Find(f => f.bodyPart == part);
            if (featureA != null && featureB != null)
            {
                bodyPartFeatures.Add(BodyPartFeature.Inherit(featureA, featureB));
            }
            else
            {
                bodyPartFeatures.Add(new BodyPartFeature { bodyPart = part });
            }
        }
    }

    public void ApplySexualPresentation()
    {
        if (visualPresentation == VisualPresentation.Masculine)
        {
            characteristics.height = Mathf.Max(characteristics.height, 1.75f);
            characteristics.weight = Mathf.Max(characteristics.weight, 70f);
            SetFeatureForPart(BodyPart.Torso, StrengthClassification.Strong, WidthClassification.Wide);
            SetFeatureForPart(BodyPart.Arms, StrengthClassification.Strong, WidthClassification.Wide);
            SetFeatureForPart(BodyPart.Shoulders, StrengthClassification.Strong, WidthClassification.Wide);
        }
        else if (visualPresentation == VisualPresentation.Feminine)
        {
            characteristics.height = Mathf.Min(characteristics.height, 1.65f);
            characteristics.weight = Mathf.Min(characteristics.weight, 60f);
            SetFeatureForPart(BodyPart.Face, AestheticClassification.VeryAttractive);
            SetFeatureForPart(BodyPart.Torso, StrengthClassification.Normal, WidthClassification.Narrow);
            SetFeatureForPart(BodyPart.Arms, StrengthClassification.Normal, WidthClassification.Normal);
        }
    }

    private void SetFeatureForPart(BodyPart part, StrengthClassification strength, WidthClassification width)
    {
        BodyPartFeature feature = bodyPartFeatures.Find(f => f.bodyPart == part);
        if (feature != null)
        {
            feature.strength = strength;
            feature.width = width;
        }
    }

    private void SetFeatureForPart(BodyPart part, AestheticClassification aesthetic)
    {
        BodyPartFeature feature = bodyPartFeatures.Find(f => f.bodyPart == part);
        if (feature != null)
        {
            feature.aesthetic = aesthetic;
        }
    }
}
