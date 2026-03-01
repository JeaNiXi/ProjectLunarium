using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class RaceRow
    {
        public RaceSO Race;
        public ulong Population;
        public ulong ChildrenAmount;
        public ulong AdultsAmount;
        public ulong EldersAmount;
        public ulong ActivePopulationAmount;
        public ulong DependablePopulationAmount;
    }
    public class PopulationRaceGroup
    {
        private RaceSO race;
        private ulong childAmount;
        private ulong adultAmount;
        private ulong elderAmount;
        private ulong activeAmount;
        private ulong inactiveAmount;
        private ulong dependablesAmount;

        private ulong totalPopulation;

        public PopulationRaceGroup(
            RaceSO race,
            ulong childAmount,
            ulong adultAmount,
            ulong elderAmount)
        {
            this.race = race;
            this.childAmount = childAmount;
            this.adultAmount = adultAmount;
            this.elderAmount = elderAmount;

            totalPopulation = CalculateTotalPopulation();
            activeAmount = CalculateActivePopulation();
            inactiveAmount = CalculateInactiveAmount();
            dependablesAmount = CalculateDependablesPopulation();
        }
        private ulong CalculateTotalPopulation() =>
            childAmount + adultAmount + elderAmount;
        public ulong GetTotalPopAmount() =>
            totalPopulation;
        private ulong CalculateActivePopulation() =>
            adultAmount;
        private ulong CalculateInactiveAmount() =>
            totalPopulation - activeAmount;
        private ulong CalculateDependablesPopulation()
            => childAmount + elderAmount;

        public ulong GetChildAmount()
            => childAmount;
        public ulong GetAdultAmount()
            => adultAmount;
        public ulong GetElderAmount()
            => elderAmount;
        public ulong GetActiveAmount() =>
            activeAmount;
        public ulong GetInactiveAmount() =>
            inactiveAmount;
        public ulong GetDependablesAmount()
            => dependablesAmount;
        public RaceSO GetRaceSO()
            => race;
    }
    public class PopulationState
    {
        public event Action<bool, ulong> OnTotalPopulationChanged;
        public event Action<bool, ulong> OnActivePopulationChanged;
        public event Action<bool, ulong> OnInactivePopulationChanged;

        private Dictionary<RaceSO, PopulationRaceGroup> allRacesDictionary;
        private List<PopulationRaceGroup> raceGroups;

        public PopulationState()
        {
            allRacesDictionary = new Dictionary<RaceSO, PopulationRaceGroup>();
            raceGroups = new List<PopulationRaceGroup>();
        }
        public bool InitializePopulation(List<NGPopState> popStateList)
        {
            Debug.Log("[PopulationManager -> PopulationState] Initializing Population From NGPopState!");
            foreach (var popGroup in popStateList)
            {
                var newRaceGroup = new PopulationRaceGroup(
                    popGroup.Race,
                    popGroup.ChildAmount,
                    popGroup.AdultAmount,
                    popGroup.ElderAmount);
                raceGroups.Add(newRaceGroup);
                allRacesDictionary.Add(popGroup.Race, newRaceGroup);
            }
            OnTotalPopulationChanged?.Invoke(GetCurrentPopulation(out ulong population), population);
            OnActivePopulationChanged?.Invoke(GetCurrentActivePopulation(out ulong activePopulation), activePopulation);
            OnInactivePopulationChanged?.Invoke(GetCurrentInactivePopulation(out ulong activeInactivePopulation), activeInactivePopulation);
            return true;
        }
        public bool GetCurrentPopulation(out ulong population) =>
            GetPopAmount(g => g.GetTotalPopAmount(), out population);
        public bool GetCurrentInactivePopulation(out ulong population) =>
            GetPopAmount(g => g.GetInactiveAmount(), out population);
        public bool GetCurrentActivePopulation(out ulong population) =>
            GetPopAmount(g => g.GetActiveAmount(), out population);

        public bool GetPopAmount(Func<PopulationRaceGroup, ulong> popSelector, out ulong population)
        {
            population = 0;
            foreach (var popGroup in raceGroups)
                population += popSelector(popGroup);
            return population > 0;
        }
        public bool GetAllPopulationGroupsData(out List<PopulationRaceGroup> allPopGroupsData)
        {
            allPopGroupsData = raceGroups;
            return true;
        }












        private PopulationStateSO populationStateSO;
        private PopulationAgeDistribution populationAgedistribution;

















        //public PopulationState()
        //{
        //    currentPopulation = STARTING_POPULATION;
        //    populationStateSO = Resources.Load<PopulationStateSO>("SO/PopulationState");
        //    populationAgedistribution = new PopulationAgeDistribution(currentPopulation);
        //    Debug.Log($"Initializing Population. Current Population: " +
        //        $"Childer: {populationAgedistribution.GetChildPopulationAmount()}, " +
        //        $"Adults: {populationAgedistribution.GetAdultPopulationAmount()}, " +
        //        $"Elders: {populationAgedistribution.GetElderPopulationAmount()}.");
        //    InitializePopulationStateSO();
        //}
        public enum RaceType
        {
            Human,
            Elf,
            Dwarf
        }
        public enum PopulationType
        {
            Children,
            Adults,
            Elders
        }
        //public int CurrentPopulation { get { return currentPopulation; } private set { } }
        //public void AddPopulation(int population) => currentPopulation += population;
        //private void InitializePopulationStateSO() => populationStateSO.SetCurrentPopulation(currentPopulation);
        public PopulationAgeDistribution GetPopulationAgeDistribution() => populationAgedistribution;
    }
    public class PopulationAgeDistribution
    {
        private int dependables;
        private int childPopulation;
        private int adultPopulation;
        private int elderPopulation;
        public PopulationAgeDistribution()
        {
            dependables = 0;
            childPopulation = 0;
            adultPopulation = 0;
            elderPopulation = 0;
        }
        public PopulationAgeDistribution(int population)
        {
            childPopulation = Mathf.RoundToInt(population * 0.3f);
            elderPopulation = Mathf.RoundToInt(population * 0.1f);
            dependables = childPopulation + elderPopulation;
            adultPopulation = population - dependables;
        }
        public int GetDependablesAmount() => dependables;
        public int GetChildPopulationAmount() => childPopulation;
        public int GetAdultPopulationAmount() => adultPopulation;
        public int GetElderPopulationAmount() => elderPopulation;
    }

}