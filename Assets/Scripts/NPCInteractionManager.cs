using UnityEngine;
using System.Collections.Generic;

public class NPCInteractionManager : MonoBehaviour
{
    // The current interaction session this NPC is engaged in.
    public InteractionSession currentSession;
    // Optional reference to the NPCScheduling component.
    private NPCScheduling scheduling;

    // Configuration: radius to search for social candidates.
    public float socialSearchRadius = 10f;

    void Awake()
    {
        scheduling = GetComponent<NPCScheduling>();
    }

    /// <summary>
    /// Returns a nearby NPC candidate for social interaction (excluding self).
    /// </summary>
    public NPC FindNearestSocialCandidate(NPC self)
    {
        NPC bestCandidate = null;
        float bestDistance = Mathf.Infinity;
        NPC[] allNPCs = GameObject.FindObjectsOfType<NPC>();
        foreach (NPC npc in allNPCs)
        {
            if (npc == self) continue;
            float dist = Vector3.Distance(self.transform.position, npc.transform.position);
            if (dist < bestDistance && dist <= socialSearchRadius)
            {
                bestCandidate = npc;
                bestDistance = dist;
            }
        }
        return bestCandidate;
    }

    /// <summary>
    /// Determines whether the given NPC is available for social interaction.
    /// For now, this stub checks if the NPC's scheduling state is not busy.
    /// </summary>
    public bool CanSocialise(NPC npc)
    {
        if (npc == null)
            return false;
        NPCScheduling sched = npc.GetComponent<NPCScheduling>();
        if (sched != null)
        {
            // If the NPC is working or sleeping, they are not available.
            if (sched.currentTask == "Work" || sched.currentTask == "Sleep")
                return false;
        }
        return true;
    }

    /// <summary>
    /// Initiates a new interaction session if none is active.
    /// Invites nearby NPCs (within socialSearchRadius) to join and sends an initial greeting.
    /// </summary>
    public void InitiateInteraction()
    {
        if (currentSession != null && currentSession.IsActive())
            return;

        NPC self = GetComponent<NPC>();
        currentSession = new InteractionSession(self);

        Collider[] colliders = Physics.OverlapSphere(transform.position, socialSearchRadius);
        foreach (Collider col in colliders)
        {
            NPC other = col.GetComponent<NPC>();
            if (other != null && other != self)
            {
                NPCInteractionManager otherManager = other.GetComponent<NPCInteractionManager>();
                if (otherManager != null && (otherManager.currentSession == null || !otherManager.currentSession.IsActive()))
                {
                    otherManager.JoinInteraction(currentSession);
                }
            }
        }

        // Send an initial greeting.
        SendInteractionMessage(InteractionMessageType.Greeting, 0.5f, 0.5f, 0.5f, 0.5f, "Hello!", null);
    }

    /// <summary>
    /// Allows this NPC to join an existing interaction session.
    /// </summary>
    public void JoinInteraction(InteractionSession session)
    {
        NPC self = GetComponent<NPC>();
        if (session != null && session.IsActive() && !session.participants.Contains(self))
        {
            session.participants.Add(self);
            currentSession = session;
        }
    }

    /// <summary>
    /// Sends an interaction message within the current session.
    /// If no active session exists, one is initiated.
    /// </summary>
    public void SendInteractionMessage(InteractionMessageType messageType, float emotionIntensity, float friendliness, float socialSway, float persuasiveness, string description, NPC target = null)
    {
        NPC self = GetComponent<NPC>();
        if (currentSession == null || !currentSession.IsActive())
        {
            InitiateInteraction();
        }

        InteractionMessage msg = new InteractionMessage(self, target, messageType, emotionIntensity, friendliness, socialSway, persuasiveness, description);
        currentSession.AddMessage(msg);

        ProcessIncomingMessage(msg);

        foreach (NPC npc in currentSession.participants)
        {
            if (npc != self)
            {
                NPCInteractionManager manager = npc.GetComponent<NPCInteractionManager>();
                if (manager != null)
                {
                    manager.ProcessIncomingMessage(msg);
                }
            }
        }
    }

    /// <summary>
    /// Processes an incoming interaction message.
    /// Evaluates the message using personality, memory, emotion, relationship, and scheduling inputs.
    /// </summary>
    public void ProcessIncomingMessage(InteractionMessage msg)
    {
        NPC self = GetComponent<NPC>();
        Debug.Log($"{self.identity.npcName} received: '{msg.description}' from {msg.sender.identity.npcName}");
        // Reaction logic is handled in SocialiseInteraction.
    }

    /// <summary>
    /// Ends the current interaction session.
    /// </summary>
    public void EndInteraction()
    {
        if (currentSession != null)
        {
            currentSession.state = InteractionSessionState.Terminated;
            currentSession = null;
        }
    }

    /// <summary>
    /// Returns a list of NPCs that are sexually compatible with the given NPC.
    /// Checks for matching attraction preferences in the Sexuality component.
    /// </summary>
    public List<NPC> GetSexuallyCompatibleNPCs(NPC self)
    {
        List<NPC> result = new List<NPC>();
        NPCManager npcManager = NPCManager.Instance;
        if (npcManager == null || self == null)
            return result;

        Sexuality selfSex = self.GetComponent<Sexuality>();
        if (selfSex == null)
            return result;

        List<NPC> allNPCs = npcManager.GetAllNPCs();
        foreach (NPC other in allNPCs)
        {
            if (other == self)
                continue;

            Sexuality otherSex = other.GetComponent<Sexuality>();
            if (otherSex == null)
                continue;

            // Check if any attraction preferences match.
            foreach (var pref in selfSex.attractionPreferences)
            {
                foreach (var opref in otherSex.attractionPreferences)
                {
                    if (pref.bodyPart == opref.bodyPart)
                    {
                        result.Add(other);
                        goto NextNPC;
                    }
                }
            }
        NextNPC:
            continue;
        }

        return result;
    }
}
