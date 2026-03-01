using SO;
using State;
using UnityEngine;
namespace Managers
{
    public class WorkersManager : MonoBehaviour
    {
        public static WorkersManager Instance;
        public WorkersManagerSO WorkersManagerSO;
        WorkersState WorkersState;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeWorkersState();
        }
        private void InitializeWorkersState()
        {
            WorkersState = new WorkersState(WorkersManagerSO);
        }

        public void InitializeInitialWorkers() =>
            AddWorkersToWorkPlace(PopulationManager.Instance.GetActivePopulation(out ulong population) ? population : 0);
        public void AddWorkersToWorkPlace(ulong workersAmount) =>
            WorkPlaceManager.Instance.AddWorkersToWorkPlace(workersAmount);
        public void AddWorkerToResource(ResourceSO resource) => WorkersState.AddWorkerToResource(resource);
        public void UpdateWorkersAmount(int workingPopulation) => WorkersState.InitializeWorkerTypesFromWorkerPopulation(workingPopulation);
    }
}