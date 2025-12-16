using SO;
using State;
using UnityEngine;
namespace Managers
{
    public class TechnologyManager : MonoBehaviour
    {
        public static TechnologyManager Instance;
        public TechnologyManagerSO TechnologyManagerSO;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeTechState();
        }
        private void InitializeTechState()
        {
            TechnologyState technologyState = new TechnologyState(TechnologyManagerSO);
        }
        public void OnGlobalTick(TimeState timeState)
        {
            /*
             * First we look if we are actually researching something. If not, we just return and do nothing at all.
             * If we are researching, we check if we currently have enough resources to sustain our research. 
             * To do this we First check all our modifiers. Get them all together.
             * Then we update the cost for the tech based on all modifiers for THIS day.
             * Then we check if we can afford it. If yes, we update, if not we calculate the penalty, or stop the research.
             * On update we check if research complete, advance, etc.
             */
        }
    }
}