using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class ResourceState
    {

        public event Action<ResourceSO, int> OnResourceAmountChanged;

        private Dictionary<ResourceSO, int> ResourceAmounts;
        private Dictionary<ResourceSO, int> ResourceIncomes;

        public ResourceState(ResourceManagerSO MainSO)
        {
            ResourceAmounts = new Dictionary<ResourceSO, int>();
            ResourceIncomes = new Dictionary<ResourceSO, int>();

            foreach (var resource in MainSO.AllResourcesList)
            {
                if (resource == null)
                    continue;
                ResourceAmounts.Add(resource, 0);
                ResourceIncomes.Add(resource, 0);
            }
        }

        public bool HasResourceAmount(ResourceSO resource, int amount)
        {
            if (resource == null)
                return false;
            return ResourceAmounts.TryGetValue(resource, out var currentAmount) && currentAmount >= amount;
        }

        public void AddResourceAmount(ResourceSO resource, int amount)
        {
            if (resource == null || amount <= 0)
                return;
            if (ResourceAmounts.ContainsKey(resource))
            {
                ResourceAmounts[resource] += amount;
                Debug.Log($"Added Resource: {resource.ID} : {amount} ! ");
                OnResourceAmountChanged?.Invoke(resource, ResourceAmounts[resource]);
            }
        }

        public void SpendResourceAmount(ResourceSO resource, int amount)
        {
            if (resource == null || amount <= 0)
                return;
            if (ResourceAmounts.ContainsKey(resource))
            {
                ResourceAmounts[resource] -= amount;
                OnResourceAmountChanged?.Invoke(resource, ResourceAmounts[resource]);
            }
        }

        public int GetResourceAmount(ResourceSO resource) =>
            resource != null && ResourceAmounts.TryGetValue(resource, out var currentAmount) ? currentAmount : 0;
        public void ResetIncomes()
        {
            var keys = new List<ResourceSO>(ResourceIncomes.Keys);
            foreach (var key in keys)
                ResourceIncomes[key] = 0;
        }
        public void SetResourceIncome(ResourceSO resource, int amount)
        {
            if (resource != null && ResourceIncomes.ContainsKey(resource))
            {
                ResourceIncomes[resource] = amount;
            }
        }
        public int GetResourceIncome(ResourceSO resource)
        {
            if (resource != null && ResourceIncomes.TryGetValue(resource, out var currentIncome))
            {
                return currentIncome;
            }
            return 0;
        }
        public void UpdateResourceAmountsFromSaveData(Dictionary<ResourceSO, int> newAmount)
        {
            foreach (var (res, val) in newAmount)
            {
                if (ResourceAmounts.ContainsKey(res))
                    ResourceAmounts[res] = val;
            }
        }

        public Dictionary<ResourceSO, int> GetResourceAmountDictionary()
            => ResourceAmounts;
    }
}