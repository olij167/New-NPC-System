using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PedigreeGraphWindow : EditorWindow
{
    // The current focal NPC from which the full family tree is generated.
    private NPCIdentity rootIdentity;
    // The node that is currently selected (for re-centering).
    private NPCIdentity selectedNode;

    // Precomputed layout for the full family tree.
    private Dictionary<NPCIdentity, Vector2> fullTreePositions = new Dictionary<NPCIdentity, Vector2>();
    // Record node rectangles for click detection.
    private Dictionary<NPCIdentity, Rect> nodeRects = new Dictionary<NPCIdentity, Rect>();

    // Scroll position for the canvas.
    private Vector2 scrollPos;
    // Zoom factor.
    private float zoom = 1.0f;
    private const float minZoom = 0.5f;
    private const float maxZoom = 2.0f;

    // Layout settings.
    private const float nodeWidth = 120f;
    private const float nodeHeight = 60f;
    private const float horizontalSpacing = 40f;
    // Set verticalSpacing to nodeHeight plus two fixed vertical bars (each 20 pixels).
    private const float verticalBarLength = 20f;
    private const float verticalSpacing = 60f + 2 * verticalBarLength; // equals 100f

    // Canvas dimensions (dynamically determined based on window size).
    private float canvasWidth;
    private float canvasHeight;

    // To prevent duplicate drawing.
    private HashSet<NPCIdentity> drawnNodes = new HashSet<NPCIdentity>();

    // To track previous window size for re-centering.
    private Vector2 previousWindowSize;

    [MenuItem("Window/Pedigree Graph")]
    public static void ShowWindow()
    {
        GetWindow<PedigreeGraphWindow>("Pedigree Graph");
    }

    private void OnGUI()
    {
        // Process zooming and panning first.
        ProcessZoom();
        ProcessPanning();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Interactive Pedigree / Genealogy Chart", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Let the user select a new focal NPC.
        NPCIdentity newTarget = (NPCIdentity)EditorGUILayout.ObjectField("Select NPC", rootIdentity, typeof(NPCIdentity), true);
        if (newTarget != rootIdentity)
        {
            rootIdentity = newTarget;
            selectedNode = newTarget;
            BuildFullFamilyTree();
            RecenterScrollOnTarget();
            Repaint();
        }
        if (rootIdentity == null)
        {
            EditorGUILayout.HelpBox("Please assign an NPC Identity.", MessageType.Info);
            return;
        }

        // Update canvas dimensions based on the current window size.
        canvasWidth = Mathf.Max(position.width, 800);
        canvasHeight = Mathf.Max(position.height, 1200);
        if (previousWindowSize != position.size)
        {
            RecenterScrollOnTarget();
            previousWindowSize = position.size;
        }

        // Begin scroll view.
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);

        // Apply zoom scaling.
        Matrix4x4 prevMatrix = GUI.matrix;
        GUIUtility.ScaleAroundPivot(Vector2.one * zoom, Vector2.zero);

        // Draw background canvas.
        Rect canvasRect = new Rect(0, 0, canvasWidth, canvasHeight);
        GUI.Box(canvasRect, "");

        // Center generation 0 (the focal node) at the center of the canvas.
        Vector2 rootPos = new Vector2(canvasWidth / 2, canvasHeight / 2);

        // Draw all nodes from the precomputed full tree layout.
        drawnNodes.Clear();
        nodeRects.Clear();
        foreach (var kvp in fullTreePositions)
        {
            DrawNode(kvp.Key, kvp.Value);
        }

        // Draw connections.
        DrawAllConnections();

        // Process mouse clicks for interactive re-centering.
        ProcessNodeClicks();

        // Restore previous GUI matrix.
        GUI.matrix = prevMatrix;

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Processes mouse wheel events to update the zoom factor.
    /// </summary>
    private void ProcessZoom()
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom + zoomDelta, minZoom, maxZoom);
            e.Use();
            Repaint();
        }
    }

    /// <summary>
    /// Processes mouse drag events to allow panning.
    /// </summary>
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

    /// <summary>
    /// Builds the full family tree layout from the top-most ancestor down.
    /// </summary>
    private void BuildFullFamilyTree()
    {
        fullTreePositions.Clear();
        drawnNodes.Clear();
        // Find the top ancestor (an NPC with no parents) starting from the focal node.
        NPCIdentity topAncestor = GetTopAncestor(rootIdentity);
        // Recursively compute positions for the descendant tree.
        float treeWidth = CalculateLayout(topAncestor, 0, fullTreePositions);
        // Offset the tree horizontally to center it on the canvas.
        float offsetX = (canvasWidth - treeWidth) / 2;
        List<NPCIdentity> keys = new List<NPCIdentity>(fullTreePositions.Keys);
        foreach (NPCIdentity key in keys)
        {
            Vector2 pos = fullTreePositions[key];
            pos.x += offsetX;
            fullTreePositions[key] = pos;
        }
        // Add spouse nodes as offshoots on the same generation row.
        AddSpouseNodes();
    }

    /// <summary>
    /// Recursively calculates positions for the descendant tree.
    /// Generation 0 is the top ancestor; each generation increases the y value by verticalSpacing.
    /// Returns the total width required for the subtree.
    /// </summary>
    private float CalculateLayout(NPCIdentity node, int generation, Dictionary<NPCIdentity, Vector2> positions)
    {
        float baseY = 50f; // Y for generation 0.
        float y = baseY + generation * verticalSpacing;

        if (node.children == null || node.children.Count == 0)
        {
            positions[node] = new Vector2(0, y);
            return nodeWidth;
        }

        float totalWidth = 0;
        List<float> childWidths = new List<float>();
        foreach (var child in node.children)
        {
            float cw = CalculateLayout(child, generation + 1, positions);
            childWidths.Add(cw);
            totalWidth += cw;
        }
        totalWidth += (node.children.Count - 1) * horizontalSpacing;

        float parentX = 0;
        float currentX = -totalWidth / 2;
        foreach (var child in node.children)
        {
            float cw = childWidths[node.children.IndexOf(child)];
            Vector2 childPos = positions[child];
            childPos.x = currentX + cw / 2;
            positions[child] = childPos;
            parentX += childPos.x;
            currentX += cw + horizontalSpacing;
        }
        parentX /= node.children.Count;
        positions[node] = new Vector2(parentX, y);
        return totalWidth;
    }

    /// <summary>
    /// Returns the top-most ancestor (an NPC with no parents) for the given node.
    /// </summary>
    private NPCIdentity GetTopAncestor(NPCIdentity node)
    {
        if (node.parents == null || node.parents.Count == 0)
            return node;
        return GetTopAncestor(node.parents[0]);
    }

    /// <summary>
    /// Adds spouse nodes as offshoots on the same generation row if not already present.
    /// </summary>
    private void AddSpouseNodes()
    {
        List<NPCIdentity> keys = new List<NPCIdentity>(fullTreePositions.Keys);
        foreach (NPCIdentity node in keys)
        {
            if (node.spouse != null && !fullTreePositions.ContainsKey(node.spouse))
            {
                Vector2 pos = fullTreePositions[node];
                Vector2 spousePos = new Vector2(pos.x + nodeWidth + horizontalSpacing / 2, pos.y);
                fullTreePositions[node.spouse] = spousePos;
            }
        }
    }

    /// <summary>
    /// Draws a node (a box with NPC details) at the given position and records its rectangle.
    /// </summary>
    private void DrawNode(NPCIdentity identity, Vector2 pos)
    {
        Rect rect = new Rect(pos.x - nodeWidth / 2, pos.y - nodeHeight / 2, nodeWidth, nodeHeight);
        GUI.Box(rect, "");
        GUIStyle centered = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        string info = identity.npcName + "\nAge: " + identity.age + "\n" + identity.gender.ToString();
        GUI.Label(rect, info, centered);
        nodeRects[identity] = rect;
    }

    /// <summary>
    /// Draws all connections (parent–child and spouse) based on the precomputed layout.
    /// For parent–child connections, a vertical bar is drawn from the parent's bottom edge and
    /// from the child's top edge; a horizontal line connects these endpoints.
    /// Spouse connections are drawn as a horizontal line beneath the nodes.
    /// </summary>
    private void DrawAllConnections()
    {
        // Draw parent–child connections.
        foreach (KeyValuePair<NPCIdentity, Vector2> kvp in fullTreePositions)
        {
            NPCIdentity parent = kvp.Key;
            if (parent.children != null && parent.children.Count > 0)
            {
                Vector2 parentPos = fullTreePositions[parent];
                // Draw parent's vertical bar.
                Vector3 pStart = new Vector3(parentPos.x, parentPos.y + nodeHeight / 2, 0);
                Vector3 pEnd = new Vector3(parentPos.x, parentPos.y + nodeHeight / 2 + verticalBarLength, 0);
                Handles.DrawAAPolyLine(2f, pStart, pEnd);

                // Collect children's vertical endpoints.
                List<Vector3> childEndpoints = new List<Vector3>();
                foreach (var child in parent.children)
                {
                    if (!fullTreePositions.ContainsKey(child))
                        continue;
                    Vector2 childPos = fullTreePositions[child];
                    Vector3 childEndpoint = new Vector3(childPos.x, childPos.y - nodeHeight / 2 - verticalBarLength, 0);
                    childEndpoints.Add(childEndpoint);
                    // Draw child's vertical bar.
                    Vector3 cTop = new Vector3(childPos.x, childPos.y - nodeHeight / 2, 0);
                    Handles.DrawAAPolyLine(2f, childEndpoint, cTop);
                }
                if (childEndpoints.Count > 0)
                {
                    // Determine the horizontal connector endpoints.
                    float minX = childEndpoints[0].x;
                    float maxX = childEndpoints[0].x;
                    foreach (var pt in childEndpoints)
                    {
                        if (pt.x < minX) minX = pt.x;
                        if (pt.x > maxX) maxX = pt.x;
                    }
                    // Include the parent's endpoint.
                    minX = Mathf.Min(minX, pEnd.x);
                    maxX = Mathf.Max(maxX, pEnd.x);
                    Vector3 hStart = new Vector3(minX, pEnd.y, 0);
                    Vector3 hEnd = new Vector3(maxX, pEnd.y, 0);
                    Handles.DrawAAPolyLine(2f, hStart, hEnd);
                }
            }
        }
        // Draw spouse connections.
        foreach (KeyValuePair<NPCIdentity, Vector2> kvp in fullTreePositions)
        {
            NPCIdentity node = kvp.Key;
            if (node.spouse != null && fullTreePositions.ContainsKey(node.spouse))
            {
                bool shareChildren = false;
                if (node.children != null)
                {
                    foreach (var child in node.children)
                    {
                        if (child.parents != null && child.parents.Contains(node.spouse))
                        {
                            shareChildren = true;
                            break;
                        }
                    }
                }
                if (!shareChildren)
                {
                    Vector2 pos = fullTreePositions[node];
                    Vector2 spousePos = fullTreePositions[node.spouse];
                    DrawSpouseConnection(pos, spousePos);
                }
            }
        }
    }

    /// <summary>
    /// Draws a right-angle connection between a parent and child.
    /// The parent's vertical bar is drawn from its bottom edge down by verticalBarLength.
    /// The child's vertical bar is drawn from its top edge up by verticalBarLength.
    /// A horizontal line connects these endpoints.
    /// </summary>
    private void DrawRightAngleConnection(Vector2 parentPos, Vector2 childPos, string relation)
    {
        float parentBottom = parentPos.y + nodeHeight / 2;
        float childTop = childPos.y - nodeHeight / 2;
        Vector3 parentEndpoint = new Vector3(parentPos.x, parentBottom + verticalBarLength, 0);
        Vector3 childEndpoint = new Vector3(childPos.x, childTop - verticalBarLength, 0);
        // Draw parent's vertical bar.
        Vector3 pStart = new Vector3(parentPos.x, parentBottom, 0);
        Handles.DrawAAPolyLine(2f, pStart, parentEndpoint);
        // Draw child's vertical bar.
        Vector3 cStart = new Vector3(childPos.x, childTop, 0);
        Handles.DrawAAPolyLine(2f, childEndpoint, cStart);
        // Draw horizontal connector.
        Handles.DrawAAPolyLine(2f, parentEndpoint, childEndpoint);
    }

    /// <summary>
    /// Draws a spouse connection as a horizontal line beneath the nodes.
    /// </summary>
    private void DrawSpouseConnection(Vector2 pos, Vector2 spousePos)
    {
        float y = pos.y + nodeHeight / 2 + verticalBarLength;
        Vector3 sStart = new Vector3(pos.x, y, 0);
        Vector3 sEnd = new Vector3(spousePos.x, y, 0);
        Handles.DrawAAPolyLine(2f, sStart, sEnd);
    }

    /// <summary>
    /// Processes mouse clicks on node rectangles.
    /// When a node is clicked, re-center the scroll view on that node without regenerating the tree.
    /// </summary>
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

    /// <summary>
    /// Adjusts the scroll position so that the selected node is centered in the window.
    /// </summary>
    private void RecenterScrollOnTarget()
    {
        if (selectedNode != null && nodeRects.ContainsKey(selectedNode))
        {
            Rect rect = nodeRects[selectedNode];
            scrollPos = new Vector2(rect.center.x - position.width / 2, rect.center.y - position.height / 2);
        }
    }
}
