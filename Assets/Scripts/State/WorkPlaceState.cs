using Managers;
using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class WorkPlaceState
    {
        private Dictionary<string, WorkPlace> workPlaces;
        private WorkPlace defaultWorkPlace;

        public WorkPlaceState(WorkPlaceManagerSO config)
        {
            workPlaces = new Dictionary<string, WorkPlace>();
            foreach (var workCategory in config.WorkPlaceCategories)
            {
                foreach (var workPlace in workCategory.WorkPlaces)
                {
                    workPlaces.Add(workPlace.ID, new WorkPlace(workPlace));
                }
            }
            string defaultWorkPlaceID = WorkPlaceManager.Instance.DefaultWorkPlace.ID;
            workPlaces.TryGetValue(defaultWorkPlaceID, out defaultWorkPlace);
        }
        public bool GetWorkPlace(string ID, out WorkPlace workPlace) =>
            workPlaces.TryGetValue(ID, out workPlace);
        public List<WorkPlace> GetAllWorkPlaces()
        {
            return new List<WorkPlace>(workPlaces.Values);
            //var list = new List<WorkPlace>();
            //foreach (var workPlace in workPlaces.Values)
            //    list.Add(workPlace);
            //return list;
        }
        public bool IsWorkPlaceAvailable(string id)
            => workPlaces.TryGetValue(id, out var workPlace) && workPlace.IsAvailable;
        public ulong AddWorkersToDefaultWorkPlace(ulong workersAmount)
        {
            Debug.Log($"[WorkPlace Manager -> WorkPlace State] Adding Workers To Default Work Place! Amount = {workersAmount}");
            if (defaultWorkPlace == null)
                return workersAmount;
            return workersAmount - defaultWorkPlace.AddWorkers(workersAmount);
        }
        public ulong AddWorkersToWorkPlace(string ID, ulong workersAmount)
        {
            if (workPlaces.TryGetValue(ID, out var workPlace))
                return workersAmount - workPlace.AddWorkers(workersAmount);
            return workersAmount;
        }
        public ulong GetCurrentWorkersAmount(string id) =>
            workPlaces.TryGetValue(id, out var workPlace) ? workPlace.GetWorkersAmount() : 0;
        public ulong GetMaxCapacity(string id) =>
            workPlaces.TryGetValue(id, out var workPlace) ? workPlace.GetMaxCapacity() : 0;
        public void SetWorkPlaceProductionTypeMode(string id, WorkPlaceTypeSO newTypeMode)
        {
            if (workPlaces.TryGetValue(id, out var workPlace))
            {
                if (workPlace.CurrentWorkModeType == newTypeMode)
                    return;
                workPlace.SetWorkModeType(newTypeMode);
            }
        }
        public WorkPlaceTypeSO GetWorkPlaceProductionTypeMode(string id)
        {
            if (workPlaces.TryGetValue(id, out var workPlace))
                return workPlace.CurrentWorkModeType;
            else
                return null;
        }
        public ulong GetTotalAssignedButThisWorkers(string workPlaceID)
        {
            ulong total = 0;
            foreach (var workPlace in workPlaces.Values)
            {
                if (workPlace.ID != defaultWorkPlace.ID)
                {
                    total += workPlace.GetWorkersAmount();
                }
            }
            return total;
        }
        public void Produce()
        {
            foreach (var workPlace in GetAllWorkPlaces())
                workPlace.Produce();
        }
    }
    public class WorkPlace
    {
        public string ID { get; private set; }
        private WorkPlaceSO workPlaceSO;
        public WorkPlaceTypeSO CurrentWorkModeType { get; private set; }

        public ulong DefaultCapacity { get; private set; }
        public ulong CurrentCapacity { get; private set; }
        public ulong CurrentWorkersAmount { get; private set; }
        public bool IsAvailable { get; private set; } = true;


        public WorkPlace(WorkPlaceSO workPlace)
        {
            ID = workPlace.ID;
            workPlaceSO = workPlace;

            DefaultCapacity = workPlace.DefaultWorkPlaceCapacity;
            CurrentCapacity = DefaultCapacity;

            if (workPlace.WorkPlaceTypes != null && workPlace.WorkPlaceTypes.Count > 0)
                CurrentWorkModeType = workPlace.WorkPlaceTypes[0];
        }
        public ulong GetAvailableCapacity() => CurrentCapacity - CurrentWorkersAmount;
        public ulong GetMaxCapacity() => CurrentCapacity;
        public ulong GetWorkersAmount() => CurrentWorkersAmount;

        public ulong AddWorkers(ulong amount)
        {
            ulong space = GetAvailableCapacity();
            ulong added = Math.Min(space, amount);
            CurrentWorkersAmount += added;
            return added;
        }
        public ulong RemoveWorkers(ulong amount)
        {
            if (amount > CurrentWorkersAmount)
            {
                ulong remainder = amount - CurrentWorkersAmount;
                CurrentWorkersAmount = 0;
                return remainder;
            }
            else
            {
                CurrentWorkersAmount -= amount;
                return 0;
            }
        }
        public void SetWorkModeType(WorkPlaceTypeSO newMode)
        {
            if (workPlaceSO.WorkPlaceTypes.Contains(newMode))
                CurrentWorkModeType = newMode;
        }

        public void Produce()
        {
            Debug.Log("Calling Produce");
            if (CurrentWorkModeType == null || CurrentWorkersAmount == 0)
                return;
            if (CurrentWorkModeType.ProducedResources == null)
                return;

            foreach (var resourceToProduce in CurrentWorkModeType.ProducedResources)
            {
                if (resourceToProduce.Resource == null)
                    continue;
                int totalProduction = (int)(resourceToProduce.Amount * CurrentWorkersAmount);
                ResourceManager.Instance.AddResource(resourceToProduce.Resource, totalProduction);
            }
        }
    }
}