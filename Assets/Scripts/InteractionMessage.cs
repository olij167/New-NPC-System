using System;
using UnityEngine;

public enum InteractionMessageType
{
    Greeting,
    Argument,
    Agreement,
    Disagreement,
    Farewell,
    // Additional types can be added as needed.
}

public class InteractionMessage
{
    public NPC sender;
    public NPC target; // If null, the message is broadcast to the session.
    public float timestamp;
    public InteractionMessageType messageType;
    public float emotionIntensity;
    public float friendliness;
    public float socialSway;
    public float persuasiveness;
    public string description;

    public InteractionMessage(NPC sender, NPC target, InteractionMessageType messageType, float emotionIntensity, float friendliness, float socialSway, float persuasiveness, string description = "")
    {
        this.sender = sender;
        this.target = target;
        this.messageType = messageType;
        this.emotionIntensity = emotionIntensity;
        this.friendliness = friendliness;
        this.socialSway = socialSway;
        this.persuasiveness = persuasiveness;
        this.description = description;
        timestamp = Time.time;
    }
}
