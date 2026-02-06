using SO;
using System.Collections.Generic;
namespace Managers
{
    /*
     *  Класс используется как менеджер рас и управление ими.
     */
    public class RaceManager
    {
        public RaceDatabaseSO RaceDatabase { get; private set; }
        public Dictionary<RaceDatabaseSO.RaceType, RaceSO> Races;
        public RaceManager(RaceDatabaseSO raceData)
        {
            RaceDatabase = raceData;
            Races = new Dictionary<RaceDatabaseSO.RaceType, RaceSO>();
            foreach (var race in raceData.Races)
            {
                Races.Add(race.RaceType, race.RaceSO);
            }
        }
        public bool TryGetRace(RaceDatabaseSO.RaceType raceType, out RaceSO race)
            => Races.TryGetValue(raceType, out race);
    }
}