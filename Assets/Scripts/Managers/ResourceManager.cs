using SO;
using State;
using System.Collections.Generic;
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
        public List<ResourceSO> GetAllResourcesList() => ResourceManagerSO.AllResourcesList;
    }
}