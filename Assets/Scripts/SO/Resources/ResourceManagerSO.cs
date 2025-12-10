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
            {
                Debug.Log($" Tech Needed is null, returing TRUE");
                return true;
            }
            foreach(var tech in resource.TechNeeded)
            {
                if (!techState.researchedTechnologies.Contains(tech))
                {
                    Debug.Log($"Researched Tech does not contain {tech.ID}. Returning False");
                    return false;
                }
            }
            Debug.Log($"No Option of above, returning TRUE");
            return true;
        }
    }
}