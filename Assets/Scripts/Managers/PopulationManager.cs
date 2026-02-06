using SO;
using State;
using UnityEngine;
namespace Managers
{
    /*
     *   ласс используетс€ как основной менеджер всей попул€ции со своими саб менеджарами.
     */
    public class PopulationManager : MonoBehaviour
    {
        public static PopulationManager Instance;
        public RaceDatabaseSO RaceDatabaseSO;
        private RaceManager raceManager;
        private PopulationState populationState;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeSubManagers();
            InitializePopulationState();
        }   
        private void InitializeSubManagers()
        {
            raceManager = new RaceManager(RaceDatabaseSO);
        }
        private void InitializePopulationState()
        {
            populationState = new PopulationState();
        }
        public bool TryGetRace(RaceDatabaseSO.RaceType raceType, out RaceSO race)
            => raceManager.TryGetRace(raceType, out race);
        public void OnGlobalTick(TimeState timeState)
        {

        }
    }
}