//using System.Collections.Generic;
//using UnityEngine;

//public class FamilyTree : MonoBehaviour
//{
//    [Header("Family Relationships")]
//    public List<NPCIdentity> parents = new List<NPCIdentity>();
//    public List<NPCIdentity> siblings = new List<NPCIdentity>();
//    public List<NPCIdentity> children = new List<NPCIdentity>();
//    public NPCIdentity spouse;

//    /// <summary>
//    /// Recursively gathers all ancestors of this NPC.
//    /// </summary>
//    public HashSet<NPCIdentity> GetAllAncestors()
//    {
//        HashSet<NPCIdentity> ancestors = new HashSet<NPCIdentity>();
//        foreach (var parent in parents)
//        {
//            if (parent != null)
//            {
//                ancestors.Add(parent);
//                FamilyTree parentFamily = parent.GetComponent<FamilyTree>();
//                if (parentFamily != null)
//                {
//                    foreach (var ancestor in parentFamily.GetAllAncestors())
//                    {
//                        ancestors.Add(ancestor);
//                    }
//                }
//            }
//        }
//        return ancestors;
//    }
//}
