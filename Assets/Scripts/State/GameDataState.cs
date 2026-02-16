using SO;
using System;
using System.Collections.Generic;

namespace State
{
    /*
     *  Класс используется для создания состояния игры, которое используется чтобы начать новую игру, или загрузить данные из сохранения.
     */
    public class GameDataState
    {
        public GameData GameData;
        public KingdomStateData KingdomStateData;
        public TechnologyStateData TechnologyStateData;
        public GameDataState(
            GameData gameData,
            KingdomStateData kingdomStateData,
            TechnologyStateData technologyStateData)
        {
            GameData = gameData;
            KingdomStateData = kingdomStateData;
            TechnologyStateData = technologyStateData;
        }
    }
    public class GameData
    {
        public string GameVersion { get; private set; }
        public GameData(string gameVersion)
        {
            GameVersion = gameVersion;
        }
    }
    public class KingdomStateData
    {
        public string KingdomName { get; private set; }
        public KingdomStateData(string kingdomName)
        {
            KingdomName = kingdomName;
        }
    }
    public class TechnologyStateData
    {
        public List<TechnologySO> ResearchedTechnologies { get; private set; }
        public TechnologyStateData(List<TechnologySO> technologiesList)
        {
            ResearchedTechnologies = technologiesList;
        }
    }
    public class NewGameState
    {
        public RaceSO SelectedRace { get; private set; }
        public int TotalPopulation { get; private set; }
        public int ChildPopultaion { get; private set; }
        public int AdultPopulation { get; private set; }
        public int ElderPopulation { get; private set; }
        public List<TechnologySO> ResearchedTechnologies { get; private set; }

        public bool IsDataCompleted =>
            SelectedRace != null;

        public event Action OnStateChanged;
        public void SetStartingRace(RaceSO race)
        {
            SelectedRace = race;
            Notify();
        }
        public void Notify()
            => OnStateChanged?.Invoke();
    }
}