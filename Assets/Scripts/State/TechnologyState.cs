using Managers;
using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace State
{
    public class TechnologyState
    {
        private TechnologyStateSO technologyStateSO;
        private Dictionary<TechnologySO, bool> techResearchStatus = new Dictionary<TechnologySO, bool>();
        private TechnologySO currentResearchInProgressTechnology;
        private Dictionary<TechnologySO, float> techResearchProgressDictionary = new Dictionary<TechnologySO, float>();
        public List<TechnologySO> ResearchedTechs { get; private set; }

        public TechnologyState(TechnologyManagerSO technologyManagerSO)
        {
            ResearchedTechs = new List<TechnologySO>();
            int index = 0;
            foreach (var tech in technologyManagerSO.allTechnologies)
            {
                techResearchStatus.Add(tech, false);
                index++;
            }
            Debug.Log($"Loaded {index} technologies!");
            technologyStateSO = Resources.Load<TechnologyStateSO>("SO/TechnologyState");
            //SetStartingResource(technologyManagerSO);
            UpdateStateSO();
        }
        public void StartTechResearch(TechnologySO tech)
        {
            if (currentResearchInProgressTechnology == null)
                currentResearchInProgressTechnology = tech;
            if (!techResearchProgressDictionary.ContainsKey(tech))
            {
                techResearchProgressDictionary.Add(tech, 0);
                currentResearchInProgressTechnology = tech;
            }
            else
            {
                currentResearchInProgressTechnology = tech;
            }
        }
        public void AddTechnologyResearchProgress(float progress)
        {
            techResearchProgressDictionary[currentResearchInProgressTechnology] += progress;
        }
        public void CheckForTechReseachStatus()
        {
            if (techResearchProgressDictionary[currentResearchInProgressTechnology]>=100f) //Change To normal verification
            {
                ResearchedTechs.Add(currentResearchInProgressTechnology);
                technologyStateSO.researchedTechnologies.Add(currentResearchInProgressTechnology); // Temp
                GameManager.Instance.SetTechnologyResearchState(GameManager.TechnologyResearchState.NOT_RESEARCHING);
                GameManager.Instance.SetIsVisibleResourcesUpdateNeeded(true);
                GameManager.Instance.QueueTechResearched(currentResearchInProgressTechnology);
            }
        }
        public float GetCurrentReseachProgressBarValue()
            => techResearchProgressDictionary[currentResearchInProgressTechnology];
        public TechnologySO GetCurrentResearchInProgressTechnology()
            => currentResearchInProgressTechnology;
        public void InitializeResearchedTechs(List<TechnologySO> initResearchedTechsList)
        {
            foreach (var tech in initResearchedTechsList)
            {
                if (techResearchStatus.ContainsKey(tech))
                {
                    techResearchStatus[tech] = true;
                    ResearchedTechs.Add(tech);
                    Debug.Log($"Added to Researched: {tech}");
                }
            }
        }
        public bool IsTechnologyResearched(TechnologySO tech)
            => ResearchedTechs.Contains(tech);
        public bool IsTechnologyInResearchProgress(TechnologySO tech)
        {
            if (currentResearchInProgressTechnology == null || currentResearchInProgressTechnology != tech)
                return false;
            return true;
        }

        //private void SetStartingResource(TechnologyManagerSO technologyManagerSO)
        //{
        //    if (techResearchStatus.ContainsKey(technologyManagerSO.startingTech))
        //    {
        //        techResearchStatus[technologyManagerSO.startingTech] = true;
        //        Debug.Log($"Starting Tech is: {technologyManagerSO.startingTech.ID}");
        //    }
        //}
        private void UpdateStateSO()
        {
            if (technologyStateSO != null)
                technologyStateSO.ClearResearchedTechList();
            foreach ((TechnologySO key, bool value) in techResearchStatus)
            {
                Debug.Log($"Tech: {key.ID}, IsResearched: {value} ");
                if (value == true)
                    technologyStateSO.researchedTechnologies.Add(key);
            }
        }


    }
}