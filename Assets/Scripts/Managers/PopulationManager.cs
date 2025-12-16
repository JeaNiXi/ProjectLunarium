using State;
using UnityEngine;
namespace Managers
{
    public class PopulationManager : MonoBehaviour
    {
        public static PopulationManager Instance;
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
            InitializePopulationState();
        }   
        private void InitializePopulationState()
        {
            populationState = new PopulationState();
        }
        public void OnGlobalTick(TimeState timeState)
        {

        }
    }
}