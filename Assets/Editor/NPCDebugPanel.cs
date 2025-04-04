using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class NPCDebugPanel : EditorWindow
{
    private Vector2 npcListScroll;
    private Vector2 detailScroll;
    private List<NPC> npcList = new List<NPC>();
    private NPC selectedNPC;

    [MenuItem("Window/NPC Debug Panel")]
    public static void ShowWindow()
    {
        GetWindow<NPCDebugPanel>("NPC Debug Panel");
    }

    private void OnEnable()
    {
        RefreshNPCList();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // Left side: NPC List
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("NPC List", EditorStyles.boldLabel);
        npcListScroll = EditorGUILayout.BeginScrollView(npcListScroll);
        foreach (NPC npc in npcList)
        {
            if (npc == null)
                continue;
            string npcName = (npc.identity != null) ? npc.identity.npcName : npc.name;
            if (GUILayout.Button(npcName))
            {
                selectedNPC = npc;
            }
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Refresh"))
        {
            RefreshNPCList();
        }
        EditorGUILayout.EndVertical();

        // Right side: NPC Details
        EditorGUILayout.BeginVertical();
        detailScroll = EditorGUILayout.BeginScrollView(detailScroll);
        if (selectedNPC != null)
        {
            EditorGUILayout.LabelField("NPC Details", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            DrawNPCDetails(selectedNPC);
        }
        else
        {
            EditorGUILayout.LabelField("Select an NPC to view details.");
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void RefreshNPCList()
    {
        npcList.Clear();
        NPC[] npcs = GameObject.FindObjectsOfType<NPC>();
        npcList.AddRange(npcs);
    }

    private void DrawNPCDetails(NPC npc)
    {
        StringBuilder sb = new StringBuilder();

        // Identity Section
        sb.AppendLine("<b>Identity</b>");
        if (npc.identity != null)
        {
            sb.AppendLine("Name: " + npc.identity.npcName);
            sb.AppendLine("Age: " + npc.identity.age);
            sb.AppendLine("Gender: " + npc.identity.gender.ToString());
        }
        else
        {
            sb.AppendLine("NPCIdentity component not found.");
        }
        sb.AppendLine("---------------------");

        // Personality Section
        sb.AppendLine("<b>Personality</b>");
        if (npc.personality != null)
        {
            sb.AppendLine("Default Happiness: " + npc.personality.defaultHappiness.ToString("F2"));
            sb.AppendLine("Default Passion: " + npc.personality.defaultPassion.ToString("F2"));
            sb.AppendLine("Default Confidence: " + npc.personality.defaultConfidence.ToString("F2"));
            sb.AppendLine("<b>Ideology</b>");
            sb.AppendLine("Freedom: " + npc.personality.ideology.freedom.ToString("F2"));
            sb.AppendLine("Privacy: " + npc.personality.ideology.privacy.ToString("F2"));
            sb.AppendLine("Authority: " + npc.personality.ideology.authority.ToString("F2"));
            sb.AppendLine("Equality: " + npc.personality.ideology.equality.ToString("F2"));
            if (npc.personality.conceptOpinions != null && npc.personality.conceptOpinions.Count > 0)
            {
                sb.AppendLine("<b>Concept Opinions</b>");
                foreach (var op in npc.personality.conceptOpinions)
                    sb.AppendLine(op.conceptName + ": " + op.intensity.ToString("F2"));
            }
        }
        else
        {
            sb.AppendLine("Personality component not found.");
        }
        sb.AppendLine("---------------------");

        // Perception Section
        sb.AppendLine("<b>Perception</b>");
        if (npc.perceptionSystem != null)
        {
            sb.AppendLine("Perceived Objects: " + npc.perceptionSystem.perceivedObjects.Count);
        }
        else
        {
            sb.AppendLine("PerceptionSystem component not found.");
        }
        sb.AppendLine("---------------------");

        // Skills Section
        sb.AppendLine("<b>Skills</b>");
        if (npc.npcSkills != null && npc.npcSkills.skills != null)
        {
            foreach (Skill skill in npc.npcSkills.skills)
            {
                sb.AppendLine(skill.skillName + ": " + skill.level.ToString("F2"));
            }
        }
        else
        {
            sb.AppendLine("NPCSkills component not found.");
        }
        sb.AppendLine("---------------------");

        // Relationships Section
        sb.AppendLine("<b>Relationships</b>");
        if (npc.relationshipSystem != null && npc.relationshipSystem.relationshipInfo != null)
        {
            foreach (var kvp in npc.relationshipSystem.relationshipInfo)
            {
                NPC other = kvp.Key;
                if (other != null && other.identity != null)
                    sb.AppendLine("With " + other.identity.npcName + ": " + kvp.Value.category.ToString());
            }
        }
        else
        {
            sb.AppendLine("RelationshipSystem component not found.");
        }
        sb.AppendLine("---------------------");

        // Memory Log Section
        sb.AppendLine("<b>Memory Log</b>");
        if (npc.memorySystem != null && npc.memorySystem.memories != null)
        {
            foreach (var memory in npc.memorySystem.memories)
            {
                sb.AppendLine("Event: " + memory.eventData.description + " @ " + memory.timeRecorded.ToString("F2"));
            }
        }
        else
        {
            sb.AppendLine("MemorySystem component not found.");
        }
        sb.AppendLine("---------------------");

        // Debug State Section (Placeholder)
        sb.AppendLine("<b>Debug State</b>");
        sb.AppendLine("Last Decision: (Not implemented)");

        EditorGUILayout.HelpBox(sb.ToString(), MessageType.None);
    }
}
