using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(BodyPartFeature))]
public class BodyPartFeatureDrawer : PropertyDrawer
{
    // Dictionary to track foldout states per property.
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Generate a unique key for this property (based on its propertyPath).
        string key = property.propertyPath;
        if (!foldoutStates.ContainsKey(key))
            foldoutStates[key] = true;

        // Retrieve the enum value for the bodyPart.
        SerializedProperty bodyPartProp = property.FindPropertyRelative("bodyPart");
        string bodyPartName = bodyPartProp.enumDisplayNames[bodyPartProp.enumValueIndex];

        // Draw foldout header with the body part name.
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        foldoutStates[key] = EditorGUI.Foldout(foldoutRect, foldoutStates[key], new GUIContent("Body Part: " + bodyPartName), true);

        if (foldoutStates[key])
        {
            EditorGUI.indentLevel++;
            float yOffset = position.y + EditorGUIUtility.singleLineHeight;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            // Draw Size Classification
            SerializedProperty sizeProp = property.FindPropertyRelative("size");
            Rect sizeRect = new Rect(position.x, yOffset, position.width, lineHeight);
            EditorGUI.PropertyField(sizeRect, sizeProp);
            yOffset += lineHeight;

            // Draw Width Classification
            SerializedProperty widthProp = property.FindPropertyRelative("width");
            Rect widthRect = new Rect(position.x, yOffset, position.width, lineHeight);
            EditorGUI.PropertyField(widthRect, widthProp);
            yOffset += lineHeight;

            // Draw Strength Classification
            SerializedProperty strengthProp = property.FindPropertyRelative("strength");
            Rect strengthRect = new Rect(position.x, yOffset, position.width, lineHeight);
            EditorGUI.PropertyField(strengthRect, strengthProp);
            yOffset += lineHeight;

            // Draw Dexterity Classification
            SerializedProperty dexterityProp = property.FindPropertyRelative("dexterity");
            Rect dexterityRect = new Rect(position.x, yOffset, position.width, lineHeight);
            EditorGUI.PropertyField(dexterityRect, dexterityProp);
            yOffset += lineHeight;

            // Draw Aesthetic Classification
            SerializedProperty aestheticProp = property.FindPropertyRelative("aesthetic");
            Rect aestheticRect = new Rect(position.x, yOffset, position.width, lineHeight);
            EditorGUI.PropertyField(aestheticRect, aestheticProp);
            yOffset += lineHeight;

            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        string key = property.propertyPath;
        // If expanded, allocate height for the foldout header plus five fields.
        if (foldoutStates.ContainsKey(key) && foldoutStates[key])
            return EditorGUIUtility.singleLineHeight * 6;
        else
            return EditorGUIUtility.singleLineHeight;
    }
}
