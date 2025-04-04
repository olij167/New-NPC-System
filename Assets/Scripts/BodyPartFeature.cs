using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BodyPartFeature
{
    public BodyPart bodyPart;
    public SizeClassification size = SizeClassification.Normal;
    public WidthClassification width = WidthClassification.Normal;
    public StrengthClassification strength = StrengthClassification.Normal;
    public DexterityClassification dexterity = DexterityClassification.Normal; // Relevant for hands/arms
    public AestheticClassification aesthetic = AestheticClassification.Average; // Relevant for face/head

    /// <summary>
    /// Inherit features for a specific body part from two parents.
    /// Each attribute is chosen from one parent's value with a chance for mutation.
    /// </summary>
    public static BodyPartFeature Inherit(BodyPartFeature parentA, BodyPartFeature parentB, float mutationChance = 0.05f)
    {
        BodyPartFeature childFeature = new BodyPartFeature();
        childFeature.bodyPart = parentA.bodyPart; // Assumes both parents have data for the same body part.
        childFeature.size = InheritEnum(parentA.size, parentB.size, mutationChance);
        childFeature.width = InheritEnum(parentA.width, parentB.width, mutationChance);
        childFeature.strength = InheritEnum(parentA.strength, parentB.strength, mutationChance);
        childFeature.dexterity = InheritEnum(parentA.dexterity, parentB.dexterity, mutationChance);
        childFeature.aesthetic = InheritEnum(parentA.aesthetic, parentB.aesthetic, mutationChance);
        return childFeature;
    }

    /// <summary>
    /// A generic method to choose one of two enum values, with a chance to mutate to an adjacent value.
    /// </summary>
    private static T InheritEnum<T>(T valueA, T valueB, float mutationChance) where T : System.Enum
    {
        T chosen = Random.value < 0.5f ? valueA : valueB;
        if (Random.value < mutationChance)
        {
            // Convert enum to int for mutation.
            int currentValue = System.Convert.ToInt32(chosen);
            T[] values = (T[])System.Enum.GetValues(typeof(T));
            int maxIndex = values.Length - 1;
            // Choose randomly to shift up or down.
            int shift = Random.value < 0.5f ? -1 : 1;
            int newValue = Mathf.Clamp(currentValue + shift, 0, maxIndex);
            chosen = values[newValue];
        }
        return chosen;
    }
}

public enum BodyPart
{
    Head,
    Face,
    Ears,
    Eyes,
    Nose,
    Mouth,
    Neck,
    Shoulders,
    Arms,
    Hands,
    Torso,
    Waist,
    Hips,
    Legs,
    Feet
}


public enum SizeClassification
{
    VerySmall,
    Small,
    Normal,
    Large,
    VeryLarge
}

public enum WidthClassification
{
    VeryNarrow,
    Narrow,
    Normal,
    Wide,
    VeryWide
}

public enum StrengthClassification
{
    VeryWeak,
    Weak,
    Normal,
    Strong,
    VeryStrong
}

public enum DexterityClassification
{
    VeryClumsy,
    Clumsy,
    Normal,
    Agile,
    VeryAgile
}

public enum AestheticClassification
{
    Unattractive,
    BelowAverage,
    Average,
    Attractive,
    VeryAttractive
}

