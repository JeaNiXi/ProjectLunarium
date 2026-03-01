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
        public event Action OnStateChanged;
        private List<NGPopState> ngPopStateList;
        private List<NGTechState> ngTechStateList;

        public NewGameState()
        {
            ngPopStateList = new List<NGPopState>();
            ngTechStateList = new List<NGTechState>();
        }
        public NewGameState(NewGameStateSO newGameStateSO)
        {
            ngPopStateList = new List<NGPopState>();
            ngTechStateList = new List<NGTechState>();
            ngPopStateList = newGameStateSO.NGPopStateList;
            ngTechStateList = newGameStateSO.NGTechStateList;
        }

        public bool IsDataCompleted =>
            ngPopStateList != null &&
            ngTechStateList != null;
        public void SetNGPopState(List<NGPopState> popStateList)
        {
            ngPopStateList = popStateList;
            OnStateChanged?.Invoke();
        }
        public List<NGPopState> GetNGPopState()
            => ngPopStateList;
        public void SetNGTechState(List<NGTechState> techStateList)
        {
            ngTechStateList = techStateList;
            OnStateChanged?.Invoke();
        }
        public List<NGTechState> GetNGTechState()
            => ngTechStateList;
    }
}