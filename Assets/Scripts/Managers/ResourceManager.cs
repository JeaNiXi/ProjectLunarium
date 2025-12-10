using SO;
using State;
using UnityEngine;
namespace Managers
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance;
        public ResourceManagerSO ResourceManagerSO;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeStartingResourcesState();
        }
        public void InitializeStartingResourcesState()
        {
            ResourceState resourceState = new ResourceState(ResourceManagerSO);
        }
        public bool IsResourceResearched(ResourceSO resource)
        {
            return true;
        }

    }
}