using SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;
namespace State
{
    public class WorkersState
    {
        private WorkersStateSO workersStateSO;
        private Dictionary<ResourceSO, int> WorkersAmounts = new Dictionary<ResourceSO, int>();

        public WorkersState(WorkersManagerSO workersManagerSO)
        {
            workersStateSO = Resources.Load<WorkersStateSO>("SO/WorkersState");
            InitializeWorkersAmounts();
            InitializeStateSO();
        }
        private void InitializeWorkersAmounts()
        {
            //OnGame Start/Load to change Amounts;
        }
        private void InitializeStateSO()
        {
            ClearStateSODictionary();
            ClearStateSOLists();
            if (WorkersAmounts == null)
                return;
        }
        private void UpdateStateSO()
        {
            if (WorkersAmounts.Count == 0)
            {
                Debug.LogError($"Update Workers State SO was called, but Dictionary is Empty!!!");
                return;
            }
            WriteDataToWorkersStateDictionarySO();
            WriteDataToWorkersStateListSO();
        }
        private void WriteDataToWorkersStateDictionarySO()
        {
            foreach (var key in WorkersAmounts.Keys)
                if (workersStateSO.currentWorkersStateDictionary.ContainsKey(key))
                    workersStateSO.currentWorkersStateDictionary[key] = WorkersAmounts[key];
                else
                    workersStateSO.currentWorkersStateDictionary.Add(key, WorkersAmounts[key]);
        }
        private void WriteDataToWorkersStateListSO()
        {
            int index = 0;
            foreach (var key in WorkersAmounts.Keys)
                if (index >= workersStateSO.currentWorkersAmountStateList.Count)
                {
                    workersStateSO.currentWorkersResourcesStateList.Add(key.NameKey);
                    workersStateSO.currentWorkersAmountStateList.Add(WorkersAmounts[key]);
                    index++;
                }
                else
                {
                    workersStateSO.currentWorkersAmountStateList[index] = WorkersAmounts[key];
                    index++;
                }
        }
        private void ClearStateSODictionary()
        {
            if (workersStateSO.currentWorkersStateDictionary.Count > 0)
                workersStateSO.ClearWorkersDictionary();
        }

        private void ClearStateSOLists()
        {
            if (workersStateSO.currentWorkersResourcesStateList.Count > 0 || workersStateSO.currentWorkersAmountStateList.Count > 0)
            {
                workersStateSO.ClearWorkersResourcesStateList();
                workersStateSO.ClearWorkersAmountStateList();
            }
        }
        public void AddWorkerToResource(ResourceSO resource)
        {
            if (WorkersAmounts.TryGetValue(resource, out var value))
            {
                WorkersAmounts[resource] += GetWorkersAmountToAdd(resource);
                UpdateStateSO();
            }
            else
            {
                WorkersAmounts.Add(resource, GetWorkersAmountToAddOnCreate(resource));
                UpdateStateSO();
            }
        }
        public int GetWorkersAmountToAdd(ResourceSO resource) => 1;
        public int GetWorkersAmountToAddOnCreate(ResourceSO resource) => 1;
    }
}