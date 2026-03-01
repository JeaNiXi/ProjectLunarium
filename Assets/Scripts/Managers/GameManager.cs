using SO;
using State;
using System;
using UnityEngine;
namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public readonly string GameVersion = "0.1";
        /*
         *  »спользуем дл€ начала новой игры или загрузки игры.
         */
        public enum GameState
        {
            INIT,
            PAUSE,
            RUNNING
        }
        public event Action<GameState> OnGameStateChanged;

        public bool IsTickPossible { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            CurrentGameState = GameState.INIT;
            CurrentTechnologyResearchState = TechnologyResearchState.NOT_RESEARCHING;
        }
        public void StartNewGame(NewGameState newGameState)
        {
            Debug.Log("[GameManager] New Game Started!");
            PopulationManager.Instance.InitializePopulation(newGameState.GetNGPopState());
            WorkersManager.Instance.InitializeInitialWorkers();
            SetGameState(GameState.RUNNING);
            Debug.Log("[GameManager] Setting GameState to -RUNNING-");
            EnableTickPossibility();
            Debug.Log("[GameManager] Enabling Tick Possibility!");
            AudioManager.Instance.PlayBackgroundAgeMusic();
        }
        public void SetGameState(GameState state)
        {
            CurrentGameState = state;
            NotifyGameStateChange(state);
        }
        private void NotifyGameStateChange(GameState state)
        {
            OnGameStateChanged?.Invoke(state);
        }
        public void EnableTickPossibility()
            => IsTickPossible = true;
        public void DisableTickPossibility()
            => IsTickPossible = false;








        //public void StartGame()
        //{
        //    SetGameState(GameState.RUNNING);
        //    EnableTickPossibility();
        //}
        //public void StartGame(GameDataState gameState)
        //{
        //    SetGameState(GameState.RUNNING);
        //    EnableTickPossibility();
        //}

        public void OnGlobalTick(TimeState timeState)
        {
            WorkPlaceManager.Instance.OnGlobalTick();
            ResourceManager.Instance.OnGlobalTick();
            TechnologyManager.Instance.OnGlobalTick();
            /*
             * On each day the first thing that should be done is updating and checking for events and meta gameplay.
             * After that we apply event modifiers if needed. 
             * After this we calculate all our current modifiers for this DAY.
             * After this we Calculate our Population based on modifiers.
             * Research. Magic. Society.
             * And the Last thing are Resources.
             * At this stage we calculate the current DAY resource income without taking into account any button presses at all. They all are scheduled.
             * Now we calculate the resources outcome. 
             * Now we check for deficit etc. 
             * And now we generate the end result and update our resources finale.
             * All updates on button presses should be collected and scheduled for the final prep for the last, before we allow a new tick.
             * They start working from the next day.
             */
            //ResourceManager.Instance.UpdateGathererResourceIncome(18); //Need to give gatherer Amount in future.
            //ResourceManager.Instance.UpdateResourceUsage(9, 18, 3); // Need to give populations and other in future.
            //TechnologyManager.Instance.UpdateTechResearchProgressBar();
            //TechnologyManager.Instance.CheckForTechReseachStatus();
            //TechnologyManager.Instance.OnGlobalTick(timeState);
            //ResourceManager.Instance.OnGlobalTick(timeState);
            EnableTickPossibility();
        }





































        public enum TechnologyResearchState
        {
            NOT_RESEARCHING,
            RESEARCHING
        }
        public GameState CurrentGameState { get; private set; }
        public TechnologyResearchState CurrentTechnologyResearchState { get; private set; }

        public bool IsVisibleResourcesUpdateNeeded { get; private set; }

        public void SetTechnologyResearchState(TechnologyResearchState state) => CurrentTechnologyResearchState = state;
        public void SetIsVisibleResourcesUpdateNeeded(bool value)
            => IsVisibleResourcesUpdateNeeded = value;



        public bool IsResearchInProgress()
            => CurrentTechnologyResearchState == TechnologyResearchState.RESEARCHING ? true : false;
        private void InitializePopulation()
        {

        }

        public void QueueTechResearched(TechnologySO currentResearchInProgressTechnology)
        {
            Debug.Log("Congrats tech is researched! ");
        }
    }
}