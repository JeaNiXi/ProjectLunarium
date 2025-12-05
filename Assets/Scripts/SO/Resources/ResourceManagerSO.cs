using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "ResourceManager", menuName = "Scriptable Objects/Resources/Resource Manager")]
    public class ResourceManagerSO : ScriptableObject
    {
        public List<ResourceSO> AllResourcesList = new List<ResourceSO>();
    }
}