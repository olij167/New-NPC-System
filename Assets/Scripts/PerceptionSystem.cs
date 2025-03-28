using System.Collections.Generic;
using UnityEngine;

public class PerceptionSystem : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Maximum distance the NPC can see.")]
    public float viewDistance = 15f;
    [Tooltip("Field of view in degrees (e.g., 90 means 45° to each side).")]
    public float fieldOfView = 90f;
    [Tooltip("Layer mask for objects visible by sight.")]
    public LayerMask visionMask;
    [Tooltip("Multiplier for the NPC's sight sensitivity.")]
    public float sightStrength = 1.0f;

    [Header("Audio Settings")]
    [Tooltip("Maximum distance the NPC can hear.")]
    public float hearingRadius = 20f;
    [Tooltip("Layer mask for objects that emit sound.")]
    public LayerMask audioMask;
    [Tooltip("Multiplier for the NPC's hearing sensitivity.")]
    public float hearingStrength = 1.0f;

    [Header("Perceived Objects")]
    [Tooltip("List of objects currently visible.")]
    public List<GameObject> visibleObjects = new List<GameObject>();
    [Tooltip("List of objects currently audible.")]
    public List<GameObject> audibleObjects = new List<GameObject>();

    /// <summary>
    /// A property that returns all perceived objects (visible and audible) as a combined list.
    /// </summary>
    public List<GameObject> perceivedObjects
    {
        get
        {
            HashSet<GameObject> union = new HashSet<GameObject>(visibleObjects);
            union.UnionWith(audibleObjects);
            return new List<GameObject>(union);
        }
    }

    void Update()
    {
        UpdateVision();
        UpdateAudio();
    }

    /// <summary>
    /// Updates the list of visible objects using a sphere overlap and a vision cone with occlusion checks.
    /// </summary>
    void UpdateVision()
    {
        visibleObjects.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance, visionMask);
        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject)
                continue;

            Vector3 directionToObj = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToObj);
            if (angle < fieldOfView * 0.5f)
            {
                // Optional occlusion check.
                Ray ray = new Ray(transform.position, col.transform.position - transform.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, viewDistance, visionMask))
                {
                    if (hit.collider.gameObject == col.gameObject)
                    {
                        visibleObjects.Add(col.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the list of audible objects using a sphere overlap and filtering by the presence of a SoundSource component.
    /// </summary>
    void UpdateAudio()
    {
        audibleObjects.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, hearingRadius, audioMask);
        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject)
                continue;
            if (col.GetComponent<SoundSource>() != null)
                audibleObjects.Add(col.gameObject);
        }
    }

    /// <summary>
    /// Computes a vision score for an object based on distance and the angle relative to the NPC's forward direction.
    /// </summary>
    public float GetVisionScore(GameObject obj)
    {
        Vector3 toObj = obj.transform.position - transform.position;
        float distance = toObj.magnitude;
        float distanceScore = 1f / Mathf.Max(distance, 0.1f);
        float angle = Vector3.Angle(transform.forward, toObj.normalized);
        float angleScore = 1f - Mathf.Clamp01(angle / (fieldOfView * 0.5f));
        return sightStrength * distanceScore * angleScore;
    }

    /// <summary>
    /// Computes an audio score for an object based on its SoundSource loudness and its distance from the NPC.
    /// </summary>
    public float GetAudioScore(GameObject obj)
    {
        SoundSource sound = obj.GetComponent<SoundSource>();
        if (sound == null)
            return 0f;
        float distance = Vector3.Distance(transform.position, obj.transform.position);
        return hearingStrength * sound.loudness / Mathf.Max(distance, 0.1f);
    }

    /// <summary>
    /// Computes a combined perception score for an object using weighted vision and audio scores.
    /// </summary>
    public float GetCombinedPerceptionScore(GameObject obj, float visionWeight = 0.6f, float audioWeight = 0.4f)
    {
        float vScore = GetVisionScore(obj);
        float aScore = GetAudioScore(obj);
        return visionWeight * vScore + audioWeight * aScore;
    }

    /// <summary>
    /// Provides an alias for GetCombinedPerceptionScore.
    /// This allows legacy code to call GetAttentionScore.
    /// </summary>
    public float GetAttentionScore(GameObject obj)
    {
        return GetCombinedPerceptionScore(obj);
    }

    /// <summary>
    /// Returns a list of all perceived objects sorted by their combined perception score (highest first).
    /// </summary>
    public List<GameObject> GetPrioritizedPerceivedObjects()
    {
        List<GameObject> combinedList = perceivedObjects;
        combinedList.Sort((a, b) => GetCombinedPerceptionScore(b).CompareTo(GetCombinedPerceptionScore(a)));
        return combinedList;
    }
}
