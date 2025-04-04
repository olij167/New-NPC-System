using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(NPCPhysicalAppearance))]
public class NPCPhysicalAppearanceEditor : Editor
{
    // Tab settings.
    private int selectedTab = 0;
    private string[] tabNames = { "Basic Appearance", "Body Part Features" };

    // Serialized properties.
    private SerializedProperty characteristicsProp;
    private SerializedProperty bodyPartFeaturesProp;

    // Reorderable list for body part features.
    private ReorderableList bodyPartReorderableList;

    private void OnEnable()
    {
        characteristicsProp = serializedObject.FindProperty("characteristics");
        bodyPartFeaturesProp = serializedObject.FindProperty("bodyPartFeatures");

        // Initialize the reorderable list.
        bodyPartReorderableList = new ReorderableList(serializedObject, bodyPartFeaturesProp, true, true, true, true);
        bodyPartReorderableList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Detailed Body Part Features");
        };
        bodyPartReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            SerializedProperty element = bodyPartFeaturesProp.GetArrayElementAtIndex(index);
            rect.y += 2;
            // This will automatically use our custom BodyPartFeatureDrawer.
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);
        };
        bodyPartReorderableList.elementHeightCallback = (int index) => {
            SerializedProperty element = bodyPartFeaturesProp.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, GUIContent.none, true) + 4;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Tabbed interface.
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);

        if (selectedTab == 0)
        {
            // Basic Appearance tab.
            EditorGUILayout.LabelField("Basic Appearance", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(characteristicsProp.FindPropertyRelative("skinColor"));
            EditorGUILayout.PropertyField(characteristicsProp.FindPropertyRelative("hairColor"));
            EditorGUILayout.PropertyField(characteristicsProp.FindPropertyRelative("eyeColor"));
            EditorGUILayout.PropertyField(characteristicsProp.FindPropertyRelative("height"));
            EditorGUILayout.PropertyField(characteristicsProp.FindPropertyRelative("weight"));
        }
        else if (selectedTab == 1)
        {
            // Body Part Features tab.
            EditorGUILayout.LabelField("Body Part Features", EditorStyles.boldLabel);
            bodyPartReorderableList.DoLayoutList();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
