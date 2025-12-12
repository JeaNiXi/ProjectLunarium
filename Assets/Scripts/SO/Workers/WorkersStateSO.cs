using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "WorkersState", menuName = "Scriptable Objects/Workers/Workers State")]
    public class WorkersStateSO : ScriptableObject
    {
        public List<int> currentWorkersStateList = new List<int>();
        public Dictionary<ResourceSO, int> currentWorkersStateDictionary = new Dictionary<ResourceSO, int>();
        public void ClearWorkersList() => currentWorkersStateList.Clear();
        public void ClearWorkersDictionary() => currentWorkersStateDictionary.Clear();
        public int GetWorkersAmount(ResourceSO resource) => currentWorkersStateDictionary.TryGetValue(resource, out var amount) ? amount : AddResourceToState(resource);
        private int AddResourceToState(ResourceSO resource)
        {
            Debug.Log($"Adding Null to Workers{resource.ID}");
            Managers.WorkersManager.Instance.AddNullWorkerToResource(resource);
            return 0;
        }
    }
}