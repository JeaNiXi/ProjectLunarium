using SO;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class ResourceState
    {
        private ResourceStateSO resourceStateSO;
        private Dictionary<ResourceSO, int> ResourcesAmounts = new Dictionary<ResourceSO, int>();

        public ResourceState(ResourceManagerSO resourceManagerSO)
        {
            foreach (ResourceSO resource in resourceManagerSO.AllResourcesList)
            {
                ResourcesAmounts.Add(resource, 0);
            }
            resourceStateSO = Resources.Load<ResourceStateSO>("SO/ResourceState");
            UpdateStateSO();
        }

        private void UpdateStateSO()
        {
            if (resourceStateSO != null)
                resourceStateSO.ClearAmountsList();
            foreach (var resource in ResourcesAmounts.Values)
                resourceStateSO.resourcesAmounts.Add(resource);
        }
    }
}