using UnityEngine;
using System.Collections.Generic;

public class PerceptionSystem : MonoBehaviour
{
    [Header("Perception Settings")]
    [Tooltip("Strength of sight. Set in the Inspector or inherited via genetics.")]
    public float sightStrength = 1.0f;
    [Tooltip("Strength of hearing. Set in the Inspector or inherited via genetics.")]
    public float hearingStrength = 1.0f;

    [Header("Vision Settings")]
    [Tooltip("Maximum distance for vision.")]
    public float viewDistance = 30f;
    [Tooltip("Field of view angle (in degrees).")]
    public float fieldOfView = 90f;
    [Tooltip("Layer mask for vision.")]
    public LayerMask visionMask;
    [Tooltip("List of objects currently perceived by vision.")]
    public List<GameObject> perceivedObjects = new List<GameObject>();

    [Header("Audio Settings")]
    [Tooltip("Maximum hearing radius.")]
    public float hearingRadius = 20f;
    [Tooltip("Layer mask for audio sources.")]
    public LayerMask audioMask;
    [Tooltip("List of objects currently perceived by audio.")]
    public List<GameObject> audibleObjects = new List<GameObject>();

    /// <summary>
    /// Initializes the perception system. If the NPC has parental data, inherited perception strengths from genetics are used; otherwise, values are randomized.
    /// </summary>
    public void InitializePerception()
    {
        NPCFamilyManager fm = GetComponent<NPCFamilyManager>();
        NPCGenetics genetics = GetComponent<NPCGenetics>();

        if (fm != null && fm.parents != null && fm.parents.Count > 0 && genetics != null)
        {
            sightStrength = genetics.sightMultiplier;
            hearingStrength = genetics.hearingMultiplier;
        }
        else
        {
            sightStrength = Random.Range(0.8f, 1.2f);
            hearingStrength = Random.Range(0.8f, 1.2f);
        }
    }

    void Update()
    {
        UpdateVision();
        UpdateAudio();
    }

    /// <summary>
    /// Updates the list of visible objects using a sphere overlap, a vision cone, and an occlusion check.
    /// </summary>
    void UpdateVision()
    {
        // Clear the list of perceived objects.
        perceivedObjects.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance, visionMask);
        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject)
                continue;

            Vector3 directionToObj = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToObj);
            if (angle < fieldOfView * 0.5f)
            {
                // Perform an occlusion check.
                Ray ray = new Ray(transform.position, col.transform.position - transform.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, viewDistance, visionMask))
                {
                    if (hit.collider.gameObject == col.gameObject)
                        perceivedObjects.Add(col.gameObject);
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
    /// Alias for GetCombinedPerceptionScore to support legacy code.
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
        List<GameObject> sortedList = new List<GameObject>(perceivedObjects);
        sortedList.Sort((a, b) => GetCombinedPerceptionScore(b).CompareTo(GetCombinedPerceptionScore(a)));
        return sortedList;
    }

    // Debug visualization: Draw vision range (yellow) and hearing radius (cyan).
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, viewDistance);
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireSphere(transform.position, hearingRadius);
    //}
}
