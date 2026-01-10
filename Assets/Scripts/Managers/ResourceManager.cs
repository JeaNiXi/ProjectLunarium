using Data;
using SO;
using State;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Managers
{
    public class ResourceManager : MonoBehaviour, ISaveableState
    {
        public static ResourceManager Instance;
        public ResourceManagerSO ResourceManagerSO;
        private ResourceState CurrentResourceState;
        private Dictionary<string, ResourceSO> ResourceIDs;
        private List<ResourceSO> AllResourcesList;

        public string SaveDataFileName => "ResourcesSaveData.json";

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            ResourceIDs = new Dictionary<string, ResourceSO>();
            AllResourcesList = new List<ResourceSO>();
            InitializeHelperData();
            InitializeStartingResourcesState();
        }
        private void InitializeHelperData()
        {
            var allResources = GetAllResourcesList();
            foreach (var resource in allResources)
                ResourceIDs.Add(resource.ID, resource);
            AllResourcesList = allResources;
        }
        public ResourceState GetCurrentResourceState()
            => CurrentResourceState;
        public void InitializeStartingResourcesState() => CurrentResourceState = new ResourceState(ResourceManagerSO);
        public List<ResourceSO> GetAllResourcesList() => ResourceManagerSO.AllResourcesList;
        public void OnGlobalTick(TimeState timeState)
        {

            /*
             * We check here what are the resources we collect first. 
             * All this should be done After technologies and modifiers are in line. (Later Magic, Society, Population).
             * Basically this shlould be the last step, duh. Every other step before this should already have updated everything.
             */
        }
        public void UpdateGathererResourceIncome(int gatherersAmount)
        {
            CurrentResourceState.UpdateGatherersResourcesIncomes(gatherersAmount);
        }
        public void UpdateResourceUsage(int childAmount, int adultAmount, int elderAmount)
        {
            CurrentResourceState.UpdateResourceUsage(childAmount, adultAmount, elderAmount);
        }
        public Dictionary<ResourceSO, int> GetResourceAmountsDictionary()
            => CurrentResourceState.GetResourceAmountsDictionary();
        public bool HasResourceAmount(ResourceSO resource, int amount)
            => CurrentResourceState.HasResourceAmount(resource, amount);
        public int GetResourceAmount(ResourceSO resource)
            => CurrentResourceState.GetResourceAmount(resource);
        public object SaveState()
        {
            var data = new ResourceSaveData()
            {
                ResourceIDs = new(),
                Amounts = new()
            };
            var amountsDictionary = GetResourceAmountsDictionary();
            foreach (var (resource, amount) in amountsDictionary)
            {
                data.ResourceIDs.Add(resource.ID);
                data.Amounts.Add(amount);
            }
            return data;
        }
        public bool TryGetResourceByID(string ID, out ResourceSO resource)
        {
            if (ResourceIDs.ContainsKey(ID))
            {
                resource = ResourceIDs[ID];
                return true;
            }
            else
            {
                resource = null;
                return false;
            }
        }
        public void LoadState(object saveData)
        {
            var data = (ResourceSaveData)saveData;
            var newAmounts = new Dictionary<ResourceSO, int>();
            for (int i = 0; i < data.ResourceIDs.Count; i++)
                if (TryGetResourceByID(data.ResourceIDs[i], out var resource))
                {
                    newAmounts.Add(resource, data.Amounts[i]);
                    Debug.Log($"Loaded From Data File Resource: {resource.ID}, with amount: {data.Amounts[i]}.");
                }
                else
                    Debug.LogError($"Data Corruption Found. ID: {resource.ID} not detected in Save File!");
            CurrentResourceState.UpdateResourceAmountsFromSaveData(newAmounts);
        }

        public void ResetState()
        {
            CurrentResourceState.ClearResourceAmounts();
            CurrentResourceState.InitializeRuntimeResourcesDictionary();
        }
    }
}