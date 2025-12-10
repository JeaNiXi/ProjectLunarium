using SO;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "ResourceManager", menuName = "Scriptable Objects/Resources/Resource Manager")]
    public class ResourceManagerSO : ScriptableObject
    {
        public List<ResourceSO> AllResourcesList = new List<ResourceSO>();
        public bool IsResourceVisible(ResourceSO resource, TechnologyStateSO techState)
        {
            if (resource.TechNeeded == null)
                return true;
            foreach(var tech in resource.TechNeeded)
            {
                if (!techState.researchedTechnologies.Contains(tech))
                    return false;
            }
            return true;
        }
    }
}