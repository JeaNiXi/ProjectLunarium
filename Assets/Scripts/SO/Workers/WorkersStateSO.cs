using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "WorkersState", menuName = "Scriptable Objects/Workers/Workers State")]

    public class WorkersStateSO : ScriptableObject
    {
        public List<ResourceWorkersAmounts> WorkersPerResourceAmounts = new List<ResourceWorkersAmounts>();
        public List<string> currentWorkersResourcesStateList = new List<string>();
        public List<int> currentWorkersAmountStateList = new List<int>();
        public Dictionary<ResourceSO, int> currentWorkersStateDictionary = new Dictionary<ResourceSO, int>();
        public void ClearWorkersResourcesStateList() => currentWorkersResourcesStateList.Clear();
        public void ClearWorkersAmountStateList() => currentWorkersAmountStateList.Clear();
        public void ClearWorkersDictionary() => currentWorkersStateDictionary.Clear();
        public int GetWorkersAmount(ResourceSO resource) => currentWorkersStateDictionary.TryGetValue(resource, out var amount) ? amount : 0;

    }

    [Serializable]
    public struct ResourceWorkersAmounts
    {
        public string ResourceKey;
        public int WorkersAmount;
    }
}