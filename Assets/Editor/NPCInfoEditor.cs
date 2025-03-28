using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

[CustomEditor(typeof(NPCInfo))]
public class NPCInfoEditor : Editor
{
    // Scroll positions for opinion lists.
    private Vector2 currentConceptScrollPos;
    private Vector2 currentTangibleScrollPos;

    // Dictionaries to track foldout states for current opinion cells.
    private Dictionary<int, bool> currentConceptFoldouts = new Dictionary<int, bool>();
    private Dictionary<int, bool> currentTangibleFoldouts = new Dictionary<int, bool>();

    // New constant sizes for the personality section.
    private const float collapsedCellHeight = 50f;
    private const float expandedCellHeight = 100f;
    private const float maxScrollHeight = 600f;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        NPCInfo npcInfo = (NPCInfo)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("NPC Information Overview", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ---------- Personal Information ----------
        EditorGUILayout.LabelField("Personal Information", EditorStyles.boldLabel);
        if (npcInfo.identity != null)
        {
            EditorGUILayout.LabelField("Full Name:", npcInfo.identity.npcName);
            EditorGUILayout.LabelField("Age:", npcInfo.identity.age.ToString());
            EditorGUILayout.LabelField("Gender:", npcInfo.identity.gender.ToString());
        }
        else
        {
            EditorGUILayout.HelpBox("NPCIdentity component not found.", MessageType.Warning);
        }
        EditorGUILayout.Space();

        // ---------- Sexuality ----------
        EditorGUILayout.LabelField("Sexuality", EditorStyles.boldLabel);
        if (npcInfo.sexuality != null)
        {
            EditorGUILayout.LabelField("Sexual Presentation:", npcInfo.sexuality.visualPresentation.ToString());
            EditorGUILayout.LabelField("Attraction Preferences:",
                "To Masculine: " + npcInfo.sexuality.attractionToMasculine.ToString("F2") +
                ", To Feminine: " + npcInfo.sexuality.attractionToFeminine.ToString("F2"));
        }
        else
        {
            EditorGUILayout.HelpBox("Sexuality component not found.", MessageType.Warning);
        }
        EditorGUILayout.Space();

        // ---------- Family Connections ----------
        EditorGUILayout.LabelField("Family Connections", EditorStyles.boldLabel);
        if (npcInfo.identity != null && npcInfo.relationshipSystem != null)
        {
            StringBuilder familySummary = new StringBuilder();
            familySummary.AppendLine("Parents: " + npcInfo.identity.parents.Count);
            familySummary.AppendLine("Siblings: " + npcInfo.identity.siblings.Count);
            familySummary.AppendLine("Children: " + npcInfo.identity.children.Count);
            familySummary.AppendLine("Spouse: " + (npcInfo.identity.spouse != null ? npcInfo.identity.spouse.npcName : "None"));
            familySummary.AppendLine("Ancestors: " + npcInfo.identity.GetAllAncestors().Count);
            EditorGUILayout.LabelField(familySummary.ToString());
        }
        else
        {
            EditorGUILayout.HelpBox("Relationship or Identity component not found.", MessageType.Warning);
        }
        EditorGUILayout.Space();

        // ---------- Personality Section: Current Opinions Only ----------
        EditorGUILayout.LabelField("Personality - Current Opinions", EditorStyles.boldLabel);
        if (npcInfo.personality == null)
        {
            EditorGUILayout.HelpBox("Personality component not found.", MessageType.Warning);
        }
        else
        {
            // --- Current Concept Opinions ---
            EditorGUILayout.LabelField("Current Concept Opinions", EditorStyles.boldLabel);
            if (npcInfo.personality.conceptOpinions == null || npcInfo.personality.conceptOpinions.Count == 0)
            {
                EditorGUILayout.HelpBox("No current concept opinions.", MessageType.Info);
            }
            else
            {
                float totalConceptHeight = 0f;
                for (int i = 0; i < npcInfo.personality.conceptOpinions.Count; i++)
                {
                    bool expanded = currentConceptFoldouts.ContainsKey(i) ? currentConceptFoldouts[i] : false;
                    totalConceptHeight += expanded ? expandedCellHeight : collapsedCellHeight;
                }
                float currentConceptScrollHeight = Mathf.Min(totalConceptHeight, maxScrollHeight);
                currentConceptScrollPos = EditorGUILayout.BeginScrollView(currentConceptScrollPos, GUILayout.Height(currentConceptScrollHeight));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < npcInfo.personality.conceptOpinions.Count; i++)
                {
                    ConceptOpinion opinion = npcInfo.personality.conceptOpinions[i];
                    if (!currentConceptFoldouts.ContainsKey(i))
                        currentConceptFoldouts[i] = false;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();
                    currentConceptFoldouts[i] = EditorGUILayout.Foldout(currentConceptFoldouts[i], opinion.conceptName, true);
                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        npcInfo.personality.conceptOpinions.RemoveAt(i);
                        currentConceptFoldouts.Remove(i);
                        break; // break to avoid collection modification issues
                    }
                    EditorGUILayout.EndHorizontal();
                    if (currentConceptFoldouts[i])
                    {
                        opinion.intensity = EditorGUILayout.Slider("Intensity", opinion.intensity, -1f, 1f);
                        opinion.moralJudgement = EditorGUILayout.Slider("Moral", opinion.moralJudgement, -1f, 1f);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.Space();

            // --- Current Tangible Opinions ---
            EditorGUILayout.LabelField("Current Tangible Opinions", EditorStyles.boldLabel);
            int totalTangibleCount = (npcInfo.personality.tangibleInterests != null ? npcInfo.personality.tangibleInterests.Count : 0) +
                                     (npcInfo.personality.tangibleFears != null ? npcInfo.personality.tangibleFears.Count : 0);
            if (totalTangibleCount == 0)
            {
                EditorGUILayout.HelpBox("No current tangible opinions.", MessageType.Info);
            }
            else
            {
                float totalTangibleHeight = 0f;
                if (npcInfo.personality.tangibleInterests != null)
                {
                    for (int i = 0; i < npcInfo.personality.tangibleInterests.Count; i++)
                    {
                        bool expanded = currentTangibleFoldouts.ContainsKey(i) ? currentTangibleFoldouts[i] : false;
                        totalTangibleHeight += expanded ? expandedCellHeight : collapsedCellHeight;
                    }
                }
                if (npcInfo.personality.tangibleFears != null)
                {
                    for (int i = 0; i < npcInfo.personality.tangibleFears.Count; i++)
                    {
                        int key = i + 1000;
                        bool expanded = currentTangibleFoldouts.ContainsKey(key) ? currentTangibleFoldouts[key] : false;
                        totalTangibleHeight += expanded ? expandedCellHeight : collapsedCellHeight;
                    }
                }
                float currentTangibleScrollHeight = Mathf.Min(totalTangibleHeight, maxScrollHeight);
                currentTangibleScrollPos = EditorGUILayout.BeginScrollView(currentTangibleScrollPos, GUILayout.Height(currentTangibleScrollHeight));
                EditorGUILayout.BeginVertical();
                // Display tangible interests.
                if (npcInfo.personality.tangibleInterests != null)
                {
                    for (int i = 0; i < npcInfo.personality.tangibleInterests.Count; i++)
                    {
                        TangibleOpinion opinion = npcInfo.personality.tangibleInterests[i];
                        if (!currentTangibleFoldouts.ContainsKey(i))
                            currentTangibleFoldouts[i] = false;
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.BeginHorizontal();
                        currentTangibleFoldouts[i] = EditorGUILayout.Foldout(currentTangibleFoldouts[i], opinion.tangibleName, true);
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            npcInfo.personality.tangibleInterests.RemoveAt(i);
                            currentTangibleFoldouts.Remove(i);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                        if (currentTangibleFoldouts[i])
                        {
                            opinion.intensity = EditorGUILayout.Slider("Intensity", opinion.intensity, 0f, 1f);
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }
                }
                // Display tangible fears.
                if (npcInfo.personality.tangibleFears != null)
                {
                    for (int i = 0; i < npcInfo.personality.tangibleFears.Count; i++)
                    {
                        int key = i + 1000;
                        TangibleOpinion opinion = npcInfo.personality.tangibleFears[i];
                        if (!currentTangibleFoldouts.ContainsKey(key))
                            currentTangibleFoldouts[key] = false;
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.BeginHorizontal();
                        currentTangibleFoldouts[key] = EditorGUILayout.Foldout(currentTangibleFoldouts[key], opinion.tangibleName, true);
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            npcInfo.personality.tangibleFears.RemoveAt(i);
                            currentTangibleFoldouts.Remove(key);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                        if (currentTangibleFoldouts[key])
                        {
                            opinion.intensity = EditorGUILayout.Slider("Intensity", opinion.intensity, 0f, 1f);
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
