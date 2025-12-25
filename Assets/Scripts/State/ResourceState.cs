using Data;
using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class ResourceState
    {
        private ResourceStateSO resourceStateSO;
        private Dictionary<ResourceSO, int> ResourcesAmounts = new Dictionary<ResourceSO, int>();

        private readonly ResourceSO GathererFoodResource;
        private readonly ResourceSO GathererWaterResource;
        private readonly ResourceSO GathererWoodResource;
        private readonly int BaseGathererFoodIncome = 2;
        private readonly int BaseGathererWaterIncome = 1;
        private readonly int BaseGathererWoodIncome = 1;
        private readonly float BaseChildFoodUsage = .6f;
        private readonly float BaseChildWaterUsage = .4f;
        private readonly float BaseChildWoodUsage = .2f;
        private readonly float BaseAdultFoodUsage = 1f;
        private readonly float BaseAdultWaterUsage = .6f;
        private readonly float BaseAdultWoodUsage = .8f;
        private readonly float BaseElderFoodUsage = .8f;
        private readonly float BaseElderWaterUsage = .6f;
        private readonly float BaseElederWoodUsage = .6f;

        public ResourceState(ResourceManagerSO resourceManagerSO)
        {
            GathererFoodResource = resourceManagerSO.GathererFoodResource;
            GathererWaterResource = resourceManagerSO.GathererWaterResource;
            GathererWoodResource = resourceManagerSO.GahrererWoodResource;
            foreach (ResourceSO resource in resourceManagerSO.AllResourcesList)
            {
                ResourcesAmounts.Add(resource, 0);
            }
            resourceStateSO = Resources.Load<ResourceStateSO>("SO/ResourceState");
            //InitializeResourceStateSO();
            InitializeRuntimeResourcesDictionary();
        }
        public Dictionary<ResourceSO, int> GetResourceAmountsDictionary()
            => ResourcesAmounts;
        public void InitializeRuntimeResourcesDictionary()
        {
            resourceStateSO.ClearRuntimeResourceAmountsDictionary();
            foreach (var (resource, amount) in ResourcesAmounts)
                resourceStateSO.InitializeRuntimeResourceAmountsDictionary(resource, amount);
            resourceStateSO.ClearRuntimeResourceAmountsList();
            resourceStateSO.UpdateRuntimeResourceAmountsList();
        }
        private void InitializeResourceStateSO()
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



        public void UpdateGatherersResourcesIncomes(int gatherersAmount) // Remake to Income State
        {
            var foodIncome = BaseGathererFoodIncome * gatherersAmount;
            var waterIncome = BaseGathererWaterIncome * gatherersAmount;
            var woodIncome = BaseGathererWoodIncome * gatherersAmount;

            AddResourceAmount(GathererFoodResource, foodIncome);
            AddResourceAmount(GathererWaterResource, waterIncome);
            AddResourceAmount(GathererWoodResource, woodIncome);
            Debug.Log($"Current Date :{DebugExtensions.GetCurrentDateString()}, adding " +
                $"food: {foodIncome}, " +
                $"water: {waterIncome}, " +
                $"wood: {woodIncome},");
        }
        public void UpdateResourceUsage(int childAmount, int adultAmount, int elderAmount)
        {
            int foodUsage = Mathf.RoundToInt(BaseChildFoodUsage * childAmount + BaseAdultFoodUsage * adultAmount + BaseElderFoodUsage * elderAmount);
            var waterUsage = Mathf.RoundToInt(BaseChildWaterUsage * childAmount + BaseAdultWaterUsage * adultAmount + BaseElderWaterUsage * elderAmount);
            var woodUsage = Mathf.RoundToInt(BaseChildWoodUsage * childAmount + BaseAdultWoodUsage * adultAmount + BaseElederWoodUsage * elderAmount);
            RemoveResourceAmount(GathererFoodResource, foodUsage);
            RemoveResourceAmount(GathererWaterResource, waterUsage);
            RemoveResourceAmount(GathererWoodResource, woodUsage);
            Debug.Log($"Removing: " +
                $"food: {foodUsage}, " +
                $"water: {waterUsage}, " +
                $"wood: {woodUsage},");
            Debug.Log($"Resources Left: " +
                $"Food: {GetResourceAmount(GathererFoodResource)}, " +
                $"Water: {GetResourceAmount(GathererWaterResource)}, " +
                $"Wood: {GetResourceAmount(GathererWoodResource)}.");
            UpdateResourceState();
        }

        public void AddResourceAmount(ResourceSO resource, int amount)
        {
            if (ResourcesAmounts.ContainsKey(resource))
                ResourcesAmounts[resource] += amount;
        }
        public void RemoveResourceAmount(ResourceSO resource, int amount)
        {
            if (ResourcesAmounts.ContainsKey(resource))
                ResourcesAmounts[resource] -= amount;
        }
        private int GetResourceAmount(ResourceSO resource)
                    => ResourcesAmounts.TryGetValue(resource, out var amount) ? amount : 0;
        public void UpdateResourceState()
        {
            foreach (var resource in ResourcesAmounts.Keys)
                resourceStateSO.UpdateResourceRuntimeAmountsDictionary(resource, ResourcesAmounts[resource]);
            resourceStateSO.UpdateRuntimeResourceAmountsList();
        }
        public void ClearResourceAmounts()
            => ResourcesAmounts.Clear();
        public void UpdateResourceAmountsFromSaveData(Dictionary<ResourceSO, int> resourcesAmountsDictionary)
            => ResourcesAmounts = resourcesAmountsDictionary;
        public bool HasResourceAmount(ResourceSO resource, int amount)
        {
            if (ResourcesAmounts.ContainsKey(resource))
                if (ResourcesAmounts[resource] >= amount)
                    return true;
            return false;
        }
    }
}