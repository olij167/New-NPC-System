using UnityEngine;
using System.Collections.Generic;
using Pathfinding;  // Required for A* Pathfinding Pro types

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Generation Settings")]
    [Tooltip("List of object templates for level generation.")]
    public List<ObjectTemplate> objectTemplates = new List<ObjectTemplate>();
    [Tooltip("Center of the spawn area.")]
    public Vector3 spawnAreaCenter;
    [Tooltip("Size of the spawn area.")]
    public Vector3 spawnAreaSize;
    [Tooltip("Number of objects to spawn.")]
    public int numberOfObjects = 20;

    void Start()
    {
        if (objectTemplates == null || objectTemplates.Count == 0)
        {
            CreateDefaultObjectTemplates();
        }
        GenerateLevelObjects();
    }

    /// <summary>
    /// Generates objects using the ObjectTemplate list.
    /// This version uses the recast graph to snap the random spawn point to a walkable position.
    /// </summary>
    public void GenerateLevelObjects()
    {
        if (objectTemplates == null || objectTemplates.Count == 0)
        {
            Debug.LogWarning("No object templates available for level generation.");
            return;
        }
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Choose a random template.
            ObjectTemplate template = objectTemplates[Random.Range(0, objectTemplates.Count)];
            // Get a random point within the defined spawn area.
            Vector3 randomPoint = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                0,
                Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
            );
            // Snap the random point onto the recast graph.
            NNInfo nearest = AstarPath.active.GetNearest(randomPoint);
            Vector3 spawnPos = nearest.position;
            GameObject obj = Instantiate(template.prefab, spawnPos, Quaternion.identity);
            obj.tag = "LevelObject"; // Ensure the spawned object is tagged appropriately.
        }
    }

    /// <summary>
    /// Creates default ObjectTemplate instances in code if none have been provided.
    /// Assumes that corresponding prefabs exist in a Resources folder.
    /// </summary>
    void CreateDefaultObjectTemplates()
    {
        objectTemplates = new List<ObjectTemplate>();

        ObjectTemplate resourceTemplate = ScriptableObject.CreateInstance<ObjectTemplate>();
        resourceTemplate.templateName = "Food Resource";
        resourceTemplate.category = "Resource";
        resourceTemplate.prefab = Resources.Load<GameObject>("FoodPrefab");
        if (resourceTemplate.prefab != null)
        {
            objectTemplates.Add(resourceTemplate);
            Debug.Log("Default ObjectTemplate created: " + resourceTemplate.templateName);
        }
        else
        {
            Debug.LogWarning("Default FoodPrefab not found in Resources.");
        }

        ObjectTemplate workTemplate = ScriptableObject.CreateInstance<ObjectTemplate>();
        workTemplate.templateName = "Workstation";
        workTemplate.category = "Work";
        workTemplate.prefab = Resources.Load<GameObject>("WorkstationPrefab");
        if (workTemplate.prefab != null)
        {
            objectTemplates.Add(workTemplate);
            Debug.Log("Default ObjectTemplate created: " + workTemplate.templateName);
        }
        else
        {
            Debug.LogWarning("Default WorkstationPrefab not found in Resources.");
        }

        ObjectTemplate socialTemplate = ScriptableObject.CreateInstance<ObjectTemplate>();
        socialTemplate.templateName = "Park Bench";
        socialTemplate.category = "Social";
        socialTemplate.prefab = Resources.Load<GameObject>("BenchPrefab");
        if (socialTemplate.prefab != null)
        {
            objectTemplates.Add(socialTemplate);
            Debug.Log("Default ObjectTemplate created: " + socialTemplate.templateName);
        }
        else
        {
            Debug.LogWarning("Default BenchPrefab not found in Resources.");
        }
    }

    /// <summary>
    /// Searches for an active RecastGraph in the scene, then copies its forced bounds to spawnAreaCenter and spawnAreaSize.
    /// </summary>
    public void ScanAndCopyRecastGraphArea()
    {
        RecastGraph recastGraph = null;
        foreach (var graph in AstarPath.active.data.graphs)
        {
            recastGraph = graph as RecastGraph;
            if (recastGraph != null)
                break;
        }
        if (recastGraph != null)
        {
            // Build a Bounds instance from forcedBoundsCenter and forcedBoundsSize.
            Bounds bounds = new Bounds(recastGraph.forcedBoundsCenter, recastGraph.forcedBoundsSize);
            spawnAreaCenter = bounds.center;
            spawnAreaSize = bounds.size;
            Debug.Log("Copied recast graph area: Center=" + spawnAreaCenter + ", Size=" + spawnAreaSize);
        }
        else
        {
            Debug.LogWarning("No recast graph found.");
        }
    }

    /// <summary>
    /// Visualizes the spawn area in the Scene view.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
