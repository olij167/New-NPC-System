#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CommunitySpawner))]
public class CommunitySpawnerEditor : Editor
{
    // Foldout states for each section.
    private bool showGeneralSettings = true;
    private bool showFamilySettings = true;
    private bool showBusinessSettings = true;
    private bool showFactionSettings = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CommunitySpawner spawner = (CommunitySpawner)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Community Spawner", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // --- Root Community Control ---
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Root Community Settings", EditorStyles.boldLabel);
        spawner.assignToFaction = EditorGUILayout.Toggle("Assign to Community Faction", spawner.assignToFaction);
        spawner.rootCommunity = (Faction)EditorGUILayout.ObjectField("Root Community", spawner.rootCommunity, typeof(Faction), true);
        if (GUILayout.Button("Spawn New Community Faction"))
        {
            spawner.rootCommunity = spawner.factionGenerator.GenerateCommunityFaction();
            Debug.Log("New Root Community spawned: " + spawner.rootCommunity.factionName);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // --- General NPC Settings ---
        showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "NPC Generation Settings", true);
        if (showGeneralSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnNPCs"), new GUIContent("Spawn NPCs"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("totalNPCCount"), new GUIContent("Total NPC Count"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // --- Family Generation Settings ---
        showFamilySettings = EditorGUILayout.Foldout(showFamilySettings, "Family Generation Settings", true);
        if (showFamilySettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnFamilies"), new GUIContent("Spawn Families"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfFamilies"), new GUIContent("Number of Families"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("familySize"), new GUIContent("Family Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("familyGenerations"), new GUIContent("Family Generations"));
            EditorGUI.indentLevel--;

            // Calculate feasibility percentage.
            SerializedProperty totalNPCCountProp = serializedObject.FindProperty("totalNPCCount");
            SerializedProperty desiredFamilyCountProp = serializedObject.FindProperty("numberOfFamilies");
            SerializedProperty minFamilySizeProp = serializedObject.FindProperty("familySize");
            if (totalNPCCountProp != null && desiredFamilyCountProp != null && minFamilySizeProp != null)
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

        EditorGUILayout.Space();

        // --- Business Generation Settings ---
        showBusinessSettings = EditorGUILayout.Foldout(showBusinessSettings, "Business Generation Settings", true);
        if (showBusinessSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnBusinesses"), new GUIContent("Spawn Businesses"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfBusinesses"), new GUIContent("Number of Businesses"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("specifiedIndustries"), new GUIContent("Specified Industries"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("specifiedBusinessTemplates"), new GUIContent("Specified Business Templates"), true);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // --- Faction Generation Settings ---
        showFactionSettings = EditorGUILayout.Foldout(showFactionSettings, "Faction Generation Settings", true);
        if (showFactionSettings)
        {
            EditorGUI.indentLevel++;
            // The root community field is already shown above.
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // --- Spawn Buttons ---
        EditorGUILayout.LabelField("Spawn Controls", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn NPCs"))
        {
            spawner.SpawnNPCs();
        }
        if (GUILayout.Button("Spawn Families"))
        {
            spawner.SpawnFamilies();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Businesses"))
        {
            spawner.SpawnBusinesses();
        }
        if (GUILayout.Button("Spawn Faction"))
        {
            spawner.SpawnFactions();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Spawn Entire Community"))
        {
            spawner.SpawnCommunity();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
