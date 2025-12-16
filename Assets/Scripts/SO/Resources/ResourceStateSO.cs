using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "ResourceState", menuName = "Scriptable Objects/Resources/Resource State")]
    public class ResourceStateSO : ScriptableObject
    {
        public List<float> resourcesAmountsList = new List<float>();
        public Dictionary<ResourceSO, float> resourceAmountsDictionary = new Dictionary<ResourceSO, float>();

        public List<RuntimeResourceState> RuntimeResourceAmountsList = new List<RuntimeResourceState>();
        private Dictionary<ResourceSO, RuntimeResourceState> runtimeResourceAmountsDictionary = new Dictionary<ResourceSO, RuntimeResourceState>();

        public void ClearAmountsList()
            => resourcesAmountsList.Clear();
        public void ClearAmountsDictionary()
            => resourceAmountsDictionary.Clear();
        //public float GetResourceAmount(ResourceSO resource)
        //    => resourceAmountsDictionary.TryGetValue(resource, out var value) ? value : 0f;



        #region StateInitializationsAndUpdates
        public void ClearRuntimeResourceAmountsDictionary()
            => runtimeResourceAmountsDictionary.Clear();
        public void InitializeRuntimeResourceAmountsDictionary(ResourceSO resource, int amount)
            => runtimeResourceAmountsDictionary.Add(resource, new RuntimeResourceState(resource, amount));
        public void ClearRuntimeResourceAmountsList()
            => RuntimeResourceAmountsList.Clear();
        public void UpdateResourceRuntimeAmountsDictionary(ResourceSO resource, int amount)
            => UpdateResourceAmount(resource, amount);
        public void UpdateRuntimeResourceAmountsList()
        {
            ClearRuntimeResourceAmountsList();
            foreach (var resource in runtimeResourceAmountsDictionary.Keys)
                RuntimeResourceAmountsList.Add(runtimeResourceAmountsDictionary[resource]);
        }
        #endregion

        #region ResourceOperations
        public int GetResourceAmount(ResourceSO resource)
            => runtimeResourceAmountsDictionary.TryGetValue(resource, out var value) ? value.Amount : 0;
        public void AddResourceAmount(ResourceSO resource, int amount)
        {
            if (runtimeResourceAmountsDictionary.ContainsKey(resource))
                runtimeResourceAmountsDictionary[resource].AddAmount(amount);
        }
        public void UpdateResourceAmount(ResourceSO resource, int amount)
        {
            if(runtimeResourceAmountsDictionary.ContainsKey(resource))
                runtimeResourceAmountsDictionary[resource].UpdateAmount(amount);
        }
        #endregion
    }


    [Serializable]
    public class RuntimeResourceState
    {
        [field: SerializeField] public ResourceSO Resource { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        public RuntimeResourceState(ResourceSO resource, int amount)
        {
            Resource = resource;
            Amount = amount;
        }
        public void AddAmount(int amount) => Amount += amount;
        public void UpdateAmount(int amount) => Amount = amount;
    }
}