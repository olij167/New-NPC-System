using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NPCSpawner))]
public class NPCSpawnerEditor : Editor
{
    private int selectedTab = 0;
    private string[] tabs = { "Individual", "Family Tree" };

    // Parameters for individual spawns.
    private int individualCount = 1;

    // Customization options for Family Tree.
    private bool randomFamilySize = false;
    private int fixedFamilySize = 4;
    private int minFamilySize = 3;
    private int maxFamilySize = 6;

    private bool randomGenerations = false;
    private int fixedGenerations = 3;
    private int minGenerations = 2;
    private int maxGenerations = 4;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NPCSpawner spawner = (NPCSpawner)target;

        DrawDefaultInspector();
        EditorGUILayout.Space();

        selectedTab = GUILayout.Toolbar(selectedTab, tabs);
        EditorGUILayout.Space();

        switch (selectedTab)
        {
            case 0:
                EditorGUILayout.LabelField("Spawn Individual NPCs", EditorStyles.boldLabel);
                individualCount = EditorGUILayout.IntField("Count", individualCount);
                if (GUILayout.Button("Spawn Individuals"))
                {
                    for (int i = 0; i < individualCount; i++)
                    {
                        spawner.SpawnNPC();
                    }
                    Debug.Log("[NPCSpawnerEditor] Spawned " + individualCount + " individual NPC(s).");
                }
                break;
            case 1:
                EditorGUILayout.LabelField("Spawn Full Family Tree", EditorStyles.boldLabel);
                randomFamilySize = EditorGUILayout.Toggle("Random Family Size", randomFamilySize);
                if (randomFamilySize)
                {
                    EditorGUILayout.LabelField("Family Size Range:");
                    EditorGUILayout.BeginHorizontal();
                    minFamilySize = EditorGUILayout.IntField("Min", minFamilySize);
                    maxFamilySize = EditorGUILayout.IntField("Max", maxFamilySize);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    fixedFamilySize = EditorGUILayout.IntField("Family Size", fixedFamilySize);
                }

                randomGenerations = EditorGUILayout.Toggle("Random Generations", randomGenerations);
                if (randomGenerations)
                {
                    EditorGUILayout.LabelField("Generations Range:");
                    EditorGUILayout.BeginHorizontal();
                    minGenerations = EditorGUILayout.IntField("Min", minGenerations);
                    maxGenerations = EditorGUILayout.IntField("Max", maxGenerations);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    fixedGenerations = EditorGUILayout.IntField("Generations", fixedGenerations);
                }

                if (GUILayout.Button("Spawn Family Tree"))
                {
                    int familySize = randomFamilySize ? Random.Range(minFamilySize, maxFamilySize + 1) : fixedFamilySize;
                    int generations = randomGenerations ? Random.Range(minGenerations, maxGenerations + 1) : fixedGenerations;
                    spawner.SpawnFamilyTree(familySize, generations);
                    Debug.Log("[NPCSpawnerEditor] Spawned a family tree with family size " + familySize + " and " + generations + " generations.");
                }
                break;
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Despawn All NPCs"))
        {
            NPC[] npcs = GameObject.FindObjectsOfType<NPC>();
            foreach (NPC npc in npcs)
            {
                if (Application.isPlaying)
                    Destroy(npc.gameObject);
                else
                    DestroyImmediate(npc.gameObject);
            }
            Debug.Log("[NPCSpawnerEditor] All NPCs despawned.");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
