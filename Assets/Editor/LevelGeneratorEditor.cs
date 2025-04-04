using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator generator = (LevelGenerator)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Scan & Copy Recast Graph Area"))
        {
            generator.ScanAndCopyRecastGraphArea();
        }
    }
}
