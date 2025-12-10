using SO;
using System.Collections.Generic;
using UnityEngine;
namespace State
{
    public class TechnologyState
    {
        private TechnologyStateSO technologyStateSO;
        private Dictionary<TechnologySO, bool> TechResearchStatus = new Dictionary<TechnologySO, bool>();

        public TechnologyState(TechnologyManagerSO technologyManagerSO)
        {
            int index = 0;
            foreach (var tech in technologyManagerSO.allTechnologies)
            {
                TechResearchStatus.Add(tech, false);
                index++;
            }
            Debug.Log($"Loaded {index} technologies!");
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            SetStartingResource(technologyManagerSO);
            UpdateStateSO();
        }
        private void SetStartingResource(TechnologyManagerSO technologyManagerSO)
        {
            if (TechResearchStatus.ContainsKey(technologyManagerSO.startingTech))
            {
                TechResearchStatus[technologyManagerSO.startingTech] = true;
                Debug.Log($"Starting Tech is: {technologyManagerSO.startingTech.ID}");
            }
        }
        private void UpdateStateSO()
        {
            if (technologyStateSO != null)
                technologyStateSO.ClearResearchedTechList();
            foreach ((TechnologySO key, bool value) in TechResearchStatus)
            {
                Debug.Log($"Tech: {key.ID}, IsResearched: {value} ");
                if (value == true)
                    technologyStateSO.researchedTechnologies.Add(key);
            }
        }
    }
}