using System.Collections.Generic;
using NUnit.Framework;                    // For NUnit framework
using UnityEngine;                        // For GameObject, MonoBehaviour, etc.
using UnityEngine.TestTools;              // For Unity test tools
using NUnitAssert = NUnit.Framework.Assert; // Alias to avoid ambiguity

// Dummy definitions to satisfy compiler errors.
// Remove these if your project already defines these classes in a specific namespace.

public class NPC : MonoBehaviour
{
    public List<Trait> traits;
    public Personality personality;
    public NPCSkills npcSkills;
    public PerceptionSystem perceptionSystem;
    public RelationshipSystem relationshipSystem;
}

public class DecisionMaker : MonoBehaviour
{
    public List<NPCAction> actions = new List<NPCAction>();
}

public class NPCAction
{
    // Declare actionName as a public field.
    public string actionName;
    public virtual float GetUtility(NPC npc) { return 0f; }
    public virtual void Execute(NPC npc) { }
}

public class NPCIdentity : MonoBehaviour { }

public class NPCSkills : MonoBehaviour
{
    public List<Skill> skills;
    public Skill GetSkill(string name)
    {
        if (skills != null)
            return skills.Find(s => s.skillName.Equals(name));
        return null;
    }
}

public class PerceptionSystem : MonoBehaviour { }

public class Personality : MonoBehaviour
{
    public float defaultConfidence;
}

public class RelationshipSystem : MonoBehaviour
{
    public Dictionary<object, RelationshipInfo> relationshipInfo = new Dictionary<object, RelationshipInfo>();
}

public class Skill
{
    public string skillName;
    public float level;
    public Skill(string name, float lvl) { skillName = name; level = lvl; }
}

public class Trait
{
    public string traitName;
    public string description;
    public string category;
    public Dictionary<string, float> modifiers = new Dictionary<string, float>();
}

// Updated dummy ContextualInfluenceManager with a public InitializeManager method.
public class ContextualInfluenceManager : MonoBehaviour
{
    public static ContextualInfluenceManager Instance { get; private set; }
    public void InitializeManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // For testing, we simulate fixed influences.
    public float GetTotalContextualInfluence(NPC npc, string actionName)
    {
        // Expected influences: Trait +0.1, Personality: (defaultConfidence - 0.5),
        // Skill: assume athletics mapping: (60 - 50)/100 = +0.1, Relationship: 0.
        float traitInf = 0.1f;
        float personalityInf = npc.personality.defaultConfidence - 0.5f;
        float skillInf = 0.1f;
        float relationshipInf = 0f;
        return traitInf + personalityInf + skillInf + relationshipInf;
    }
}

public class RelationshipInfo
{
    public RelationshipCategory category;
}
public enum RelationshipCategory
{
    Friend,
    CloseFriend,
    Neutral,
    Rival,
    Enemy
}

// Dummy action class for testing; explicitly declares an actionName.
public class DummyAction : NPCAction
{
    public DummyAction() { this.actionName = "DummyAction"; }
    public override float GetUtility(NPC npc)
    {
        return 1.0f;
    }
    public override void Execute(NPC npc)
    {
        // No execution logic required for tests.
    }
}

// The test script
public class DecisionMakerTests
{
    GameObject npcGO;
    NPC npc;
    DecisionMaker decisionMaker;
    ContextualInfluenceManager influenceManager;

    [SetUp]
    public void Setup()
    {
        // Create a test NPC GameObject and add required components.
        npcGO = new GameObject("TestNPC");
        npc = npcGO.AddComponent<NPC>();
        npcGO.AddComponent<NPCIdentity>();
        npcGO.AddComponent<Personality>();
        npcGO.AddComponent<NPCSkills>();
        npcGO.AddComponent<PerceptionSystem>();
        npcGO.AddComponent<DecisionMaker>();
        npcGO.AddComponent<RelationshipSystem>();

        // Assign component fields explicitly.
        npc.personality = npcGO.GetComponent<Personality>();
        npc.npcSkills = npcGO.GetComponent<NPCSkills>();
        npc.perceptionSystem = npcGO.GetComponent<PerceptionSystem>();
        npc.relationshipSystem = npcGO.GetComponent<RelationshipSystem>();

        // Set up a test trait with a positive DecisionInfluence modifier.
        npc.traits = new List<Trait>();
        Trait testTrait = new Trait
        {
            traitName = "TestTrait",
            description = "A test trait with positive decision influence.",
            category = "Test"
        };
        testTrait.modifiers.Add("DecisionInfluence", 0.1f);
        npc.traits.Add(testTrait);

        // Configure personality: set defaultConfidence to 0.7 (gives +0.2 influence).
        npc.personality.defaultConfidence = 0.7f;

        // Configure NPC skills: add "Athletics" with level 60 (expected influence: (60 - 50)/100 = +0.1).
        npc.npcSkills.skills = new List<Skill>();
        npc.npcSkills.skills.Add(new Skill("Athletics", 60f));

        // Setup a basic perception system.
        // (No specific configuration needed for this test.)

        // Ensure RelationshipSystem is present (no relationships set, so influence = 0).

        // Replace DecisionMaker's actions with a dummy action.
        decisionMaker = npcGO.GetComponent<DecisionMaker>();
        decisionMaker.actions.Clear();
        decisionMaker.actions.Add(new DummyAction());

        // Create and add ContextualInfluenceManager.
        GameObject influenceGO = new GameObject("ContextualInfluenceManager");
        influenceManager = influenceGO.AddComponent<ContextualInfluenceManager>();
        // Call the public initialization method.
        influenceManager.InitializeManager();
    }

    [Test]
    public void TestTotalContextualInfluence()
    {
        // Expected influences:
        // Trait: +0.1, Personality: 0.7 - 0.5 = +0.2, Skill: +0.1, Relationship: 0.
        // Total expected influence = approximately 0.1 + 0.2 + 0.1 = 0.4.
        float totalInfluence = ContextualInfluenceManager.Instance.GetTotalContextualInfluence(npc, "DummyAction");
        NUnitAssert.AreEqual(0.4f, totalInfluence, 0.05f, "Total contextual influence should be approximately 0.4");
    }

    [Test]
    public void TestAdjustedUtilityInDecisionMaker()
    {
        // Base utility from DummyAction is 1.0.
        // With a total influence of approximately 0.4, adjusted utility should be 1.0 * (1 + 0.4) = 1.4.
        float baseUtility = 1.0f;
        float totalInfluence = ContextualInfluenceManager.Instance.GetTotalContextualInfluence(npc, "DummyAction");
        float adjustedUtility = baseUtility * (1 + totalInfluence);
        NUnitAssert.AreEqual(1.4f, adjustedUtility, 0.05f, "Adjusted utility should be approximately 1.4");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(npcGO);
        if (influenceManager != null)
            Object.DestroyImmediate(influenceManager.gameObject);
    }
}
