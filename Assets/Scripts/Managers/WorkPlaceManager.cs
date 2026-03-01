using SO;
using State;
using System.Collections.Generic;
using UnityEngine;
namespace Managers
{
    public class WorkPlaceManager : MonoBehaviour
    {
        public static WorkPlaceManager Instance { get; private set; }

        [field: SerializeField] public WorkPlaceManagerSO WorkPlaceMainManagerSO { get; private set; }

        [field: SerializeField] public WorkPlaceSO DefaultWorkPlace { get; private set; }

        private Dictionary<string, WorkPlaceCategorySO> WorkPlaceCategories;

        [Header("Localization")]
        [SerializeField] private UIWorkPlaceLocalizationSO uiWorkPlaceLocalizationSO;

        private WorkPlaceState MainWorkPlaceState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeDataOnAwake();
            InitializeMainData();
        }
        private void InitializeDataOnAwake()
        {
            WorkPlaceCategories = new Dictionary<string, WorkPlaceCategorySO>();
            MainWorkPlaceState = new WorkPlaceState(WorkPlaceMainManagerSO);
        }

        private void InitializeMainData()
        {
            foreach (var workPlaceCategory in WorkPlaceMainManagerSO.WorkPlaceCategories)
            {
                WorkPlaceCategories.Add(workPlaceCategory.CategoryID, workPlaceCategory);
            }
        }
        public bool GetWorkPlaceCategory(string categoryID, out WorkPlaceCategorySO category)
            => WorkPlaceCategories.TryGetValue(categoryID, out category);
        public bool IsWorkPlaceAvailable(string workPlaceID)
            => MainWorkPlaceState.IsWorkPlaceAvailable(workPlaceID);

        public void AddWorkersToWorkPlace(ulong population) =>
            MainWorkPlaceState.AddWorkersToDefaultWorkPlace(population);

        public void TryMoveWorkerToWorkPlace(string workPlace, ulong amount = 1)
        {
            if (workPlace == DefaultWorkPlace.ID)
                return;
            if (!PopulationManager.Instance.GetActivePopulation(out ulong totalAdults))
                return;
            ulong currentlyBusy = MainWorkPlaceState.GetTotalAssignedButThisWorkers(DefaultWorkPlace.ID);

            if (currentlyBusy + amount > totalAdults)
                return;

            if (MainWorkPlaceState.GetWorkPlace(DefaultWorkPlace.ID, out var defaultWP) &&
                MainWorkPlaceState.GetWorkPlace(workPlace, out var targetWP))
            {
                ulong canAdd = (ulong)Mathf.Min(targetWP.GetAvailableCapacity(), (float)amount);
                ulong toMove = (ulong)Mathf.Min(defaultWP.GetWorkersAmount(), (float)canAdd);

                if (toMove > 0)
                {
                    defaultWP.RemoveWorkers(toMove);
                    targetWP.AddWorkers(toMove);
                }

            }
        }
        public void TryRemoveWorkersFromWorkPlace(string workPlace, ulong amount = 1)
        {
            if (workPlace == DefaultWorkPlace.ID)
                return;
            if (MainWorkPlaceState.GetWorkPlace(workPlace, out var targetWorkPlace))
            {
                ulong availableToRemove = (ulong)Mathf.Min(targetWorkPlace.GetWorkersAmount(), (float)amount);
                if (availableToRemove > 0)
                {
                    targetWorkPlace.RemoveWorkers(availableToRemove);
                    MainWorkPlaceState.AddWorkersToDefaultWorkPlace(availableToRemove);
                }
            }
        }
        public List<WorkPlace> GetAllWorkPlaces() =>
            MainWorkPlaceState.GetAllWorkPlaces();
        public ulong GetCurrentWorkersAmount(WorkPlaceSO workPlace) =>
            MainWorkPlaceState.GetCurrentWorkersAmount(workPlace.ID);
        public ulong GetMaxCapacity(WorkPlaceSO workPlace) =>
            MainWorkPlaceState.GetMaxCapacity(workPlace.ID);
        public void SetWorkPlaceProductionTypeMode(string id, WorkPlaceTypeSO newWorkPlaceProductionTypeMode) =>
            MainWorkPlaceState.SetWorkPlaceProductionTypeMode(id, newWorkPlaceProductionTypeMode);
        public WorkPlaceTypeSO GetWorkPlaceProductionTypeMode(string id) =>
            MainWorkPlaceState.GetWorkPlaceProductionTypeMode(id);
        public UIWorkPlaceLocalizationSO GetWorkPlaceLocalizationSO() =>
            uiWorkPlaceLocalizationSO;

        public void OnGlobalTick()
        {
            MainWorkPlaceState.Produce();
        }
    }
}