using System.Collections.Generic;
using UnityEngine;

public class NeedProvider : MonoBehaviour
{
    [Tooltip("List of need names that this object can satisfy (e.g., 'Hunger', 'Thirst').")]
    public List<string> satisfiesNeeds = new List<string>();

    [Tooltip("A factor representing how effective this object is at satisfying the need (default is 1).")]
    public float satisfactionValue = 1f;
}
