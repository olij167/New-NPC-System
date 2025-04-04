using UnityEngine;

public class EmotionSystem : MonoBehaviour
{
    [Header("Emotion Scales")]
    [Range(0f, 1f)]
    public float happiness;
    [Range(0f, 1f)]
    public float passion;
    [Range(0f, 1f)]
    public float confidence;

    private float baselineHappiness, baselinePassion, baselineConfidence;
    private float velocityHappiness, velocityPassion, velocityConfidence;

    [Header("Smoothing Settings")]
    public float smoothTime = 0.5f;

    [Header("Emotional Disposition")]
    public EmotionalDisposition disposition = EmotionalDisposition.Neutral;
    public enum EmotionalDisposition { Neutral, Optimistic, Pessimistic, Excitable, Stoic }
    public enum EmotionalState { Neutral, Happy, Sad, Excited, Bored, Confident, Anxious, Overwhelmed, Calm }
    public EmotionalState currentState = EmotionalState.Neutral;

    public void Initialize(Personality personality)
    {
        happiness = personality.defaultHappiness;
        passion = personality.defaultPassion;
        confidence = personality.defaultConfidence;

        baselineHappiness = happiness;
        baselinePassion = passion;
        baselineConfidence = confidence;

        UpdateEmotionalState();
    }

    void Update()
    {
        happiness = Mathf.SmoothDamp(happiness, baselineHappiness, ref velocityHappiness, smoothTime);
        passion = Mathf.SmoothDamp(passion, baselinePassion, ref velocityPassion, smoothTime);
        confidence = Mathf.SmoothDamp(confidence, baselineConfidence, ref velocityConfidence, smoothTime);
        UpdateEmotionalState();
    }

    public void UpdateEmotionalState()
    {
        if (happiness > 0.7f && passion > 0.7f && confidence > 0.7f)
            currentState = EmotionalState.Happy;
        else if (happiness < 0.3f && passion < 0.3f)
            currentState = EmotionalState.Sad;
        else if (passion > 0.7f && happiness > 0.7f && confidence < 0.4f)
            currentState = EmotionalState.Excited;
        else if (passion < 0.3f)
            currentState = EmotionalState.Bored;
        else if (confidence < 0.3f)
            currentState = EmotionalState.Anxious;
        else if (happiness < 0.4f && passion > 0.6f && confidence < 0.4f)
            currentState = EmotionalState.Overwhelmed;
        else if (happiness > 0.6f && passion < 0.4f && confidence > 0.6f)
            currentState = EmotionalState.Calm;
        else
            currentState = EmotionalState.Neutral;
    }

    // Returns an emotion-based modifier for an action based on current emotional state.
    public float GetEmotionModifierForAction(string actionName)
    {
        // Placeholder logic: adjust modifiers based on emotional state.
        switch (currentState)
        {
            case EmotionalState.Happy:
                if (actionName == "Explore" || actionName == "InteractWithNPC")
                    return 0.2f;
                break;
            case EmotionalState.Sad:
                if (actionName == "Explore")
                    return -0.2f;
                break;
            case EmotionalState.Anxious:
                if (actionName == "Flee")
                    return 0.3f;
                if (actionName == "InteractWithNPC")
                    return -0.3f;
                break;
            case EmotionalState.Excited:
                if (actionName == "Explore" || actionName == "InteractWithNPC")
                    return 0.15f;
                break;
            default:
                break;
        }
        return 0f;
    }
}
