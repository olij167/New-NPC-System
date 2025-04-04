using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCIdentity))]
public class NPCIdentityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields.
        DrawDefaultInspector();

        // Add a clean Family Visualization section with only the button.
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Family Visualization", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Family Tree"))
        {
            NPCIdentity identity = (NPCIdentity)target;
            PedigreeGraphWindow window = EditorWindow.GetWindow<PedigreeGraphWindow>();
            window.SetTarget(identity);
            window.Show();
        }
    }
}
