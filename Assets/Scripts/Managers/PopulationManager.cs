using SO;
using State;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Managers
{
    /*
     *   ласс используетс€ как основной менеджер всей попул€ции со своими саб менеджарами.
     */
    public class PopulationManager : MonoBehaviour
    {
        public static PopulationManager Instance;
        public RaceDatabaseSO RaceDatabaseSO;
        private RaceManager raceManager;
        private PopulationState populationState;

        public event Action<bool, ulong> OnPopulationChanged;
        public event Action<bool, ulong> OnActivePopChanged;
        public event Action<bool, ulong> OnInactivePopChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeSubManagers();
            InitializePopulationState();
        }
        private void InitializeSubManagers()
        {
            raceManager = new RaceManager(RaceDatabaseSO);
        }
        private void InitializePopulationState()
        {
            populationState = new PopulationState();
            populationState.OnTotalPopulationChanged += PopulationState_OnPopulationChanged;
            populationState.OnActivePopulationChanged += PopulationState_OnActivePopulationChanged;
            populationState.OnInactivePopulationChanged += PopulationState_OnInactivePopulationChanged;
        }
        private void PopulationState_OnPopulationChanged(bool value, ulong popAmount) =>
            OnPopulationChanged?.Invoke(value, popAmount);
        private void PopulationState_OnActivePopulationChanged(bool value, ulong activeAmount) =>
            OnActivePopChanged?.Invoke(value, activeAmount);
        private void PopulationState_OnInactivePopulationChanged(bool value, ulong inactiveAmount) =>
            OnInactivePopChanged?.Invoke(value, inactiveAmount);

        public bool InitializePopulation(List<NGPopState> popStateList)
            => populationState.InitializePopulation(popStateList);
        public bool GetCurrentPopulation(out ulong population)
            => populationState.GetCurrentPopulation(out population);
        public bool GetAllPopulationGroupsData(out List<PopulationRaceGroup> allPopGroupsData)
            => populationState.GetAllPopulationGroupsData(out allPopGroupsData);
        public bool GetActivePopulation(out ulong population) =>
            populationState.GetCurrentActivePopulation(out population);








        public bool TryGetRace(RaceDatabaseSO.RaceType raceType, out RaceSO race)
            => raceManager.TryGetRace(raceType, out race);
        public void OnGlobalTick(TimeState timeState)
        {

        }
    }
}