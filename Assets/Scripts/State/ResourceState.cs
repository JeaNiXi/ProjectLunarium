using SO;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class ResourceState
    {
        private ResourceStateSO resourceStateSO;
        private Dictionary<ResourceSO, float> ResourcesAmounts = new Dictionary<ResourceSO, float>();

        public ResourceState(ResourceManagerSO resourceManagerSO)
        {
            foreach (ResourceSO resource in resourceManagerSO.AllResourcesList)
            {
                ResourcesAmounts.Add(resource, 0);
            }
            resourceStateSO = Resources.Load<ResourceStateSO>("SO/ResourceState");
            InitializeStateSO();
        }

        private void InitializeStateSO()
        {
            if (resourceStateSO.resourceAmountsDictionary != null)
                resourceStateSO.ClearAmountsDictionary();
            foreach (var (resource, value) in ResourcesAmounts)
                resourceStateSO.resourceAmountsDictionary.Add(resource, value);
            if (resourceStateSO != null)
                resourceStateSO.ClearAmountsList();
            foreach (var resource in ResourcesAmounts.Values)
                resourceStateSO.resourcesAmountsList.Add(resource);
        }
    }
}