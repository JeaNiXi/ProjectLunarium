using SO;
using UnityEngine;
namespace State
{
    public class PopulationState
    {
        private const int STARTING_POPULATION = 30;
        
        private int currentPopulation;
        private PopulationStateSO populationStateSO;
        private PopulationAgeDistribution populationAgedistribution;
        public PopulationState()
        {
            currentPopulation = STARTING_POPULATION;
            populationStateSO = Resources.Load<PopulationStateSO>("SO/PopulationState");
            populationAgedistribution = new PopulationAgeDistribution(currentPopulation);
            Debug.Log($"Initializing Population. Current Population: " +
                $"Childer: {populationAgedistribution.GetChildPopulationAmount()}, " +
                $"Adults: {populationAgedistribution.GetAdultPopulationAmount()}, " +
                $"Elders: {populationAgedistribution.GetElderPopulationAmount()}.");
            InitializePopulationStateSO();
        }
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
        public int CurrentPopulation { get { return currentPopulation; } private set { } }
        public void AddPopulation(int population) => currentPopulation += population;
        private void InitializePopulationStateSO() => populationStateSO.SetCurrentPopulation(currentPopulation);
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
    public class RaceRow
    {
        public PopulationState.RaceType Race;
        public ulong Population;
        public float Happiness;
        public ulong ChildrenAmount;
        public ulong AdultsAmount;
        public ulong EldersAmount;
        public ulong ActivePopulationAmount;
        public ulong DependablePopulationAmount;
        public string Modifiers;
    }
}