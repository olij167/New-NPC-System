using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPC : MonoBehaviour
{



    // Existing components and systems
    public NPCIdentity identity;
    public Personality personality;
    public Sexuality sexuality;
    public MemorySystem memorySystem;
    public RelationshipSystem relationshipSystem;
    public PerceptionSystem perceptionSystem;
    public NeedsSystem needsSystem;
    public EmotionSystem emotionSystem;
    public DecisionMaker decisionMaker;
    public NPCPhysicalAppearance physicalAppearance;
    public NPCSkills npcSkills;
    public NPCFamilyManager familyManager;
    public FactionMembership factionMembership;
    public NPCGenetics npcGenetics;
    public NPCInteractionManager interactionManager;
    public DecisionMakerStateMachine stateMachine;
    public List<Trait> traits = new List<Trait>();

    // --- New: Lists for colourable objects ---
    [Header("Colourable Object References")]
    public List<GameObject> hairObjects = new List<GameObject>();
    public List<GameObject> eyeObjects = new List<GameObject>();
    public List<GameObject> skinObjects = new List<GameObject>();
    public List<GameObject> bodyObjects = new List<GameObject>();

    // --- New: Hairstyle selection ---
    [Header("Hairstyle Settings")]
    [Tooltip("List of potential hairstyle prefabs. Each prefab may include a HairstyleOffset component for relative positioning.")]
    public List<GameObject> hairstylePrefabs = new List<GameObject>();
    [Tooltip("Transform reference for the head where the hairstyle will be attached.")]
    public Transform headTransform;
    [Tooltip("Default local position offset if the prefab does not specify one.")]
    public Vector3 defaultHairstylePositionOffset = Vector3.zero;
    [Tooltip("Default local rotation offset (in Euler angles) if the prefab does not specify one.")]
    public Vector3 defaultHairstyleRotationOffset = Vector3.zero;

    void Awake()
    {
        InitialiseComponents();
        InitializeIdentity();
        InitializeTraits();
        ApplyColoursToObjects();
        ApplyRandomHairstyle();
        GlobalEventManager.Instance?.RaiseNPCSpawned(this);
    }

    T EnsureComponent<T>() where T : Component
    {
        T comp = GetComponent<T>();
        if (comp == null)
        {
            comp = gameObject.AddComponent<T>();
        }
        return comp;
    }

    public void InitialiseComponents()
    {
        identity = EnsureComponent<NPCIdentity>();
        personality = EnsureComponent<Personality>();
        sexuality = EnsureComponent<Sexuality>();
        memorySystem = EnsureComponent<MemorySystem>();
        relationshipSystem = EnsureComponent<RelationshipSystem>();
        perceptionSystem = EnsureComponent<PerceptionSystem>();
        needsSystem = EnsureComponent<NeedsSystem>();
        emotionSystem = EnsureComponent<EmotionSystem>();
        decisionMaker = EnsureComponent<DecisionMaker>();
        physicalAppearance = EnsureComponent<NPCPhysicalAppearance>();
        npcSkills = EnsureComponent<NPCSkills>();
        familyManager = EnsureComponent<NPCFamilyManager>();
        factionMembership = EnsureComponent<FactionMembership>();
        npcGenetics = EnsureComponent<NPCGenetics>();
        interactionManager = EnsureComponent<NPCInteractionManager>();
        stateMachine = EnsureComponent<DecisionMakerStateMachine>();
    }

    public void TriggerEvent(GameEventData eventData)
    {
        if (memorySystem != null)
        {
            memorySystem.RecordMemory(eventData, 1.0f);
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == gameObject)
                Debug.Log("[NPC] " + identity.npcName + " triggered event: " + eventData.description);
#endif
        }
    }


    public void InitializeIdentity()
    {
        identity.GenerateIdentity();
        if (familyManager != null && familyManager.parents != null && familyManager.parents.Count > 0)
        {
            if (familyManager.parents.Count >= 2)
            {
                NPC parentA = familyManager.parents[0].GetComponent<NPC>();
                NPC parentB = familyManager.parents[1].GetComponent<NPC>();
                if (parentA != null && parentB != null)
                {
                    physicalAppearance.InheritAppearance(parentA.physicalAppearance, parentB.physicalAppearance);
                }
                else
                {
                    physicalAppearance.RandomizeAppearance();
                }
            }
            else
            {
                NPC parent = familyManager.parents[0].GetComponent<NPC>();
                if (parent != null)
                {
                    physicalAppearance.InheritAppearance(parent.physicalAppearance, physicalAppearance);
                }
                else
                {
                    physicalAppearance.RandomizeAppearance();
                }
            }
            List<Personality> parentPersonalities = new List<Personality>();
            foreach (NPCIdentity parentId in familyManager.parents)
            {
                NPC parentNPC = parentId.GetComponent<NPC>();
                if (parentNPC != null && parentNPC.personality != null)
                    parentPersonalities.Add(parentNPC.personality);
            }
            if (parentPersonalities.Count > 0)
            {
                personality.InheritFromParents(parentPersonalities);
            }
        }
        else
        {
            physicalAppearance.RandomizeAppearance();
        }
        if (sexuality != null)
        {
            sexuality.InitializeSexuality();
        }
        if (emotionSystem != null)
        {
            emotionSystem.Initialize(personality);
        }
        if (perceptionSystem != null)
        {
            perceptionSystem.InitializePerception();
        }
        if (npcSkills != null)
        {
            npcSkills.InitializeSkills();
        }
        if (needsSystem != null)
        {
            needsSystem.InitializeNeeds();
        }
    }

    public void InitializeTraits()
    {
        if (familyManager != null && familyManager.parents != null && familyManager.parents.Count > 0)
        {
            Dictionary<string, Trait> inheritedTraits = new Dictionary<string, Trait>();
            foreach (var parentIdentity in familyManager.parents)
            {
                NPC parentNPC = parentIdentity.GetComponent<NPC>();
                if (parentNPC != null && parentNPC.traits != null)
                {
                    foreach (Trait t in parentNPC.traits)
                    {
                        if (!inheritedTraits.ContainsKey(t.traitName))
                        {
                            inheritedTraits[t.traitName] = t.Clone();
                        }
                    }
                }
            }
            foreach (var trait in inheritedTraits.Values)
            {
                if (Random.value < 0.1f)
                {
                    List<string> keys = new List<string>(trait.modifiers.Keys);
                    foreach (string key in keys)
                    {
                        trait.modifiers[key] = -trait.modifiers[key];
                    }
                }
            }
            traits = new List<Trait>(inheritedTraits.Values);
        }
        else
        {
            List<Trait> defaults = DefaultTraits.GetDefaultTraits();
            traits = new List<Trait>();
            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, defaults.Count);
                traits.Add(defaults[index].Clone());
            }
        }
    }

    /// <summary>
    /// Computes the complementary colour for the given colour by rotating its hue by 180 degrees.
    /// </summary>
    public Color GetComplementaryColor(Color original)
    {
        float h, s, v;
        Color.RGBToHSV(original, out h, out s, out v);
        h = (h + 0.5f) % 1f;
        return Color.HSVToRGB(h, s, v);
    }

    /// <summary>
    /// Applies hair, eye, and skin colours to the respective object lists.
    /// Also applies the complementary colour of the skin to the body objects.
    /// </summary>
    public void ApplyColoursToObjects()
    {
        if (physicalAppearance != null)
        {
            Color skinColor = physicalAppearance.characteristics.skinColor;
            Color hairColor = physicalAppearance.characteristics.hairColor;
            Color eyeColor = physicalAppearance.characteristics.eyeColor;
            Color complementaryColor = GetComplementaryColor(skinColor);

            foreach (GameObject obj in hairObjects)
            {
                if (obj != null)
                    SetObjectColor(obj, hairColor);
            }
            foreach (GameObject obj in eyeObjects)
            {
                if (obj != null)
                    SetObjectColor(obj, eyeColor);
            }
            foreach (GameObject obj in skinObjects)
            {
                if (obj != null)
                    SetObjectColor(obj, skinColor);
            }
            foreach (GameObject obj in bodyObjects)
            {
                if (obj != null)
                    SetObjectColor(obj, complementaryColor);
            }
        }
    }

    /// <summary>
    /// Helper method to set the material colour of a GameObject's Renderer.
    /// </summary>
    private void SetObjectColor(GameObject obj, Color color)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    /// <summary>
    /// Randomly selects and instantiates a hairstyle prefab from the list,
    /// attaches it to the headTransform, and applies local offset values.
    /// If the selected prefab contains a HairstyleOffset component, its offsets are used.
    /// </summary>
    public void ApplyRandomHairstyle()
    {
        if (hairstylePrefabs == null || hairstylePrefabs.Count == 0 || headTransform == null)
            return;

        // Select a random hairstyle prefab.
        GameObject selectedHairstylePrefab = hairstylePrefabs[Random.Range(0, hairstylePrefabs.Count)];
        if (selectedHairstylePrefab == null)
            return;

        // Instantiate the hairstyle under the head transform.
        GameObject hairstyleInstance = Instantiate(selectedHairstylePrefab, headTransform);

        // Check for a HairstyleOffset component to apply offsets.
        HairstyleOffset offsetComponent = hairstyleInstance.GetComponent<HairstyleOffset>();
        if (offsetComponent != null)
        {
            hairstyleInstance.transform.localPosition = offsetComponent.positionOffset;
            hairstyleInstance.transform.localEulerAngles = offsetComponent.rotationOffset;
        }
        else
        {
            hairstyleInstance.transform.localPosition = defaultHairstylePositionOffset;
            hairstyleInstance.transform.localEulerAngles = defaultHairstyleRotationOffset;
        }

        // Clear any previously assigned hair objects (optional, based on your design).
        hairObjects.Clear();

        // Recursively add the instantiated hairstyle and all its children to the hairObjects list.
        AddToHairListRecursive(hairstyleInstance);

        // Set the hair colour on all collected hair objects.
        Color hairColor = physicalAppearance.characteristics.hairColor;
        foreach (GameObject hairObj in hairObjects)
        {
            SetObjectColor(hairObj, hairColor);
        }
    }

    /// <summary>
    /// Recursively adds the given GameObject and all its children to the hairObjects list.
    /// </summary>
    private void AddToHairListRecursive(GameObject obj)
    {
        if (obj == null) return;
        hairObjects.Add(obj);
        foreach (Transform child in obj.transform)
        {
            AddToHairListRecursive(child.gameObject);
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // Determine if we should draw the label.
        bool isSelected = Selection.activeGameObject == gameObject;
        bool shouldDrawLabel = NPCManager.Instance.drawLabelsForEveryNPC || isSelected;
        bool shouldDrawGizmos = NPCManager.Instance.drawGizmosForEveryNPC || isSelected;

        if (!shouldDrawLabel)
            return;

        // Draw perception gizmos if needed.
        if (perceptionSystem != null && shouldDrawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, perceptionSystem.viewDistance);
        }

        // Get the NPC's name and current action.
        string npcName = (identity != null && !string.IsNullOrEmpty(identity.npcName)) ? identity.npcName : "Unnamed";
        string currentAction = (stateMachine != null) ? stateMachine.CurrentState.stateName : "None";
        string labelText = string.Format("{0}\n<color=grey><size=12>{1}</size></color>", npcName, currentAction);

        // Determine label position (above the NPC).
        Vector3 labelPosition = transform.position + Vector3.up * 3.5f;

        // Set up the style: if this NPC is selected, emphasize the text.
        GUIStyle style = new GUIStyle();
        style.richText = true;
        if (isSelected)
        {
            style.normal.textColor = Color.yellow;
            style.fontSize = 16; // Emphasized font size.
            style.fontStyle = FontStyle.Bold;
        }
        else
        {
            style.normal.textColor = Color.white;
            style.fontSize = 14;
        }

        Handles.Label(labelPosition, labelText, style);
#endif
    }

}
