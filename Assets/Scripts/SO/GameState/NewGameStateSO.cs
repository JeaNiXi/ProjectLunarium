using System;
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
        public List<NGPopState> NGPopStateList;

        [Header("Technology Data")]
        public List<NGTechState> NGTechStateList;
    }
    [Serializable]
    public class NGPopState
    {
        public RaceSO Race;
        public ulong ChildAmount;
        public ulong AdultAmount;
        public ulong ElderAmount;
        public NGPopState(
            RaceSO race,
            ulong childAmount,
            ulong adultAmount,
            ulong elderAmount)
        {
            Race = race;
            ChildAmount = childAmount;
            AdultAmount = adultAmount;
            ElderAmount = elderAmount;
        }
    }
    [Serializable]
    public class NGTechState
    {
        public List<TechnologySO> ResearchedTech;
        public NGTechState(List<TechnologySO> researchedTechList)
        {
            ResearchedTech = researchedTechList;
        }
    }
}