using Data;
using SO;
using State;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Managers
{
    public class ResourceManager : MonoBehaviour, ISaveableState
    {
        public static ResourceManager Instance;
        public ResourceManagerSO MainSO;

        public event Action OnVisibleUIResourcesUpdateNeeded;

        private ResourceState CurrentResourceState;

        private Dictionary<string, ResourceSO> ResourceIDs;
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
            InitializeData();
            InitializeHelperData();
        }

        private void InitializeData()
        {
            CurrentResourceState = new ResourceState(MainSO);
            ResourceIDs = new Dictionary<string, ResourceSO>();
        }
        private void InitializeHelperData()
        {
            ResourceIDs.Clear();
            foreach (var resource in MainSO.AllResourcesList)
            {
                if (resource != null)
                    ResourceIDs.Add(resource.ID, resource);
            }
        }

        public ResourceState GetCurrentResourceState()
            => CurrentResourceState;

        public void OnGlobalTick()
        {
            CurrentResourceState.ResetIncomes();
            CalculateGlobalProduction();
            UpdateVisibleUIResources();
        }
        public void CalculateGlobalProduction()
        {
            foreach (WorkPlace workPlace in WorkPlaceManager.Instance.GetAllWorkPlaces())
            {
                if (workPlace.CurrentWorkModeType == null || workPlace.GetWorkersAmount() == 0)
                    continue;
                foreach(var stack in workPlace.CurrentWorkModeType.ProducedResources)
                {
                    int amount = (int)(stack.Amount * workPlace.GetWorkersAmount());
                    int current = CurrentResourceState.GetResourceIncome(stack.Resource);
                    CurrentResourceState.SetResourceIncome(stack.Resource, current + amount);
                }
            }
        }
        public bool HasResourceAmount(ResourceSO resource, int amount) =>
            CurrentResourceState.HasResourceAmount(resource, amount);

        public int GetResourceAmount(ResourceSO resource) =>
            CurrentResourceState.GetResourceAmount(resource);

        public void AddResource(ResourceSO resource, int amount) =>
            CurrentResourceState.AddResourceAmount(resource, amount);

        public void SpendResource(ResourceSO resource, int amount) =>
            CurrentResourceState.SpendResourceAmount(resource, amount);

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

        public Dictionary<ResourceSO, int> GetResourceAmountsDictionary()
            => CurrentResourceState.GetResourceAmountDictionary();

        public bool TryGetResourceByID(string id, out ResourceSO resource) =>
            ResourceIDs.TryGetValue(id, out resource);

        public void UpdateVisibleUIResources() =>
            OnVisibleUIResourcesUpdateNeeded?.Invoke();

        public void ResetState()
        {

        }
    }
}