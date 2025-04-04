#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FactionGenerator))]
public class FactionGeneratorEditor : Editor
{
    SerializedProperty totalNPCCountProp;
    SerializedProperty includeFamiliesProp;
    SerializedProperty desiredFamilyCountProp;
    SerializedProperty minFamilySizeProp;
    SerializedProperty maxFamilySizeProp;
    SerializedProperty familyGenerationsProp;

    void OnEnable()
    {
        totalNPCCountProp = serializedObject.FindProperty("totalNPCCount");
        includeFamiliesProp = serializedObject.FindProperty("includeFamilies");
        desiredFamilyCountProp = serializedObject.FindProperty("desiredFamilyCount");
        minFamilySizeProp = serializedObject.FindProperty("minFamilySize");
        maxFamilySizeProp = serializedObject.FindProperty("maxFamilySize");
        familyGenerationsProp = serializedObject.FindProperty("familyGenerations");
    }

    public override void OnInspectorGUI()
    {
        // Draw all properties except the community spawn settings.
        DrawPropertiesExcluding(serializedObject, new string[] { "totalNPCCount", "includeFamilies", "desiredFamilyCount", "minFamilySize", "maxFamilySize", "familyGenerations" });
        EditorGUILayout.Space();
        serializedObject.Update();

        EditorGUILayout.LabelField("Community Spawn Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (totalNPCCountProp != null)
            EditorGUILayout.PropertyField(totalNPCCountProp, new GUIContent("Total NPC Count"));
        if (includeFamiliesProp != null)
            EditorGUILayout.PropertyField(includeFamiliesProp, new GUIContent("Include Families"));

        if (includeFamiliesProp != null && includeFamiliesProp.boolValue)
        {
            EditorGUI.indentLevel++;
            if (desiredFamilyCountProp != null)
                EditorGUILayout.PropertyField(desiredFamilyCountProp, new GUIContent("Desired Family Count"));
            if (minFamilySizeProp != null)
                EditorGUILayout.PropertyField(minFamilySizeProp, new GUIContent("Minimum Family Size"));
            if (maxFamilySizeProp != null)
                EditorGUILayout.PropertyField(maxFamilySizeProp, new GUIContent("Maximum Family Size"));
            if (familyGenerationsProp != null)
                EditorGUILayout.PropertyField(familyGenerationsProp, new GUIContent("Family Generations"));
            EditorGUI.indentLevel--;

            // Calculate feasibility percentage if all necessary properties are available.
            if (desiredFamilyCountProp != null && minFamilySizeProp != null && totalNPCCountProp != null)
            {
                float requiredNPCs = desiredFamilyCountProp.intValue * minFamilySizeProp.intValue;
                float feasibility = (requiredNPCs > 0) ? Mathf.Clamp01((float)totalNPCCountProp.intValue / requiredNPCs) : 0f;
                int feasibilityPercentage = Mathf.RoundToInt(feasibility * 100f);
                EditorGUILayout.HelpBox("Family Feasibility: " + feasibilityPercentage + "%", MessageType.Info);
                if (feasibility < 1f)
                {
                    EditorGUILayout.HelpBox("Warning: The total NPC count may be insufficient for the desired family settings.", MessageType.Warning);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Community"))
        {
            int individuals = (totalNPCCountProp != null) ? totalNPCCountProp.intValue : 0;
            int families = (includeFamiliesProp != null && includeFamiliesProp.boolValue && desiredFamilyCountProp != null) ? desiredFamilyCountProp.intValue : 0;
            int famSize = (includeFamiliesProp != null && includeFamiliesProp.boolValue && minFamilySizeProp != null) ? minFamilySizeProp.intValue : 0;
            int generations = (includeFamiliesProp != null && includeFamiliesProp.boolValue && familyGenerationsProp != null) ? familyGenerationsProp.intValue : 0;
            ((FactionGenerator)target).GenerateCommunity(individuals, families, famSize, generations);
        }
    }
}
#endif
