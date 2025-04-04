using UnityEngine;

public class HairstyleOffset : MonoBehaviour
{
    [Tooltip("Local position offset relative to the head.")]
    public Vector3 positionOffset = Vector3.zero;
    [Tooltip("Local rotation offset (Euler angles) relative to the head.")]
    public Vector3 rotationOffset = Vector3.zero;
}
