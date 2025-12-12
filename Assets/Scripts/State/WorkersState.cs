using SO;
using System.Collections.Generic;
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

        }
        public void AddEmptyWorker(ResourceSO resource)
        {
            WorkersAmounts.Add(resource, 0);
            AddEmptyWorkersToStateSO(resource);
        }
           
        private void InitializeStateSO()
        {
            ClearDictionaryAndList();
            if (WorkersAmounts == null)
                return;
            foreach (var key in WorkersAmounts.Keys)
                workersStateSO.currentWorkersStateDictionary.Add(key, 0);
            foreach (var key in WorkersAmounts.Keys)
                workersStateSO.currentWorkersStateList.Add(WorkersAmounts[key]);
        }
        private void ClearDictionaryAndList()
        {
            if (workersStateSO.currentWorkersStateDictionary != null)
                workersStateSO.ClearWorkersDictionary();
            if (workersStateSO.currentWorkersStateList != null)
                workersStateSO.ClearWorkersList();
        }
        private void AddEmptyWorkersToStateSO(ResourceSO resource)
        {
            workersStateSO.currentWorkersStateDictionary.Add(resource, 0);
            workersStateSO.currentWorkersStateList.Add(0);
        }
    }
}