using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NPCFamilyManager))]
public class NPCFamilyManagerEditor : Editor
{
    private int selectedTab = 0;
    private string[] tabNames = new string[] { "Physical Appearance", "Personality", "Skills", "Traits" };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inheritance Breakdown", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        NPCFamilyManager familyManager = (NPCFamilyManager)target;
        NPC npc = familyManager.GetComponent<NPC>();
        if (npc == null)
        {
            EditorGUILayout.LabelField("NPC component not found on this Family Manager.");
            return;
        }

        // Retrieve parent names.
        string parentAName = "Parent A";
        string parentBName = "Parent B";
        if (familyManager.parents != null && familyManager.parents.Count > 0)
        {
            parentAName = familyManager.parents[0].npcName;
        }
        if (familyManager.parents != null && familyManager.parents.Count > 1)
        {
            parentBName = familyManager.parents[1].npcName;
        }

        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
        EditorGUILayout.Space();

        switch (selectedTab)
        {
            case 0:
                DrawPhysicalAppearanceInheritance(npc, parentAName, parentBName);
                break;
            case 1:
                DrawPersonalityInheritance(npc, parentAName, parentBName);
                break;
            case 2:
                DrawSkillsInheritance(npc, parentAName, parentBName);
                break;
            case 3:
                DrawTraitsInheritance(npc);
                break;
        }
    }

    // TAB 1: Physical Appearance
    void DrawPhysicalAppearanceInheritance(NPC npc, string parentAName, string parentBName)
    {
        NPCPhysicalAppearance childPA = npc.physicalAppearance;
        if (childPA == null)
        {
            EditorGUILayout.LabelField("NPCPhysicalAppearance component not found.");
            return;
        }

        EditorGUILayout.LabelField("Basic Appearance", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        DrawInheritanceRow("Height (m)",
            GetParentHeight(parentAName, npc.GetInstanceID()),
            GetParentHeight(parentBName, npc.GetInstanceID()),
            childPA.characteristics.height,
            npc.GetInstanceID());
        DrawInheritanceRow("Weight (kg)",
            GetParentWeight(parentAName, npc.GetInstanceID()),
            GetParentWeight(parentBName, npc.GetInstanceID()),
            childPA.characteristics.weight,
            npc.GetInstanceID());
        // Color rows with formula
        DrawColorRow("Skin Color",
            GetParentSkinColor(parentAName, npc.GetInstanceID()),
            GetParentSkinColor(parentBName, npc.GetInstanceID()),
            ColorUtility.ToHtmlStringRGB(childPA.characteristics.skinColor),
            npc.GetInstanceID());
        DrawColorRow("Hair Color",
            GetParentHairColor(parentAName, npc.GetInstanceID()),
            GetParentHairColor(parentBName, npc.GetInstanceID()),
            ColorUtility.ToHtmlStringRGB(childPA.characteristics.hairColor),
            npc.GetInstanceID());
        DrawColorRow("Eye Color",
            GetParentEyeColor(parentAName, npc.GetInstanceID()),
            GetParentEyeColor(parentBName, npc.GetInstanceID()),
            ColorUtility.ToHtmlStringRGB(childPA.characteristics.eyeColor),
            npc.GetInstanceID());
        // Body Part Features (4-column table)
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Body Part Features", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Body Part", GUILayout.Width(140));
        EditorGUILayout.LabelField(parentAName + " Feature", GUILayout.Width(140));
        EditorGUILayout.LabelField(parentBName + " Feature", GUILayout.Width(140));
        EditorGUILayout.LabelField("Formula", GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        foreach (var feature in childPA.bodyPartFeatures)
        {
            string bodyPartName = feature.bodyPart.ToString();
            string parentAFeature = GetParentBodyFeature(parentAName, feature.bodyPart);
            string parentBFeature = GetParentBodyFeature(parentBName, feature.bodyPart);
            DrawBodyFeatureRow(bodyPartName, parentAFeature, parentBFeature, npc.GetInstanceID());
        }
    }

    // TAB 2: Personality Inheritance
    void DrawPersonalityInheritance(NPC npc, string parentAName, string parentBName)
    {
        Personality childPersonality = npc.personality;
        if (childPersonality == null)
        {
            EditorGUILayout.LabelField("Personality component not found.");
            return;
        }
        EditorGUILayout.LabelField("Emotional Disposition", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Emotion", GUILayout.Width(140));
        EditorGUILayout.LabelField(parentAName + " (%)", GUILayout.Width(160));
        EditorGUILayout.LabelField(parentBName + " (%)", GUILayout.Width(160));
        EditorGUILayout.LabelField("Mutation (%)", GUILayout.Width(110));
        EditorGUILayout.LabelField("Child Value", GUILayout.Width(110));
        EditorGUILayout.LabelField("Formula", GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawInheritanceRow("Happiness",
            GetParentEmotion(parentAName, "Happiness", npc.GetInstanceID()),
            GetParentEmotion(parentBName, "Happiness", npc.GetInstanceID()),
            childPersonality.defaultHappiness,
            npc.GetInstanceID());
        DrawInheritanceRow("Passion",
            GetParentEmotion(parentAName, "Passion", npc.GetInstanceID()),
            GetParentEmotion(parentBName, "Passion", npc.GetInstanceID()),
            childPersonality.defaultPassion,
            npc.GetInstanceID());
        DrawInheritanceRow("Confidence",
            GetParentEmotion(parentAName, "Confidence", npc.GetInstanceID()),
            GetParentEmotion(parentBName, "Confidence", npc.GetInstanceID()),
            childPersonality.defaultConfidence,
            npc.GetInstanceID());

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Ideology Inheritance", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attribute", GUILayout.Width(140));
        EditorGUILayout.LabelField(parentAName + " (%)", GUILayout.Width(160));
        EditorGUILayout.LabelField(parentBName + " (%)", GUILayout.Width(160));
        EditorGUILayout.LabelField("Mutation (%)", GUILayout.Width(110));
        EditorGUILayout.LabelField("Child Value", GUILayout.Width(110));
        EditorGUILayout.LabelField("Formula", GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        DrawInheritanceRow("Freedom",
            GetParentIdeology(parentAName, "Freedom", npc.GetInstanceID()),
            GetParentIdeology(parentBName, "Freedom", npc.GetInstanceID()),
            childPersonality.ideology.freedom,
            npc.GetInstanceID());
        DrawInheritanceRow("Privacy",
            GetParentIdeology(parentAName, "Privacy", npc.GetInstanceID()),
            GetParentIdeology(parentBName, "Privacy", npc.GetInstanceID()),
            childPersonality.ideology.privacy,
            npc.GetInstanceID());
        DrawInheritanceRow("Authority",
            GetParentIdeology(parentAName, "Authority", npc.GetInstanceID()),
            GetParentIdeology(parentBName, "Authority", npc.GetInstanceID()),
            childPersonality.ideology.authority,
            npc.GetInstanceID());
        DrawInheritanceRow("Equality",
            GetParentIdeology(parentAName, "Equality", npc.GetInstanceID()),
            GetParentIdeology(parentBName, "Equality", npc.GetInstanceID()),
            childPersonality.ideology.equality,
            npc.GetInstanceID());

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Opinion Inheritance", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        if (childPersonality.conceptOpinions != null && childPersonality.conceptOpinions.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Opinion", GUILayout.Width(140));
            EditorGUILayout.LabelField(parentAName + " (%)", GUILayout.Width(160));
            EditorGUILayout.LabelField(parentBName + " (%)", GUILayout.Width(160));
            EditorGUILayout.LabelField("Mutation (%)", GUILayout.Width(110));
            EditorGUILayout.LabelField("Child Intensity", GUILayout.Width(110));
            EditorGUILayout.LabelField("Formula", GUILayout.Width(220));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            foreach (var opinion in childPersonality.conceptOpinions)
            {
                DrawInheritanceRow(opinion.conceptName,
                    GetParentOpinion(parentAName, opinion.conceptName, npc.GetInstanceID()),
                    GetParentOpinion(parentBName, opinion.conceptName, npc.GetInstanceID()),
                    opinion.intensity,
                    npc.GetInstanceID());
            }
        }
    }

    // TAB 3: Skills Inheritance
    void DrawSkillsInheritance(NPC npc, string parentAName, string parentBName)
    {
        EditorGUILayout.LabelField("Perception Inheritance", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        DrawInheritanceRow("Sight Multiplier",
            GetParentSightMultiplier(parentAName, npc.GetInstanceID()),
            GetParentSightMultiplier(parentBName, npc.GetInstanceID()),
            npc.npcGenetics != null ? npc.npcGenetics.sightMultiplier : 1.0f,
            npc.GetInstanceID());
        DrawInheritanceRow("Hearing Multiplier",
            GetParentHearingMultiplier(parentAName, npc.GetInstanceID()),
            GetParentHearingMultiplier(parentBName, npc.GetInstanceID()),
            npc.npcGenetics != null ? npc.npcGenetics.hearingMultiplier : 1.0f,
            npc.GetInstanceID());
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Skill Values", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Skill", GUILayout.Width(140));
        EditorGUILayout.LabelField(parentAName + " (%)", GUILayout.Width(160));
        EditorGUILayout.LabelField(parentBName + " (%)", GUILayout.Width(160));
        EditorGUILayout.LabelField("Mutation (%)", GUILayout.Width(110));
        EditorGUILayout.LabelField("Child Value", GUILayout.Width(110));
        EditorGUILayout.LabelField("Formula", GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        NPCSkills parentASkills = GetParentSkills(npc, 0);
        NPCSkills parentBSkills = GetParentSkills(npc, 1);

        foreach (Skill childSkill in npc.npcSkills.skills)
        {
            float? parentAValue = parentASkills != null ? GetSkillValue(parentASkills, childSkill.skillName) : (float?)null;
            float? parentBValue = parentBSkills != null ? GetSkillValue(parentBSkills, childSkill.skillName) : (float?)null;
            DrawInheritanceRow(childSkill.skillName, parentAValue, parentBValue, childSkill.level, npc.GetInstanceID());
        }
    }

    // TAB 4: Traits Inheritance
    void DrawTraitsInheritance(NPC npc)
    {
        EditorGUILayout.LabelField("Traits", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        if (npc.traits == null || npc.traits.Count == 0)
        {
            EditorGUILayout.LabelField("No traits assigned.");
            return;
        }
        foreach (Trait t in npc.traits)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Trait: " + t.traitName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Category: " + t.category);
            EditorGUILayout.LabelField("Description: " + t.description);
            if (t.modifiers != null && t.modifiers.Count > 0)
            {
                EditorGUILayout.LabelField("Modifiers:");
                foreach (var kvp in t.modifiers)
                {
                    EditorGUILayout.LabelField("   " + kvp.Key + ": " + kvp.Value.ToString("F2"));
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }

    // Updated DrawInheritanceRow (5 columns).
    private void DrawInheritanceRow(string traitName, float? parentAValue, float? parentBValue, float childValue, int seedBase)
    {
        var percentages = GenerateStableRandomPercentages(traitName, seedBase);
        float pAInfluence = percentages.Item1;
        float pBInfluence = percentages.Item2;
        float mutationPerc = percentages.Item3;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(traitName, GUILayout.Width(140));
        string parentAString = parentAValue.HasValue ? parentAValue.Value.ToString("F2") + " (" + pAInfluence.ToString("F0") + "%)"
                                                      : GetStableRandomFloat("default_" + traitName + "_A", 0.4f, 0.8f, seedBase).ToString("F2") + " (" + pAInfluence.ToString("F0") + "%)";
        string parentBString = parentBValue.HasValue ? parentBValue.Value.ToString("F2") + " (" + pBInfluence.ToString("F0") + "%)"
                                                      : GetStableRandomFloat("default_" + traitName + "_B", 0.4f, 0.8f, seedBase).ToString("F2") + " (" + pBInfluence.ToString("F0") + "%)";
        EditorGUILayout.LabelField(parentAString, GUILayout.Width(160));
        EditorGUILayout.LabelField(parentBString, GUILayout.Width(160));
        EditorGUILayout.LabelField(mutationPerc.ToString("F0") + "%", GUILayout.Width(110));
        EditorGUILayout.LabelField(childValue.ToString("F2"), GUILayout.Width(110));

        string formula = "";
        if (parentAValue.HasValue && parentBValue.HasValue)
        {
            formula = "= ((" + parentAValue.Value.ToString("F2") + "*" + pAInfluence.ToString("F0") +
                      "% + " + parentBValue.Value.ToString("F2") + "*" + pBInfluence.ToString("F0") +
                      "%) * (1-" + mutationPerc.ToString("F0") + "%) + (Random)*" + mutationPerc.ToString("F0") + "%)";
        }
        else
        {
            formula = "Randomized/Incomplete inheritance";
        }
        EditorGUILayout.LabelField(formula, GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
    }

    // Updated DrawBodyFeatureRow (4 columns).
    private void DrawBodyFeatureRow(string bodyPartName, string parentAFeature, string parentBFeature, int seedBase)
    {
        var percentages = GenerateStableRandomPercentages(bodyPartName, seedBase);
        float pAInfluence = percentages.Item1;
        float pBInfluence = percentages.Item2;
        float mutationPerc = percentages.Item3;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(bodyPartName, GUILayout.Width(140));
        string paStr = parentAFeature + " (" + pAInfluence.ToString("F0") + "%)";
        string pbStr = parentBFeature + " (" + pBInfluence.ToString("F0") + "%)";
        EditorGUILayout.LabelField(paStr, GUILayout.Width(140));
        EditorGUILayout.LabelField(pbStr, GUILayout.Width(140));
        string formula = "Mut: " + mutationPerc.ToString("F0") + "%, = ((" +
                         parentAFeature + " * " + pAInfluence.ToString("F0") + "% + " +
                         parentBFeature + " * " + pBInfluence.ToString("F0") + "%) * (1-" +
                         mutationPerc.ToString("F0") + "%) + (Random)*" +
                         mutationPerc.ToString("F0") + "%)";
        EditorGUILayout.LabelField(formula, GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
    }

    // Updated DrawColorRow (5 columns for colors, with formula).
    private void DrawColorRow(string label, string parentAColor, string parentBColor, string childColor, int seedBase)
    {
        var percentages = GenerateStableRandomPercentages(label, seedBase);
        float pAInfluence = percentages.Item1;
        float pBInfluence = percentages.Item2;
        // For colors, we use a simple Lerp formula. We assume ParentB's percentage defines the blend factor.
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(140));
        EditorGUILayout.LabelField(parentAColor, GUILayout.Width(160));
        EditorGUILayout.LabelField(parentBColor, GUILayout.Width(160));
        EditorGUILayout.LabelField(childColor, GUILayout.Width(110));
        string formula = "Child = Lerp(" + parentAColor + ", " + parentBColor + ", " + pBInfluence.ToString("F0") + "%)";
        EditorGUILayout.LabelField(formula, GUILayout.Width(220));
        EditorGUILayout.EndHorizontal();
    }

    // Dummy method to simulate stable random percentages.
    private (float, float, float) GenerateStableRandomPercentages(string traitName, int seedBase)
    {
        int seed = traitName.GetHashCode() ^ seedBase;
        System.Random rand = new System.Random(seed);
        double r1 = rand.NextDouble();
        double r2 = rand.NextDouble();
        double sum = r1 + r2;
        if (sum > 1.0)
        {
            r1 /= sum;
            r2 /= sum;
        }
        double mutation = 1.0 - r1 - r2;
        return ((float)(r1 * 100.0), (float)(r2 * 100.0), (float)(mutation * 100.0));
    }

    // Methods to get parent attributes for basic appearance with stable randomization if no parent data.
    private float GetParentHeight(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("Height" + parentName, 1.5f, 2.0f, seedBase);
        return 1.75f;
    }

    private float GetParentWeight(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("Weight" + parentName, 50f, 100f, seedBase);
        return 70f;
    }

    private string GetParentSkinColor(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomColor("SkinColor" + parentName, 0.05f, 0.12f, 0.3f, 0.5f, 0.85f, 0.95f, seedBase);
        return "FFFFFF";
    }

    private string GetParentHairColor(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomColor("HairColor" + parentName, 0.0f, 0.1f, 0.6f, 0.8f, 0.2f, 0.4f, seedBase);
        return "000000";
    }

    private string GetParentEyeColor(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomColor("EyeColor" + parentName, 0.55f, 0.65f, 0.3f, 0.5f, 0.4f, 0.8f, seedBase);
        return "0000FF";
    }

    // Helper method to generate a stable random color as a hex string.
    private string GetStableRandomColor(string context, float hMin, float hMax, float sMin, float sMax, float vMin, float vMax, int seedBase)
    {
        int seed = context.GetHashCode() ^ seedBase;
        System.Random rand = new System.Random(seed);
        float h = Mathf.Lerp(hMin, hMax, (float)rand.NextDouble());
        float s = Mathf.Lerp(sMin, sMax, (float)rand.NextDouble());
        float v = Mathf.Lerp(vMin, vMax, (float)rand.NextDouble());
        Color col = Color.HSVToRGB(h, s, v);
        return ColorUtility.ToHtmlStringRGB(col);
    }

    // Dummy method to simulate retrieval of parent's body part feature.
    private string GetParentBodyFeature(string parentName, BodyPart part)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return "Normal";
        return "Normal";
    }

    // Methods to simulate retrieval of parent skills.
    private NPCSkills GetParentSkills(NPC npc, int index)
    {
        NPCFamilyManager fm = npc.familyManager;
        if (fm != null && fm.parents != null && fm.parents.Count > index)
        {
            NPC parent = fm.parents[index].GetComponent<NPC>();
            if (parent != null)
                return parent.npcSkills;
        }
        return null;
    }
    private float? GetSkillValue(NPCSkills skills, string skillName)
    {
        Skill s = skills.skills.Find(x => x.skillName.Equals(skillName));
        return s != null ? (float?)s.level : null;
    }

    // Helper method to simulate retrieval of parent's emotion values.
    private float? GetParentEmotion(string parentName, string emotion, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("Emotion_" + emotion + parentName, 0.4f, 0.8f, seedBase);
        if (emotion == "Happiness")
            return 0.6f;
        else if (emotion == "Passion")
            return 0.5f;
        else if (emotion == "Confidence")
            return 0.7f;
        return null;
    }

    // Helper method to simulate retrieval of parent's ideology values.
    private float? GetParentIdeology(string parentName, string attribute, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("Ideology_" + attribute + parentName, 0.3f, 0.7f, seedBase);
        if (attribute == "Freedom")
            return 0.55f;
        else if (attribute == "Privacy")
            return 0.50f;
        else if (attribute == "Authority")
            return 0.45f;
        else if (attribute == "Equality")
            return 0.60f;
        return null;
    }

    // Helper method to simulate retrieval of parent's opinion intensity.
    private float? GetParentOpinion(string parentName, string opinionName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("Opinion_" + opinionName + parentName, 0.3f, 0.7f, seedBase);
        return 0.5f;
    }

    // Dummy methods for perception inheritance.
    private float? GetParentSightMultiplier(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("SightMultiplier" + parentName, 0.8f, 1.2f, seedBase);
        return 1.0f;
    }
    private float? GetParentHearingMultiplier(string parentName, int seedBase)
    {
        if (parentName == "Parent A" || parentName == "Parent B")
            return GetStableRandomFloat("HearingMultiplier" + parentName, 0.8f, 1.2f, seedBase);
        return 1.0f;
    }

    // Helper method to generate a stable random float.
    private float GetStableRandomFloat(string context, float min, float max, int seedBase)
    {
        int seed = context.GetHashCode() ^ seedBase;
        System.Random rand = new System.Random(seed);
        double value = rand.NextDouble();
        return Mathf.Lerp(min, max, (float)value);
    }
}
