using System;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMakerStateMachine : MonoBehaviour
{
    // The current state as defined by a StateDefinition asset.
    public StateDefinition CurrentState { get; private set; }

    // Timer tracking the duration in the current state.
    public float StateTimer { get; private set; }

    [Header("State Timing Settings")]
    [Tooltip("Maximum duration (in seconds) a state can persist before forcing a fallback (typically to Idle).")]
    public float maxStateDuration = 5f;

    [Header("Transition Settings")]
    [Tooltip("Minimum utility difference required to override the current state.")]
    public float overrideUtilityDifference = 0.5f;

    // List of all available state definitions (set via the Inspector).
    [Header("Available States (Data-Driven)")]
    [Tooltip("List of state definitions available to the NPC.")]
    public List<StateDefinition> availableStates;

    // Event raised when the state changes (old state, new state).
    public event Action<StateDefinition, StateDefinition> OnStateChanged;

    void Awake()
    {
        if (availableStates == null || availableStates.Count == 0)
        {
            Debug.LogError("No state definitions assigned to DecisionMakerStateMachine.");
            return;
        }
        // Default to the state named "Idle" if available; otherwise, use the first state.
        StateDefinition exploreState = availableStates.Find(state => state.stateName == "Explore");
        CurrentState = exploreState != null ? exploreState : availableStates[0];
        StateTimer = 0f;
    }

    void Update()
    {
        // Increment the timer.
        StateTimer += Time.deltaTime;

        // If the current state exceeds its maximum allowed duration (and isn't Idle), force a fallback to Idle.
        if (CurrentState != null && StateTimer >= maxStateDuration && CurrentState.stateName != "Explore")
        {
            ChangeState("Explore");
        }
    }

    /// <summary>
    /// Returns the list of allowed transitions from the current state.
    /// </summary>
    public List<StateDefinition> GetAllowedTransitions()
    {
        return CurrentState != null ? CurrentState.allowedTransitions : new List<StateDefinition>();
    }

    /// <summary>
    /// Evaluates whether a transition should occur based on candidate and current utility values.
    /// The evaluation considers the current state's minimum duration requirement and a utility gap threshold.
    /// </summary>
    /// <param name="candidateUtility">Utility value of the candidate action/state.</param>
    /// <param name="currentUtility">Utility value of the current action/state.</param>
    /// <returns>True if conditions for transition are met; otherwise, false.</returns>
    public bool EvaluateTransition(float candidateUtility, float currentUtility)
    {
        if (CurrentState == null)
            return false;
        return StateTimer >= CurrentState.minDuration && (candidateUtility - currentUtility) >= overrideUtilityDifference;
    }

    /// <summary>
    /// Changes the current state to the state with the specified stateName, if the transition is allowed.
    /// Resets the state timer and raises the OnStateChanged event.
    /// </summary>
    /// <param name="newStateName">The name of the state to transition into.</param>
    public void ChangeState(string newStateName)
    {
        if (CurrentState == null)
            return;

        // Verify that the transition is allowed from the current state.
        if (!CurrentState.IsTransitionAllowed(availableStates.Find(state => state.stateName == newStateName)))
        {
            Debug.LogWarning($"Transition from {CurrentState.stateName} to {newStateName} is not allowed.");
            return;
        }

        // Find the new state from available states.
        StateDefinition newState = availableStates.Find(state => state.stateName == newStateName);
        if (newState != null && newState != CurrentState)
        {
            StateDefinition oldState = CurrentState;
            CurrentState = newState;
            StateTimer = 0f;
            OnStateChanged?.Invoke(oldState, newState);
            Debug.Log($"State changed from {oldState.stateName} to {newState.stateName}");
        }
    }
}
