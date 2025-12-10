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
    }
}