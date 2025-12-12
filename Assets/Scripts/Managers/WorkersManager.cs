using SO;
using State;
using UnityEngine;
namespace Managers
{
    public class WorkersManager : MonoBehaviour
    {
        public static WorkersManager Instance;
        public WorkersManagerSO WorkersManagerSO;
        WorkersState workersState;
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
            workersState = new WorkersState(WorkersManagerSO);
        }
        public void AddNullWorkerToResource(ResourceSO resource) => workersState.AddEmptyWorker(resource);
        public void AddWorkerToResource(ResourceSO resource)
        {
            // To Add Later.
        }
    }
}