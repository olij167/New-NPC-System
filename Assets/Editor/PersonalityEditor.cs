using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

[CustomEditor(typeof(Personality))]
public class PersonalityEditor : Editor
{
    // Scroll positions for opinion lists.
    private Vector2 dbScrollPos;
    private Vector2 availableConceptScrollPos;
    private Vector2 currentConceptScrollPos;
    private Vector2 availableTangibleScrollPos;
    private Vector2 currentTangibleScrollPos;

    // Dictionaries to track foldout states for individual opinion cells.
    private Dictionary<int, bool> availableConceptFoldouts = new Dictionary<int, bool>();
    private Dictionary<int, bool> currentConceptFoldouts = new Dictionary<int, bool>();
    private Dictionary<int, bool> availableTangibleFoldouts = new Dictionary<int, bool>();
    private Dictionary<int, bool> currentTangibleFoldouts = new Dictionary<int, bool>();

    // Section foldout toggles for entire available opinions sections.
    private bool showAvailableConcepts = true;
    private bool showAvailableTangibles = true;
    private bool showCurrentConcepts = true;
    private bool showCurrentTangibles = true;

    // Constants for cell sizes.
    private const float collapsedCellHeight = 50f;
    private const float expandedCellHeight = 100f;
    private const float maxScrollHeight = 600f; // Maximum scroll view height

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Personality personality = (Personality)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Personality Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ---------- Default Emotion Values ----------
        EditorGUILayout.LabelField("Default Emotions", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        personality.defaultHappiness = EditorGUILayout.Slider("Happiness", personality.defaultHappiness, 0f, 1f);
        personality.defaultPassion = EditorGUILayout.Slider("Passion", personality.defaultPassion, 0f, 1f);
        personality.defaultConfidence = EditorGUILayout.Slider("Confidence", personality.defaultConfidence, 0f, 1f);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        // ---------- Ideological Profile ----------
        EditorGUILayout.LabelField("Ideological Profile", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        personality.ideology.freedom = EditorGUILayout.Slider("Freedom", personality.ideology.freedom, 0f, 1f);
        personality.ideology.privacy = EditorGUILayout.Slider("Privacy", personality.ideology.privacy, 0f, 1f);
        personality.ideology.authority = EditorGUILayout.Slider("Authority", personality.ideology.authority, 0f, 1f);
        personality.ideology.equality = EditorGUILayout.Slider("Equality", personality.ideology.equality, 0f, 1f);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        // ---------- Opinions Database ----------
        EditorGUILayout.LabelField("Opinions Database", EditorStyles.boldLabel);
        SerializedProperty dbProp = serializedObject.FindProperty("opinionsDatabase");
        EditorGUILayout.PropertyField(dbProp);
        EditorGUILayout.Space();

        // ---------- Available Concept Opinions ----------
        showAvailableConcepts = EditorGUILayout.Foldout(showAvailableConcepts, "Available Concept Opinions");
        if (showAvailableConcepts)
        {
            if (personality.opinionsDatabase == null ||
                personality.opinionsDatabase.potentialConceptOpinions == null ||
                personality.opinionsDatabase.potentialConceptOpinions.Count == 0)
            {
                EditorGUILayout.HelpBox("No available concept opinions.", MessageType.Info);
            }
            else
            {
                float availableConceptHeight = Mathf.Min(personality.opinionsDatabase.potentialConceptOpinions.Count * collapsedCellHeight, maxScrollHeight);
                availableConceptScrollPos = EditorGUILayout.BeginScrollView(availableConceptScrollPos, GUILayout.Height(availableConceptHeight));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < personality.opinionsDatabase.potentialConceptOpinions.Count; i++)
                {
                    ConceptOpinion potential = personality.opinionsDatabase.potentialConceptOpinions[i];
                    if (!availableConceptFoldouts.ContainsKey(i))
                        availableConceptFoldouts[i] = false; // default collapsed

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();
                    availableConceptFoldouts[i] = EditorGUILayout.Foldout(availableConceptFoldouts[i], potential.conceptName, true);
                    // Replace Add button with "Added" if opinion exists.
                    bool exists = false;
                    foreach (var op in personality.conceptOpinions)
                    {
                        if (op.conceptName == potential.conceptName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists)
                        EditorGUILayout.LabelField("Added", GUILayout.Width(50));
                    else
                    {
                        if (GUILayout.Button("Add", GUILayout.Width(50)))
                        {
                            ConceptOpinion newConcept = new ConceptOpinion();
                            newConcept.conceptName = potential.conceptName;
                            newConcept.category = potential.category;
                            newConcept.intensity = potential.intensity;
                            newConcept.moralJudgement = potential.moralJudgement;
                            newConcept.description = potential.description;
                            newConcept.requiredIdeology = potential.requiredIdeology; // Ideology requirement for the opinion.
                            personality.conceptOpinions.Add(newConcept);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (availableConceptFoldouts[i])
                    {
                        EditorGUILayout.LabelField(potential.description);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.Space();

        // ---------- Current Concept Opinions ----------
        showCurrentConcepts = EditorGUILayout.Foldout(showCurrentConcepts, "Current Concept Opinions");
        if (showCurrentConcepts)
        {
            if (personality.conceptOpinions == null || personality.conceptOpinions.Count == 0)
            {
                EditorGUILayout.HelpBox("No current concept opinions.", MessageType.Info);
            }
            else
            {
                float totalConceptHeight = 0f;
                for (int i = 0; i < personality.conceptOpinions.Count; i++)
                {
                    bool expanded = currentConceptFoldouts.ContainsKey(i) ? currentConceptFoldouts[i] : false;
                    totalConceptHeight += expanded ? expandedCellHeight : collapsedCellHeight;
                }
                float currentConceptScrollHeight = Mathf.Min(totalConceptHeight, maxScrollHeight);
                currentConceptScrollPos = EditorGUILayout.BeginScrollView(currentConceptScrollPos, GUILayout.Height(currentConceptScrollHeight));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < personality.conceptOpinions.Count; i++)
                {
                    ConceptOpinion opinion = personality.conceptOpinions[i];
                    if (!currentConceptFoldouts.ContainsKey(i))
                        currentConceptFoldouts[i] = false;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();
                    currentConceptFoldouts[i] = EditorGUILayout.Foldout(currentConceptFoldouts[i], opinion.conceptName, true);
                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        personality.conceptOpinions.RemoveAt(i);
                        currentConceptFoldouts.Remove(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (currentConceptFoldouts[i])
                    {
                        opinion.intensity = EditorGUILayout.Slider("Intensity", opinion.intensity, -1f, 1f);
                        opinion.moralJudgement = EditorGUILayout.Slider("Moral", opinion.moralJudgement, -1f, 1f);
                        // Display required ideology information.
                        EditorGUILayout.LabelField("Required Ideology:");
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("Freedom: " + opinion.requiredIdeology.freedom.ToString("F2"));
                        EditorGUILayout.LabelField("Privacy: " + opinion.requiredIdeology.privacy.ToString("F2"));
                        EditorGUILayout.LabelField("Authority: " + opinion.requiredIdeology.authority.ToString("F2"));
                        EditorGUILayout.LabelField("Equality: " + opinion.requiredIdeology.equality.ToString("F2"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.Space();

        // ---------- Available Tangible Opinions ----------
        showAvailableTangibles = EditorGUILayout.Foldout(showAvailableTangibles, "Available Tangible Opinions");
        if (showAvailableTangibles)
        {
            if (personality.opinionsDatabase == null ||
                personality.opinionsDatabase.potentialTangibleOpinions == null ||
                personality.opinionsDatabase.potentialTangibleOpinions.Count == 0)
            {
                EditorGUILayout.HelpBox("No available tangible opinions.", MessageType.Info);
            }
            else
            {
                float availableTangibleHeight = Mathf.Min(personality.opinionsDatabase.potentialTangibleOpinions.Count * collapsedCellHeight, maxScrollHeight);
                availableTangibleScrollPos = EditorGUILayout.BeginScrollView(availableTangibleScrollPos, GUILayout.Height(availableTangibleHeight));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < personality.opinionsDatabase.potentialTangibleOpinions.Count; i++)
                {
                    TangibleOpinion potential = personality.opinionsDatabase.potentialTangibleOpinions[i];
                    int key = i + 10000; // Unique key offset for tangible opinions
                    if (!availableTangibleFoldouts.ContainsKey(key))
                        availableTangibleFoldouts[key] = false;
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();
                    availableTangibleFoldouts[key] = EditorGUILayout.Foldout(availableTangibleFoldouts[key], potential.tangibleName, true);
                    bool exists = false;
                    foreach (var op in personality.tangibleInterests)
                    {
                        if (op.tangibleName == potential.tangibleName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    foreach (var op in personality.tangibleFears)
                    {
                        if (op.tangibleName == potential.tangibleName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists)
                        EditorGUILayout.LabelField("Added", GUILayout.Width(50));
                    else
                    {
                        if (GUILayout.Button("Add", GUILayout.Width(50)))
                        {
                            TangibleOpinion newOpinion = new TangibleOpinion();
                            newOpinion.tangibleName = potential.tangibleName;
                            newOpinion.intensity = potential.intensity;
                            newOpinion.description = potential.description;
                            personality.tangibleInterests.Add(newOpinion);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (availableTangibleFoldouts[key])
                    {
                        EditorGUILayout.LabelField(potential.description);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.Space();

        // ---------- Current Tangible Opinions ----------
        showCurrentTangibles = EditorGUILayout.Foldout(showCurrentTangibles, "Current Tangible Opinions");
        if (showCurrentTangibles)
        {
            int totalTangibleCount = (personality.tangibleInterests != null ? personality.tangibleInterests.Count : 0) +
                                     (personality.tangibleFears != null ? personality.tangibleFears.Count : 0);
            if (totalTangibleCount == 0)
            {
                EditorGUILayout.HelpBox("No current tangible opinions.", MessageType.Info);
            }
            else
            {
                float totalTangibleHeight = 0f;
                if (personality.tangibleInterests != null)
                {
                    for (int i = 0; i < personality.tangibleInterests.Count; i++)
                    {
                        bool expanded = currentTangibleFoldouts.ContainsKey(i) ? currentTangibleFoldouts[i] : false;
                        totalTangibleHeight += expanded ? expandedCellHeight : collapsedCellHeight;
                    }
                }
                if (personality.tangibleFears != null)
                {
                    for (int i = 0; i < personality.tangibleFears.Count; i++)
                    {
                        int key = i + 1000;
                        bool expanded = currentTangibleFoldouts.ContainsKey(key) ? currentTangibleFoldouts[key] : false;
                        totalTangibleHeight += expanded ? expandedCellHeight : collapsedCellHeight;
                    }
                }
                float currentTangibleScrollHeight = Mathf.Min(totalTangibleHeight, maxScrollHeight);
                currentTangibleScrollPos = EditorGUILayout.BeginScrollView(currentTangibleScrollPos, GUILayout.Height(currentTangibleScrollHeight));
                EditorGUILayout.BeginVertical();
                if (personality.tangibleInterests != null)
                {
                    for (int i = 0; i < personality.tangibleInterests.Count; i++)
                    {
                        TangibleOpinion opinion = personality.tangibleInterests[i];
                        if (!currentTangibleFoldouts.ContainsKey(i))
                            currentTangibleFoldouts[i] = false;
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.BeginHorizontal();
                        currentTangibleFoldouts[i] = EditorGUILayout.Foldout(currentTangibleFoldouts[i], opinion.tangibleName, true);
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            personality.tangibleInterests.RemoveAt(i);
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
                if (personality.tangibleFears != null)
                {
                    for (int i = 0; i < personality.tangibleFears.Count; i++)
                    {
                        int key = i + 1000;
                        TangibleOpinion opinion = personality.tangibleFears[i];
                        if (!currentTangibleFoldouts.ContainsKey(key))
                            currentTangibleFoldouts[key] = false;
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.BeginHorizontal();
                        currentTangibleFoldouts[key] = EditorGUILayout.Foldout(currentTangibleFoldouts[key], opinion.tangibleName, true);
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            personality.tangibleFears.RemoveAt(i);
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
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
