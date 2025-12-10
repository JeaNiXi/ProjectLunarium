using SO;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class TechnologyState : MonoBehaviour
    {
        private TechnologyStateSO technologyStateSO;
        private Dictionary<TechnologySO, bool> TechResearchStatus = new Dictionary<TechnologySO, bool>();

        public TechnologyState(TechnologyManagerSO technologyManagerSO)
        {
            foreach (var tech in technologyManagerSO.allTechnologies)
            {
                TechResearchStatus.Add(tech, false);
            }
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            SetStartingResource(technologyManagerSO);
            UpdateStateSO();
        }
        private void SetStartingResource(TechnologyManagerSO technologyManagerSO)
        {
            if (TechResearchStatus.TryGetValue(technologyManagerSO.startingTech, out var value))
            {
                value = true;
            }
        }
        private void UpdateStateSO()
        {
            foreach (var key in TechResearchStatus.Keys)
                if (TechResearchStatus[key] == true)
                    technologyStateSO.researchedTechnologies.Add(key);
        }
    }
}