using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PedigreeGraphWindow : EditorWindow
{
    public NPCIdentity rootIdentity;       // Focal NPC for the pedigree.
    public NPCIdentity selectedNode;

    private Dictionary<NPCIdentity, Vector2> nodePositions = new Dictionary<NPCIdentity, Vector2>();
    private Dictionary<NPCIdentity, Rect> nodeRects = new Dictionary<NPCIdentity, Rect>();

    private Vector2 scrollPos;
    private float zoom = 1.0f;
    private const float minZoom = 0.5f;
    private const float maxZoom = 2.0f;

    private const float nodeWidth = 120f;
    private const float nodeHeight = 60f;
    private const float horizontalSpacing = 40f;
    private const float verticalSpacing = 100f;
    private const float connectionOffset = 10f;

    private Vector2 previousWindowSize;

    [MenuItem("Window/Pedigree Graph")]
    public static void ShowWindow()
    {
        GetWindow<PedigreeGraphWindow>("Pedigree Graph");
    }

    public void SetTarget(NPCIdentity target)
    {
        rootIdentity = target;
        selectedNode = target;
        BuildLayout();
        RecenterScrollOnTarget();
        Repaint();
    }

    private void BuildLayout()
    {
        nodePositions.Clear();
        nodeRects.Clear();

        if (rootIdentity == null)
            return;

        float canvasWidth = Mathf.Max(position.width, 800);
        float canvasHeight = Mathf.Max(position.height, 600);

        // Place the focal NPC at the center.
        nodePositions[rootIdentity] = new Vector2(canvasWidth / 2, canvasHeight / 2);

        // Layout direct parents (above focal).
        List<NPCIdentity> parents = rootIdentity.familyManager.parents;
        if (parents != null && parents.Count > 0)
        {
            float totalWidth = parents.Count * nodeWidth + (parents.Count - 1) * horizontalSpacing;
            float startX = (canvasWidth - totalWidth) / 2;
            float yPos = canvasHeight / 2 - verticalSpacing;
            foreach (NPCIdentity parent in parents)
            {
                nodePositions[parent] = new Vector2(startX + nodeWidth / 2, yPos);
                startX += nodeWidth + horizontalSpacing;
            }
        }

        // Layout direct children (below focal).
        List<NPCIdentity> children = rootIdentity.familyManager.children;
        if (children != null && children.Count > 0)
        {
            float totalWidth = children.Count * nodeWidth + (children.Count - 1) * horizontalSpacing;
            float startX = (canvasWidth - totalWidth) / 2;
            float yPos = canvasHeight / 2 + verticalSpacing;
            foreach (NPCIdentity child in children)
            {
                nodePositions[child] = new Vector2(startX + nodeWidth / 2, yPos);
                startX += nodeWidth + horizontalSpacing;
            }
        }

        // Layout spouse (same generation as focal).
        if (rootIdentity.familyManager != null && rootIdentity.familyManager.spouse != null)
        {
            NPCIdentity spouseId = rootIdentity.familyManager.spouse.GetComponent<NPCIdentity>();
            if (spouseId != null)
            {
                nodePositions[spouseId] = new Vector2(canvasWidth / 2 + nodeWidth + horizontalSpacing, canvasHeight / 2);
            }
        }
    }

    private void OnGUI()
    {
        ProcessZoom();
        ProcessPanning();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pedigree Graph", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        NPCIdentity newTarget = (NPCIdentity)EditorGUILayout.ObjectField("Select NPC", rootIdentity, typeof(NPCIdentity), true);
        if (newTarget != rootIdentity)
            SetTarget(newTarget);
        if (rootIdentity == null)
        {
            EditorGUILayout.HelpBox("Assign an NPC Identity.", MessageType.Info);
            return;
        }

        float canvasWidth = Mathf.Max(position.width, 800);
        float canvasHeight = Mathf.Max(position.height, 600);
        if (previousWindowSize != position.size)
        {
            RecenterScrollOnTarget();
            previousWindowSize = position.size;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);
        Matrix4x4 prevMatrix = GUI.matrix;
        GUIUtility.ScaleAroundPivot(Vector2.one * zoom, Vector2.zero);

        Rect canvasRect = new Rect(0, 0, canvasWidth, canvasHeight);
        GUI.Box(canvasRect, "");

        DrawConnections();

        foreach (var kvp in nodePositions)
            DrawNode(kvp.Key, kvp.Value);

        ProcessNodeClicks();

        GUI.matrix = prevMatrix;
        EditorGUILayout.EndScrollView();
    }

    private void DrawNode(NPCIdentity identity, Vector2 pos)
    {
        string label = identity.npcName;
        if (identity == rootIdentity)
        {
            label += "\n(Focal)";
        }
        else if (rootIdentity.familyManager.parents.Contains(identity))
        {
            label += "\n(Parent)";
        }
        else if (rootIdentity.familyManager.children.Contains(identity))
        {
            label += "\n(Child)";
        }
        else if (rootIdentity.familyManager.spouse != null &&
                 rootIdentity.familyManager.spouse.GetComponent<NPCIdentity>() == identity)
        {
            label += "\n(Spouse)";
        }
        Rect rect = new Rect(pos.x - nodeWidth / 2, pos.y - nodeHeight / 2, nodeWidth, nodeHeight);
        GUI.Box(rect, label);
        nodeRects[identity] = rect;
    }

    private void DrawConnections()
    {
        // Draw parent-child connections.
        foreach (NPCIdentity parent in rootIdentity.familyManager.parents)
        {
            if (nodePositions.ContainsKey(parent))
            {
                Vector2 parentPos = nodePositions[parent];
                Vector2 focalPos = nodePositions[rootIdentity];
                Handles.DrawAAPolyLine(3f,
                    new Vector3(parentPos.x, parentPos.y + nodeHeight / 2 + connectionOffset, 0),
                    new Vector3(focalPos.x, focalPos.y - nodeHeight / 2 - connectionOffset, 0));
            }
        }
        foreach (NPCIdentity child in rootIdentity.familyManager.children)
        {
            if (nodePositions.ContainsKey(child))
            {
                Vector2 focalPos = nodePositions[rootIdentity];
                Vector2 childPos = nodePositions[child];
                Handles.DrawAAPolyLine(3f,
                    new Vector3(focalPos.x, focalPos.y + nodeHeight / 2 + connectionOffset, 0),
                    new Vector3(childPos.x, childPos.y - nodeHeight / 2 - connectionOffset, 0));
            }
        }
        // Draw spouse connection.
        if (rootIdentity.familyManager.spouse != null)
        {
            NPCIdentity spouseId = rootIdentity.familyManager.spouse.GetComponent<NPCIdentity>();
            if (spouseId != null && nodePositions.ContainsKey(spouseId))
            {
                Vector2 focalPos = nodePositions[rootIdentity];
                Vector2 spousePos = nodePositions[spouseId];
                float midY = (focalPos.y + spousePos.y) / 2;
                Handles.DrawDottedLine(new Vector3(focalPos.x, midY, 0), new Vector3(spousePos.x, midY, 0), 3f);
            }
        }
    }

    private void ProcessZoom()
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            float delta = -e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom + delta, minZoom, maxZoom);
            e.Use();
            Repaint();
        }
    }

    private void ProcessPanning()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            scrollPos -= e.delta;
            e.Use();
            Repaint();
        }
    }

    private void ProcessNodeClicks()
    {
        Event e = Event.current;
        if (e.type != EventType.MouseDown)
            return;
        foreach (var kvp in nodeRects)
        {
            if (kvp.Value.Contains(e.mousePosition))
            {
                selectedNode = kvp.Key;
                RecenterScrollOnTarget();
                Repaint();
                break;
            }
        }
    }

    private void RecenterScrollOnTarget()
    {
        if (selectedNode != null && nodeRects.ContainsKey(selectedNode))
        {
            Rect rect = nodeRects[selectedNode];
            scrollPos = new Vector2(rect.center.x - position.width / 2, rect.center.y - position.height / 2);
        }
    }
}
