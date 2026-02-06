using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    /*
     *  Класс используется как хаб для всех рас игры и доступ к ним.
     */
    [CreateAssetMenu(fileName = "RaceDatabase", menuName = "Scriptable Objects/Population/Races/RaceDatabase")]
    public class RaceDatabaseSO : ScriptableObject
    {
        public enum RaceType
        {
            Default,
            Human,
            Elf
        }
        public List<Race> Races = new List<Race>();
    }
    [Serializable]
    public class Race
    {
        public RaceDatabaseSO.RaceType RaceType;
        public RaceSO RaceSO;
    }
}