using System.Collections.Generic;
using UnityEngine;

public enum InteractionSessionState
{
    Initiation,
    Ongoing,
    Terminated
}

public class InteractionSession
{
    public List<NPC> participants = new List<NPC>();
    public List<InteractionMessage> conversationHistory = new List<InteractionMessage>();
    public InteractionSessionState state = InteractionSessionState.Initiation;
    public int maxExchanges = 5; // Maximum number of messages before termination.
    public float sessionStartTime;

    public InteractionSession(NPC initiator)
    {
        sessionStartTime = Time.time;
        participants.Add(initiator);
    }

    public void AddMessage(InteractionMessage msg)
    {
        conversationHistory.Add(msg);
        if (conversationHistory.Count >= maxExchanges)
        {
            state = InteractionSessionState.Terminated;
        }
    }

    public bool IsActive()
    {
        return state != InteractionSessionState.Terminated;
    }
}
