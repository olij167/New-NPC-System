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

    // Baseline values for natural decay (set at initialization)
    private float baselineHappiness, baselinePassion, baselineConfidence;

    [Header("Smoothing Settings")]
    [Tooltip("Time in seconds for the emotion values to smoothly return to baseline.")]
    public float smoothTime = 0.5f;
    private float velocityHappiness, velocityPassion, velocityConfidence;



    [Header("Emotional Disposition")]
    [Tooltip("Defines the NPC's overall emotional temperament which affects how events influence their emotions.")]
    public EmotionalDisposition disposition = EmotionalDisposition.Neutral;
    public enum EmotionalDisposition { Neutral, Optimistic, Pessimistic, Excitable, Stoic }

    public enum EmotionalState { Neutral, Happy, Sad, Excited, Bored, Confident, Anxious, Overwhelmed, Calm }
    public EmotionalState currentState = EmotionalState.Neutral;

    /// <summary>
    /// Initializes the emotion system using default values from the NPC's Personality.
    /// </summary>
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
        // Gradually decay current emotions back toward their baseline values.
        happiness = Mathf.SmoothDamp(happiness, baselineHappiness, ref velocityHappiness, smoothTime);
        passion = Mathf.SmoothDamp(passion, baselinePassion, ref velocityPassion, smoothTime);
        confidence = Mathf.SmoothDamp(confidence, baselineConfidence, ref velocityConfidence, smoothTime);

        UpdateEmotionalState();
    }

    /// <summary>
    /// Maps the current emotion scales to a discrete emotional state.
    /// Adjust thresholds as needed to reflect nuanced behavior.
    /// </summary>
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

    /// <summary>
    /// Applies an event to the emotion scales.
    /// Uses the NPC's emotional disposition to modify the impact of positive and negative events.
    /// </summary>
    public void ApplyEvent(GameEventData eventData)
    {
        // Determine multipliers based on emotional disposition.
        float positiveMultiplier = 1f;
        float negativeMultiplier = 1f;

        switch (disposition)
        {
            case EmotionalDisposition.Optimistic:
                positiveMultiplier = 1.2f;
                negativeMultiplier = 0.8f;
                break;
            case EmotionalDisposition.Pessimistic:
                positiveMultiplier = 0.8f;
                negativeMultiplier = 1.2f;
                break;
            case EmotionalDisposition.Excitable:
                positiveMultiplier = 1.5f;
                negativeMultiplier = 1.5f;
                break;
            case EmotionalDisposition.Stoic:
                positiveMultiplier = 0.5f;
                negativeMultiplier = 0.5f;
                break;
            default: // Neutral
                positiveMultiplier = 1f;
                negativeMultiplier = 1f;
                break;
        }

        // Compute base deltas from the event.
        float deltaH = 0f, deltaP = 0f, deltaC = 0f;
        switch (eventData.eventType)
        {
            case GameEventType.PlayerInteraction:
                deltaH = eventData.intensity * 0.1f;
                deltaC = eventData.intensity * 0.05f;
                break;
            case GameEventType.WitnessedAction:
                deltaH = -eventData.intensity * 0.1f;
                break;
            case GameEventType.EnvironmentalChange:
                deltaP = eventData.intensity * 0.05f;
                break;
        }

        // Apply multipliers based on whether the delta is positive or negative.
        deltaH = deltaH >= 0 ? deltaH * positiveMultiplier : deltaH * negativeMultiplier;
        deltaP = deltaP >= 0 ? deltaP * positiveMultiplier : deltaP * negativeMultiplier;
        deltaC = deltaC >= 0 ? deltaC * positiveMultiplier : deltaC * negativeMultiplier;

        // Update both the baseline (for long-term mood) and current values (for immediate reactions).
        baselineHappiness = Mathf.Clamp01(baselineHappiness + deltaH);
        baselinePassion = Mathf.Clamp01(baselinePassion + deltaP);
        baselineConfidence = Mathf.Clamp01(baselineConfidence + deltaC);

        happiness = Mathf.Clamp01(happiness + deltaH);
        passion = Mathf.Clamp01(passion + deltaP);
        confidence = Mathf.Clamp01(confidence + deltaC);

        UpdateEmotionalState();
    }
}
