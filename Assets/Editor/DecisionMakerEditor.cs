using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DecisionMaker))]
public class DecisionMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector elements first.
        DrawDefaultInspector();

        DecisionMaker decisionMaker = (DecisionMaker)target;
        if (decisionMaker == null)
            return;

        // Ensure the NPC and its integrated systems are present.
        NPC npc = decisionMaker.GetComponent<NPC>();
        if (npc == null || npc.perceptionSystem == null)
        {
            EditorGUILayout.HelpBox("NPC or its PerceptionSystem component is missing!", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Decision Maker Debug Info", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Display Perception Information
        EditorGUILayout.LabelField("Perception Data", EditorStyles.boldLabel);
        List<GameObject> prioritizedObjects = npc.perceptionSystem.GetPrioritizedPerceivedObjects();
        EditorGUILayout.LabelField("Total Perceived Objects: " + prioritizedObjects.Count.ToString());
        if (prioritizedObjects.Count > 0)
        {
            foreach (GameObject obj in prioritizedObjects)
            {
                float score = npc.perceptionSystem.GetCombinedPerceptionScore(obj);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(obj.name, GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField("Score: " + score.ToString("F2"));
                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                {
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No objects perceived.");
        }

        EditorGUILayout.Space();

        // Display Action Utilities
        EditorGUILayout.LabelField("Action Utilities", EditorStyles.boldLabel);
        if (decisionMaker.actions != null && decisionMaker.actions.Count > 0)
        {
            foreach (NPCAction action in decisionMaker.actions)
            {
                float utility = action.GetUtility(npc);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(action.actionName, GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField("Utility: " + utility.ToString("F2"));
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No actions assigned.");
        }
    }
}
