using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Pathfinding; // For A* calls

public class DebugPanel : MonoBehaviour
{
    // Panel toggle.
    public bool showDebugPanel = true;

    // Top-level mode selection: Generation, NPC, Faction.
    private enum DebugMode { Generation, NPC, Faction }
    private DebugMode currentMode = DebugMode.Generation;
    private readonly string[] modeNames = new string[] { "Generation", "NPC", "Faction" };

    // --- Generation Mode Sub-Tabs ---
    private enum GenerationSubTab { NPCGeneration, FactionGeneration, LevelGeneration }
    private GenerationSubTab currentGenSubTab = GenerationSubTab.NPCGeneration;

    // Generation mode parameters.
    [Header("Generation Parameters")]
    public int npcSpawnCount = 10;
    public int familyCount = 2;
    public int familySize = 4;
    public int familyGenerations = 2;
    public int businessSpawnCount = 1; // for faction/business generation
    public int levelSpawnCount = 5;
    public int selectedTemplateIndex = 0;
    public string despawnFilter = "";

    // Faction Generation adjustable parameters.
    [Header("Faction Generation Parameters")]
    public float factionCompatibilityThreshold = 0.7f;
    public float ideologyFreedom = 0.5f;
    public float ideologyPrivacy = 0.5f;
    public float ideologyAuthority = 0.5f;
    public float ideologyEquality = 0.5f;
    public string customFactionName = "";
    public string customBusinessName = "";

    // Industry and Business Template selections.
    public int selectedIndustryIndex = 0;
    public int selectedBusinessTemplateIndex = 0;

    // --- NPC Mode Variables ---
    private Vector2 npcListScrollPos = Vector2.zero;
    private int selectedNPCIndex = 0;
    private readonly string[] npcDetailTabNames = new string[] { "Core", "Scheduling", "Navigation", "Decision", "Perception", "Relationships", "Skills", "Family/Genetics", "Appearance", "Interactions" };
    private int npcDetailTabIndex = 0;

    // --- Faction Mode Variables ---
    private Vector2 factionListScrollPos = Vector2.zero;
    private int selectedFactionIndex = 0;
    private readonly string[] factionDetailTabNames = new string[] { "Overview", "Members", "Hierarchy", "Ideology" };
    private int factionDetailTabIndex = 0;

    // Cached manager references.
    private NPCManager npcManager;
    private EconomicManager economicManager;
    private LevelGenerator levelGenerator;
    private FactionGenerator factionGenerator;
    private NPCSpawner npcSpawner;
    private FactionManager factionManager;

    // For Level Generation details.
    private Vector2 levelGenScrollPos = Vector2.zero;

    // For industry and business template selections.
    private List<Industry> industries = new List<Industry>();

    // Class-level scroll position for Generation mode right pane.
    private Vector2 genScrollPos = Vector2.zero;

    void Start()
    {
        npcManager = NPCManager.Instance;
        economicManager = FindObjectOfType<EconomicManager>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        factionGenerator = FindObjectOfType<FactionGenerator>();
        npcSpawner = FindObjectOfType<NPCSpawner>();
        factionManager = FindObjectOfType<FactionManager>();

        if (factionGenerator != null && factionGenerator.industries != null)
            industries = factionGenerator.industries;
    }

    void OnGUI()
    {
        if (!showDebugPanel)
            return;

        // Outer panel with a slightly transparent background.
        Color originalColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.8f);
        GUI.Box(new Rect(10, 10, 950, 650), "");
        GUI.color = originalColor;
        GUI.Box(new Rect(10, 10, 950, 650), "Debug Panel");

        // Top-level mode toolbar.
        currentMode = (DebugMode)GUI.Toolbar(new Rect(20, 20, 910, 30), (int)currentMode, modeNames);

        // Get current NPC list.
        List<NPC> npcs = npcManager.GetAllNPCs();
        if (npcs.Count == 0)
            currentMode = DebugMode.Generation; // Force generation if no NPCs exist.

        // Layout by mode.
        switch (currentMode)
        {
            case DebugMode.Generation:
                DrawGenerationMode();
                break;
            case DebugMode.NPC:
                DrawNPCMode(npcs);
                break;
            case DebugMode.Faction:
                DrawFactionMode();
                break;
        }

        // Toggle button.
        if (GUI.Button(new Rect(10, 630, 150, 30), "Toggle Debug Panel"))
            showDebugPanel = !showDebugPanel;
    }

    #region Generation Mode

    void DrawGenerationMode()
    {
        float leftPaneWidth = 150;
        float panelX = 20;
        float panelY = 60;
        float panelWidth = 910;
        float panelHeight = 570;

        // Left Pane: Sub-tab buttons.
        GUILayout.BeginArea(new Rect(panelX, panelY, leftPaneWidth, panelHeight));
        GUILayout.Label("Gen Options", EditorStyles.boldLabel);
        if (GUILayout.Button("NPC Generation", GUILayout.Height(30)))
            currentGenSubTab = GenerationSubTab.NPCGeneration;
        if (GUILayout.Button("Faction Generation", GUILayout.Height(30)))
            currentGenSubTab = GenerationSubTab.FactionGeneration;
        if (GUILayout.Button("Level Generation", GUILayout.Height(30)))
            currentGenSubTab = GenerationSubTab.LevelGeneration;
        GUILayout.EndArea();

        // Right Pane: Detailed controls for the selected Generation sub-tab.
        float rightPaneX = panelX + leftPaneWidth + 10;
        float rightPaneWidth = panelWidth - leftPaneWidth - 10;
        GUILayout.BeginArea(new Rect(rightPaneX, panelY, rightPaneWidth, panelHeight));
        genScrollPos = GUILayout.BeginScrollView(genScrollPos, GUILayout.Width(rightPaneWidth), GUILayout.Height(panelHeight));
        switch (currentGenSubTab)
        {
            case GenerationSubTab.NPCGeneration:
                DrawNPCGenerationTab();
                break;
            case GenerationSubTab.FactionGeneration:
                DrawFactionGenerationTab();
                break;
            case GenerationSubTab.LevelGeneration:
                DrawLevelGenerationTab();
                break;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    void DrawNPCGenerationTab()
    {
        GUILayout.Label("NPC Generation", EditorStyles.boldLabel);
        GUILayout.Label("Spawn Individuals", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Count:", GUILayout.Width(60));
        string npcCountStr = GUILayout.TextField(npcSpawnCount.ToString(), GUILayout.Width(50));
        int.TryParse(npcCountStr, out npcSpawnCount);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Spawn NPCs", GUILayout.Height(30)))
        {
            if (npcSpawner != null)
            {
                for (int i = 0; i < npcSpawnCount; i++)
                    npcSpawner.SpawnNPC();
            }
            else Debug.LogWarning("NPCSpawner not found!");
        }
        GUILayout.Space(10);
        GUILayout.Label("Spawn Family Trees", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Families:", GUILayout.Width(70));
        string famCountStr = GUILayout.TextField(familyCount.ToString(), GUILayout.Width(50));
        int.TryParse(famCountStr, out familyCount);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Family Size:", GUILayout.Width(70));
        string famSizeStr = GUILayout.TextField(familySize.ToString(), GUILayout.Width(50));
        int.TryParse(famSizeStr, out familySize);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Generations:", GUILayout.Width(70));
        string famGenStr = GUILayout.TextField(familyGenerations.ToString(), GUILayout.Width(50));
        int.TryParse(famGenStr, out familyGenerations);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Spawn Family Trees", GUILayout.Height(30)))
        {
            if (npcSpawner != null)
                npcSpawner.SpawnFamilyTree(familySize, familyGenerations);
            else Debug.LogWarning("NPCSpawner not found!");
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Despawn All NPCs", GUILayout.Height(30)))
        {
            NPC[] npcs = GameObject.FindObjectsOfType<NPC>();
            foreach (NPC npc in npcs)
                Destroy(npc.gameObject);
        }
    }

    void DrawFactionGenerationTab()
    {
        GUILayout.Label("Faction & Business Generation", EditorStyles.boldLabel);
        GUILayout.Label("Faction Parameters", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Compat Threshold:", GUILayout.Width(130));
        string compatStr = GUILayout.TextField(factionCompatibilityThreshold.ToString("F2"), GUILayout.Width(50));
        float.TryParse(compatStr, out factionCompatibilityThreshold);
        GUILayout.EndHorizontal();
        GUILayout.Label("Ideology (F, P, A, E)", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Freedom:", GUILayout.Width(60));
        string freeStr = GUILayout.TextField(ideologyFreedom.ToString("F2"), GUILayout.Width(50));
        float.TryParse(freeStr, out ideologyFreedom);
        GUILayout.Label("Privacy:", GUILayout.Width(60));
        string privStr = GUILayout.TextField(ideologyPrivacy.ToString("F2"), GUILayout.Width(50));
        float.TryParse(privStr, out ideologyPrivacy);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Authority:", GUILayout.Width(60));
        string authStr = GUILayout.TextField(ideologyAuthority.ToString("F2"), GUILayout.Width(50));
        float.TryParse(authStr, out ideologyAuthority);
        GUILayout.Label("Equality:", GUILayout.Width(60));
        string eqStr = GUILayout.TextField(ideologyEquality.ToString("F2"), GUILayout.Width(50));
        float.TryParse(eqStr, out ideologyEquality);
        GUILayout.EndHorizontal();
        GUILayout.Label("Custom Names", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Faction Name:", GUILayout.Width(100));
        customFactionName = GUILayout.TextField(customFactionName, GUILayout.Width(150));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Business Name:", GUILayout.Width(100));
        customBusinessName = GUILayout.TextField(customBusinessName, GUILayout.Width(150));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        if (industries != null && industries.Count > 0)
        {
            List<string> industryNames = new List<string>();
            foreach (Industry ind in industries)
                industryNames.Add(ind.industryName);
            selectedIndustryIndex = EditorGUILayout.Popup("Industry", selectedIndustryIndex, industryNames.ToArray());

            Industry selectedIndustry = industries[selectedIndustryIndex];
            if (selectedIndustry.businessTemplates != null && selectedIndustry.businessTemplates.Count > 0)
            {
                List<string> templateNames = new List<string>();
                foreach (BusinessTemplate bt in selectedIndustry.businessTemplates)
                    templateNames.Add(bt.templateName);
                selectedBusinessTemplateIndex = EditorGUILayout.Popup("Business Template", selectedBusinessTemplateIndex, templateNames.ToArray());
            }
        }
        else
        {
            GUILayout.Label("No industries defined.");
        }
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn with Parameters", GUILayout.Height(30)))
        {
            string factionName = string.IsNullOrEmpty(customFactionName) ? "Faction_" + Random.Range(1000, 9999) : customFactionName;
            string businessName = string.IsNullOrEmpty(customBusinessName) ? "Business_" + Random.Range(1000, 9999) : customBusinessName;

            if (factionGenerator != null)
            {
                factionGenerator.GenerateBusinessFactionWithParameters(factionName, businessName, factionCompatibilityThreshold,
                    ideologyFreedom, ideologyPrivacy, ideologyAuthority, ideologyEquality,
                    industries[selectedIndustryIndex], industries[selectedIndustryIndex].businessTemplates[selectedBusinessTemplateIndex]);
            }
            else
            {
                Debug.LogWarning("FactionGenerator not found!");
            }
        }
        if (GUILayout.Button("Spawn Random", GUILayout.Height(30)))
        {
            if (factionGenerator != null)
            {
                factionGenerator.GenerateBusinessFaction();
            }
            else Debug.LogWarning("FactionGenerator not found!");
        }
        GUILayout.EndHorizontal();
    }

    void DrawLevelGenerationTab()
    {
        GUILayout.Label("Level Generation", EditorStyles.boldLabel);
        if (levelGenerator != null && levelGenerator.objectTemplates != null && levelGenerator.objectTemplates.Count > 0)
        {
            List<string> templateNames = new List<string>();
            foreach (var template in levelGenerator.objectTemplates)
                templateNames.Add(template.templateName);
            selectedTemplateIndex = EditorGUILayout.Popup("Template", selectedTemplateIndex, templateNames.ToArray());
        }
        else
        {
            GUILayout.Label("No ObjectTemplates available.");
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawn Count:", GUILayout.Width(90));
        string levelCountStr = GUILayout.TextField(levelSpawnCount.ToString(), GUILayout.Width(50));
        int.TryParse(levelCountStr, out levelSpawnCount);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Spawn Random Objects", GUILayout.Height(30)))
        {
            if (levelGenerator != null)
            {
                for (int i = 0; i < levelSpawnCount; i++)
                    levelGenerator.GenerateLevelObjects();
            }
        }
        if (GUILayout.Button("Spawn Selected Template", GUILayout.Height(30)))
        {
            if (levelGenerator != null && levelGenerator.objectTemplates != null && levelGenerator.objectTemplates.Count > 0)
            {
                ObjectTemplate selectedTemplate = levelGenerator.objectTemplates[selectedTemplateIndex];
                for (int i = 0; i < levelSpawnCount; i++)
                {
                    Vector3 randomPoint = levelGenerator.spawnAreaCenter + new Vector3(
                        Random.Range(-levelGenerator.spawnAreaSize.x / 2f, levelGenerator.spawnAreaSize.x / 2f),
                        0,
                        Random.Range(-levelGenerator.spawnAreaSize.z / 2f, levelGenerator.spawnAreaSize.z / 2f)
                    );
                    NNInfo nearest = AstarPath.active.GetNearest(randomPoint);
                    Vector3 spawnPos = nearest.position;
                    GameObject obj = Instantiate(selectedTemplate.prefab, spawnPos, Quaternion.identity);
                    obj.tag = "LevelObject";
                }
            }
        }
        if (GUILayout.Button("Despawn All Level Objects", GUILayout.Height(30)))
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("LevelObject");
            foreach (GameObject obj in objs)
                Destroy(obj);
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Despawn by Name:", GUILayout.Width(110));
        despawnFilter = GUILayout.TextField(despawnFilter, GUILayout.Width(100));
        if (GUILayout.Button("Despawn Selected", GUILayout.Height(30)))
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("LevelObject");
            foreach (GameObject obj in objs)
            {
                if (obj.name.Contains(despawnFilter))
                    Destroy(obj);
            }
        }
        GUILayout.EndHorizontal();
    }

    #endregion

    #region NPC Mode

    void DrawNPCMode(List<NPC> npcs)
    {
        if (npcs == null || npcs.Count == 0)
        {
            GUILayout.Label("No NPCs available.");
            return;
        }

        selectedNPCIndex = Mathf.Clamp(selectedNPCIndex, 0, npcs.Count - 1);

        GUILayout.BeginArea(new Rect(20, 60, 200, 580));
        GUILayout.Label("NPC List", EditorStyles.boldLabel);
        npcListScrollPos = GUILayout.BeginScrollView(npcListScrollPos, GUILayout.Width(200), GUILayout.Height(580));
        for (int i = 0; i < npcs.Count; i++)
        {
            if (GUILayout.Button(npcs[i].identity.npcName, GUILayout.Height(30)))
            {
                selectedNPCIndex = i;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(230, 60, 700, 580));
        GUILayout.Label("NPC Details - " + npcs[selectedNPCIndex].identity.npcName, EditorStyles.boldLabel);
        npcDetailTabIndex = GUILayout.Toolbar(npcDetailTabIndex, npcDetailTabNames);
        GUILayout.Space(5);
        Vector2 npcDetailScroll = GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(700), GUILayout.Height(540));
        DrawNPCDetailTab(npcs[selectedNPCIndex], npcDetailTabIndex);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    void DrawNPCDetailTab(NPC npc, int tabIndex)
    {
        switch (tabIndex)
        {
            case 0: // Core
                GUILayout.Label("Core Info", EditorStyles.boldLabel);
                GUILayout.Label("Name: " + npc.identity.npcName);
                GUILayout.Label("Age: " + npc.identity.age);
                GUILayout.Label("Gender: " + npc.identity.gender);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Skin:", GUILayout.Width(50));
                GUI.color = npc.physicalAppearance.characteristics.skinColor;
                GUILayout.Box("", GUILayout.Width(20), GUILayout.Height(20));
                GUI.color = Color.white;
                GUILayout.Label("Hair:", GUILayout.Width(50));
                GUI.color = npc.physicalAppearance.characteristics.hairColor;
                GUILayout.Box("", GUILayout.Width(20), GUILayout.Height(20));
                GUI.color = Color.white;
                GUILayout.Label("Eye:", GUILayout.Width(50));
                GUI.color = npc.physicalAppearance.characteristics.eyeColor;
                GUILayout.Box("", GUILayout.Width(20), GUILayout.Height(20));
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Hap: " + npc.personality.defaultHappiness.ToString("F2"), GUILayout.Width(100));
                GUILayout.Label("Pas: " + npc.personality.defaultPassion.ToString("F2"), GUILayout.Width(100));
                GUILayout.Label("Con: " + npc.personality.defaultConfidence.ToString("F2"), GUILayout.Width(100));
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.Label("Traits:", EditorStyles.boldLabel);
                foreach (Trait trait in npc.traits)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(trait.traitName, GUILayout.Width(100));
                    GUILayout.Label(trait.category, GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                }
                break;
            case 1: // Scheduling
                GUILayout.Label("Scheduling", EditorStyles.boldLabel);
                NPCScheduling sched = npc.GetComponent<NPCScheduling>();
                if (sched != null)
                {
                    GUILayout.Label("Current Task: " + sched.currentTask);
                    GUILayout.Label("Destination: " + sched.GetDestination().ToString("F1"));
                    if (GUILayout.Button("Refresh Schedule", GUILayout.Height(25)))
                    {
                        // Optionally trigger schedule refresh.
                    }
                }
                break;
            case 2: // Navigation
                GUILayout.Label("Navigation", EditorStyles.boldLabel);
                NavigationController nav = npc.GetComponent<NavigationController>();
                if (nav != null)
                {
                    GUILayout.Label("Arrived: " + nav.HasArrived());
                    if (GUILayout.Button("Show Path", GUILayout.Height(25)))
                    {
                        // Optionally trigger path visualization.
                    }
                }
                break;
            case 3: // Decision
                GUILayout.Label("Decision Making", EditorStyles.boldLabel);
                DecisionMakerStateMachine dmsm = npc.GetComponent<DecisionMakerStateMachine>();
                if (dmsm != null)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label("Current State: " + dmsm.CurrentState.stateName);
                    GUILayout.Label("State Timer: " + dmsm.StateTimer.ToString("F2") + " sec");

                    // Retrieve allowed transitions from the state machine.
                    List<StateDefinition> allowedTransitions = dmsm.GetAllowedTransitions();
                    // Create a list of state names.
                    List<string> transitionNames = new List<string>();
                    foreach (StateDefinition stateDef in allowedTransitions)
                    {
                        transitionNames.Add(stateDef.stateName);
                    }
                    // Now join the names with a comma.
                    GUILayout.Label("Allowed Transitions: " + string.Join(", ", transitionNames.ToArray()));

                    GUILayout.EndVertical();
                    GUILayout.Space(5);
                }
                DecisionMaker dm = npc.GetComponent<DecisionMaker>();
                if (dm != null)
                {
                    foreach (NPCAction action in dm.actions)
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label("Action: " + action.actionName, GUILayout.Width(150));
                        GUILayout.Label("Utility: " + action.GetUtility(npc).ToString("F2"), GUILayout.Width(100));
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Refresh Decisions", GUILayout.Height(25)))
                    {
                        // Optionally refresh decisions.
                    }
                }
                break;
            case 4: // Perception
                GUILayout.Label("Perception", EditorStyles.boldLabel);
                PerceptionSystem ps = npc.GetComponent<PerceptionSystem>();
                if (ps != null)
                {
                    GUILayout.Label("Objects Perceived: " + ps.perceivedObjects.Count);
                    foreach (GameObject obj in ps.perceivedObjects)
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label("Object: " + obj.name, GUILayout.Width(150));
                        GUILayout.Label("Vision: " + ps.GetVisionScore(obj).ToString("F2"), GUILayout.Width(80));
                        GUILayout.Label("Audio: " + ps.GetAudioScore(obj).ToString("F2"), GUILayout.Width(80));
                        GUILayout.Label("Combined: " + ps.GetCombinedPerceptionScore(obj).ToString("F2"), GUILayout.Width(80));
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            case 5: // Relationships
                GUILayout.Label("Relationships", EditorStyles.boldLabel);
                RelationshipSystem rs = npc.GetComponent<RelationshipSystem>();
                if (rs != null && rs.relationshipInfo.Count > 0)
                {
                    foreach (var rel in rs.relationshipInfo)
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label("With: " + rel.Key.identity.npcName, GUILayout.Width(150));
                        GUILayout.Label("Cat: " + rel.Value.category.ToString(), GUILayout.Width(100));
                        GUILayout.Label("Affil: " + rel.Value.affiliationLabel, GUILayout.Width(100));
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Refresh Relationships", GUILayout.Height(25)))
                    {
                        // Optionally refresh relationships.
                    }
                }
                break;
            case 6: // Skills
                GUILayout.Label("Skills", EditorStyles.boldLabel);
                NPCSkills skills = npc.GetComponent<NPCSkills>();
                if (skills != null)
                {
                    foreach (Skill s in skills.skills)
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label(s.skillName, GUILayout.Width(150));
                        GUILayout.Label("Level: " + s.level.ToString("F2"), GUILayout.Width(100));
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            case 7: // Family/Genetics
                GUILayout.Label("Family & Genetics", EditorStyles.boldLabel);
                NPCFamilyManager fam = npc.familyManager;
                NPCGenetics genes = npc.npcGenetics;
                if (fam != null)
                {
                    GUILayout.Label("Parents: " + (fam.parents != null ? fam.parents.Count.ToString() : "0"));
                    GUILayout.Label("Children: " + (fam.children != null ? fam.children.Count.ToString() : "0"));
                }
                if (genes != null)
                {
                    GUILayout.Label("Genetic ID: " + genes.geneticID);
                    GUILayout.Label("Sight Multiplier: " + genes.sightMultiplier.ToString("F2"));
                    GUILayout.Label("Hearing Multiplier: " + genes.hearingMultiplier.ToString("F2"));
                }
                break;
            case 8: // Appearance
                GUILayout.Label("Physical Appearance", EditorStyles.boldLabel);
                GUILayout.Label("Basic Appearance:");
                GUILayout.Label("Height: " + npc.physicalAppearance.characteristics.height.ToString("F2") + " m");
                GUILayout.Label("Weight: " + npc.physicalAppearance.characteristics.weight.ToString("F2") + " kg");
                GUILayout.Space(5);
                GUILayout.Label("Body Part Features:", EditorStyles.boldLabel);
                foreach (var feature in npc.physicalAppearance.bodyPartFeatures)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("Part: " + feature.bodyPart.ToString(), GUILayout.Width(100));
                    GUILayout.Label("Size: " + feature.size.ToString(), GUILayout.Width(80));
                    GUILayout.Label("Strength: " + feature.strength.ToString(), GUILayout.Width(80));
                    GUILayout.Label("Dexterity: " + feature.dexterity.ToString(), GUILayout.Width(80));
                    GUILayout.Label("Aesthetic: " + feature.aesthetic.ToString(), GUILayout.Width(80));
                    GUILayout.EndHorizontal();
                }
                break;
            case 9: // Interactions
                GUILayout.Label("Interactions", EditorStyles.boldLabel);
                NPCInteractionManager im = npc.GetComponent<NPCInteractionManager>();
                if (im != null)
                {
                    List<NPC> compatibleNPCs = im.GetSexuallyCompatibleNPCs(npc);
                    GUILayout.Label("Sexually Compatible NPCs: " + (compatibleNPCs != null ? compatibleNPCs.Count.ToString() : "0"));
                    foreach (NPC other in compatibleNPCs)
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label(other.identity.npcName, GUILayout.Width(150));
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Refresh Interactions", GUILayout.Height(25)))
                    {
                        // Optionally refresh interactions.
                    }
                }
                break;
        }
        GUILayout.EndScrollView();
    }

    #endregion

    #region Faction Mode

    void DrawFactionMode()
    {
        GUILayout.BeginArea(new Rect(20, 60, 200, 580));
        GUILayout.Label("Faction List", EditorStyles.boldLabel);
        factionListScrollPos = GUILayout.BeginScrollView(factionListScrollPos, GUILayout.Width(200), GUILayout.Height(580));
        List<Faction> factions = factionManager.allFactions;
        for (int i = 0; i < factions.Count; i++)
        {
            if (GUILayout.Button(factions[i].factionName, GUILayout.Height(30)))
                selectedFactionIndex = i;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(230, 60, 700, 580));
        if (factions.Count > 0)
        {
            Faction selectedFaction = factions[selectedFactionIndex];
            GUILayout.Label("Faction Details - " + selectedFaction.factionName, EditorStyles.boldLabel);
            factionDetailTabIndex = GUILayout.Toolbar(factionDetailTabIndex, factionDetailTabNames);
            GUILayout.Space(5);
            Vector2 factionDetailScroll = GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(700), GUILayout.Height(540));
            DrawFactionDetailTab(selectedFaction, factionDetailTabIndex);
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No factions available.");
        }
        GUILayout.EndArea();
    }

    void DrawFactionDetailTab(Faction faction, int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                GUILayout.Label("Overview", EditorStyles.boldLabel);
                GUILayout.Label("Name: " + faction.factionName);
                GUILayout.Label("Description: " + faction.description);
                GUILayout.Label("Type: " + faction.factionType.ToString());
                GUILayout.Label("Compatibility Threshold: " + faction.compatibilityThreshold.ToString("F2"));
                break;
            case 1:
                GUILayout.Label("Members", EditorStyles.boldLabel);
                if (faction.members != null)
                {
                    foreach (NPC npc in faction.members)
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label(npc.identity.npcName, GUILayout.Width(150));
                        GUILayout.EndHorizontal();
                    }
                }
                break;
            case 2:
                GUILayout.Label("Hierarchy", EditorStyles.boldLabel);
                if (faction.parentFaction != null)
                    GUILayout.Label("Parent: " + faction.parentFaction.factionName);
                if (faction.subFactions != null && faction.subFactions.Count > 0)
                {
                    GUILayout.Label("Sub-Factions:");
                    foreach (Faction sub in faction.subFactions)
                        GUILayout.Label("- " + sub.factionName);
                }
                break;
            case 3:
                GUILayout.Label("Ideological Profile", EditorStyles.boldLabel);
                GUILayout.Label("Freedom: " + faction.ideology.freedom.ToString("F2"));
                GUILayout.Label("Privacy: " + faction.ideology.privacy.ToString("F2"));
                GUILayout.Label("Authority: " + faction.ideology.authority.ToString("F2"));
                GUILayout.Label("Equality: " + faction.ideology.equality.ToString("F2"));
                break;
        }
    }

    #endregion
}
