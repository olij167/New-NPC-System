using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCSpawner))]
public class NPCSpawnerEditor : Editor
{
    // Fields for specifying spawn counts in the editor.
    private int numberOfNPCsToSpawn = 1;
    private int numberOfFamilyMembers = 3;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields for NPCSpawner.
        DrawDefaultInspector();

        // Add a separator and a header for our debug tools.
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawner Debug Tools", EditorStyles.boldLabel);

        // Field and button to spawn individual NPCs.
        numberOfNPCsToSpawn = EditorGUILayout.IntField("Number of Individual NPCs", numberOfNPCsToSpawn);
        if (GUILayout.Button("Spawn Individual NPC(s)"))
        {
            NPCSpawner spawner = (NPCSpawner)target;
            for (int i = 0; i < numberOfNPCsToSpawn; i++)
            {
                spawner.SpawnNPC();
            }
            Debug.Log(numberOfNPCsToSpawn + " NPC(s) spawned.");
        }

        EditorGUILayout.Space();

        // Field and button to spawn a family.
        numberOfFamilyMembers = EditorGUILayout.IntField("Family Size", numberOfFamilyMembers);
        if (GUILayout.Button("Spawn Family"))
        {
            NPCSpawner spawner = (NPCSpawner)target;
            spawner.SpawnFamily(numberOfFamilyMembers);
            Debug.Log("Family of " + numberOfFamilyMembers + " spawned.");
        }

        EditorGUILayout.Space();

        // Button to despawn all NPCs.
        if (GUILayout.Button("Despawn All NPCs"))
        {
            // Finds all NPC components in the scene.
            NPC[] npcs = GameObject.FindObjectsOfType<NPC>();
            foreach (NPC npc in npcs)
            {
                if (Application.isPlaying)
                {
                    Destroy(npc.gameObject);
                }
                else
                {
                    DestroyImmediate(npc.gameObject);
                }
            }
            Debug.Log("All NPCs despawned.");
        }
    }
}
