using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "NewGameState", menuName = "Scriptable Objects/State/New Game State")]
    public class NewGameStateSO : ScriptableObject
    {
        [Header("Info")]
        public string StartIDString;

        [Header("Race Data")]
        public List<RaceSO> AllStartingRaces;
        public RaceSO MainRace;

        [Header("Population Data")]
        public int MainRaceChildPopulation;
        public int MainRaceAdultPopulation;
        public int MainRaceElderPopulation;

        [Header("Technology Data")]
        public List<TechnologySO> ReaserchedTechnologies;
    }
}