//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(NPCInfo))]
//public class NPCInfoEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        NPCInfo npcInfo = (NPCInfo)target;

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("NPC Information Overview", EditorStyles.boldLabel);
//        EditorGUILayout.Space();

//        // ---------- Personal Information ----------
//        EditorGUILayout.LabelField("Personal Information", EditorStyles.boldLabel);
//        if (npcInfo.identity != null)
//        {
//            EditorGUILayout.LabelField("Full Name:", npcInfo.identity.npcName);
//            EditorGUILayout.LabelField("Age:", npcInfo.identity.age.ToString());
//            EditorGUILayout.LabelField("Gender:", npcInfo.identity.gender.ToString());
//        }
//        else
//        {
//            EditorGUILayout.HelpBox("NPCIdentity component not found.", MessageType.Warning);
//        }
//        EditorGUILayout.Space();

//        // ---------- Sexuality (Attraction Preferences) ----------
//        EditorGUILayout.LabelField("Sexuality", EditorStyles.boldLabel);
//        if (npcInfo.sexuality != null)
//        {
//            // Now, sexuality only handles attraction preferences.
//            EditorGUILayout.LabelField("Attraction Preferences:", "Defined in Sexuality component");
//        }
//        else
//        {
//            EditorGUILayout.HelpBox("Sexuality component not found.", MessageType.Warning);
//        }
//        EditorGUILayout.Space();

//        // ---------- Physical Appearance ----------
//        EditorGUILayout.LabelField("Physical Appearance", EditorStyles.boldLabel);
//        NPCPhysicalAppearance appearance = npcInfo.GetComponent<NPCPhysicalAppearance>();
//        if (appearance != null)
//        {
//            EditorGUILayout.ColorField("Skin Color:", appearance.characteristics.skinColor);
//            EditorGUILayout.ColorField("Hair Color:", appearance.characteristics.hairColor);
//            EditorGUILayout.ColorField("Eye Color:", appearance.characteristics.eyeColor);
//            EditorGUILayout.LabelField("Height:", appearance.characteristics.height.ToString("F2") + " m");
//            EditorGUILayout.LabelField("Weight:", appearance.characteristics.weight.ToString("F2") + " kg");
//            EditorGUILayout.Space();
//            EditorGUILayout.LabelField("Sexual Presentation", EditorStyles.boldLabel);
//            EditorGUILayout.LabelField("Masculinity:", appearance.masculinity.ToString("F2"));
//            EditorGUILayout.LabelField("Visual Presentation:", appearance.visualPresentation.ToString());
//        }
//        else
//        {
//            EditorGUILayout.HelpBox("NPCPhysicalAppearance component not found.", MessageType.Warning);
//        }
//        EditorGUILayout.Space();

//        // ---------- Personality Section (Opinions) ----------
//        EditorGUILayout.LabelField("Personality - Current Opinions", EditorStyles.boldLabel);
//        if (npcInfo.personality == null)
//        {
//            EditorGUILayout.HelpBox("Personality component not found.", MessageType.Warning);
//        }
//        else
//        {
//            // Display personality opinions (this section remains as before).
//            EditorGUILayout.LabelField("Concept Opinions:", EditorStyles.boldLabel);
//            if (npcInfo.personality.conceptOpinions == null || npcInfo.personality.conceptOpinions.Count == 0)
//                EditorGUILayout.HelpBox("No current concept opinions.", MessageType.Info);
//            else
//            {
//                foreach (var opinion in npcInfo.personality.conceptOpinions)
//                    EditorGUILayout.LabelField(opinion.conceptName, opinion.intensity.ToString("F2"));
//            }
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
