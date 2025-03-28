using UnityEngine;

public enum VisualPresentation { Masculine, Feminine }

public class Sexuality : MonoBehaviour
{
    [Header("Sexual Presentation")]
    // Internal measure: 0 = very feminine, 1 = very masculine.
    [Range(0f, 1f)]
    public float masculinity;
    // Discrete visual appearance setting.
    public VisualPresentation visualPresentation;

    [Header("Sexual Preference")]
    [Range(0f, 1f)]
    public float attractionToMasculine;
    [Range(0f, 1f)]
    public float attractionToFeminine;

    // Determines attraction based on both internal measure and visual presentation.
    public bool IsAttractedTo(Sexuality other)
    {
        float attraction = 0f;
        if (other.visualPresentation == VisualPresentation.Masculine)
        {
            attraction = other.masculinity * attractionToMasculine;
        }
        else // if Feminine
        {
            attraction = (1f - other.masculinity) * attractionToFeminine;
        }
        return attraction > 0.5f; // Adjustable threshold.
    }
}
