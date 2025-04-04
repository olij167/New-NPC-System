using UnityEngine;
using Pathfinding;
using System.Reflection;

public class PathCleanup : MonoBehaviour
{
    void OnDisable()
    {
        if (AstarPath.active != null)
        {
            MethodInfo onDisableMethod = typeof(AstarPath).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
            if (onDisableMethod != null)
            {
                onDisableMethod.Invoke(AstarPath.active, null);
                Debug.Log("AstarPath.OnDisable() invoked via reflection in OnDisable()");
            }
            else
            {
                Debug.LogWarning("OnDisable method not found on AstarPath.");
            }
        }
    }

    void OnApplicationQuit()
    {
        if (AstarPath.active != null)
        {
            MethodInfo onDisableMethod = typeof(AstarPath).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
            if (onDisableMethod != null)
            {
                onDisableMethod.Invoke(AstarPath.active, null);
                Debug.Log("AstarPath.OnDisable() invoked via reflection in OnApplicationQuit()");
            }
            else
            {
                Debug.LogWarning("OnDisable method not found on AstarPath.");
            }
        }
    }
}
