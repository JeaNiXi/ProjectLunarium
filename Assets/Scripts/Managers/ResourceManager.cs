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
        private ResourceState resourceState;

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
        public void InitializeStartingResourcesState() => resourceState = new ResourceState(ResourceManagerSO);
        public List<ResourceSO> GetAllResourcesList() => ResourceManagerSO.AllResourcesList;
        public void OnGlobalTick(TimeState timeState)
        {

            /*
             * We check here what are the resources we collect first. 
             * All this should be done After technologies and modifiers are in line. (Later Magic, Society, Population).
             * Basically this shlould be the last step, duh. Every other step before this should already have updated everything.
             */
        }
        public void UpdateGathererResourceIncome(int gatherersAmount)
        {
            resourceState.UpdateGatherersResourcesIncomes(gatherersAmount);
        }
        public void UpdateResourceUsage(int childAmount, int adultAmount, int elderAmount)
        {
            resourceState.UpdateResourceUsage(childAmount, adultAmount, elderAmount);
        }    
    }
}