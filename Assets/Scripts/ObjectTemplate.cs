using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectTemplate", menuName = "Level/ObjectTemplate", order = 1)]
public class ObjectTemplate : ScriptableObject
{
    public string templateName;
    [Tooltip("Prefab for this object.")]
    public GameObject prefab;
    [Tooltip("Category (e.g., Resource, Work, Social)")]
    public string category;
}
