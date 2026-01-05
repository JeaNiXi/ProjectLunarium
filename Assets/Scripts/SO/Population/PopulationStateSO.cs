using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "PopulationState", menuName = "Scriptable Objects/Population/Population State")]
    public class PopulationStateSO : ScriptableObject
    {
        [field: SerializeField] public int CurrentPopulation { get; private set; }
        public void SetCurrentPopulation(int population) => CurrentPopulation = population;
        [field: SerializeField] public List<RacePopulationData> Races { get; private set; }
    }
    [System.Serializable]
    public class RacePopulationData
    {
        public State.PopulationState.RaceType Race;
        public ulong Population;
        public float Happiness;
        public ulong ChildrenAmount;
        public ulong AdultsAmount;
        public ulong EldersAmount;
        public ulong ActivePopulationAmount;
        public ulong DependablePopulationAmount;
        public List<string> Modifiers;
    }
}