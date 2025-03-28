using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NPCIdentity))]
public class NPCFamilyTreeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default NPCIdentity inspector.
        DrawDefaultInspector();

        NPCIdentity identity = (NPCIdentity)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Family Tree", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Display Parents
        EditorGUILayout.LabelField("Parents:", EditorStyles.boldLabel);
        if (identity.parents != null && identity.parents.Count > 0)
        {
            foreach (NPCIdentity parent in identity.parents)
            {
                if (parent != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("- " + parent.npcName);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        EditorGUIUtility.PingObject(parent.gameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }

        EditorGUILayout.Space();

        // Display Spouse
        EditorGUILayout.LabelField("Spouse:", EditorStyles.boldLabel);
        if (identity.spouse != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(identity.spouse.npcName);
            if (GUILayout.Button("Ping", GUILayout.Width(50)))
            {
                EditorGUIUtility.PingObject(identity.spouse.gameObject);
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }

        EditorGUILayout.Space();

        // Display Siblings
        EditorGUILayout.LabelField("Siblings:", EditorStyles.boldLabel);
        if (identity.siblings != null && identity.siblings.Count > 0)
        {
            foreach (NPCIdentity sibling in identity.siblings)
            {
                if (sibling != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("- " + sibling.npcName);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        EditorGUIUtility.PingObject(sibling.gameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }

        EditorGUILayout.Space();

        // Display Children
        EditorGUILayout.LabelField("Children:", EditorStyles.boldLabel);
        if (identity.children != null && identity.children.Count > 0)
        {
            foreach (NPCIdentity child in identity.children)
            {
                if (child != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("- " + child.npcName);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        EditorGUIUtility.PingObject(child.gameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }

        EditorGUILayout.Space();

        // Display Ancestors (flat list).
        EditorGUILayout.LabelField("Ancestors:", EditorStyles.boldLabel);
        HashSet<NPCIdentity> ancestors = identity.GetAllAncestors();
        if (ancestors != null && ancestors.Count > 0)
        {
            foreach (NPCIdentity ancestor in ancestors)
            {
                if (ancestor != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("- " + ancestor.npcName);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        EditorGUIUtility.PingObject(ancestor.gameObject);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }
    }
}
