using NUnit.Framework;
using SO;
using State;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Managers
{
    public class TechnologyManager : MonoBehaviour
    {
        public static TechnologyManager Instance;
        public TechnologyManagerSO TechnologyManagerSO;
        public TechnologyState TechnologyState;
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
            InitializeResearchedTechs(TechnologyManagerSO.startingTech);
        }
        private void InitializeTechState()
        {
            TechnologyState = new TechnologyState(TechnologyManagerSO);
        }
        private void InitializeResearchedTechs(List<TechnologySO> initResearchedTechsList)
            => TechnologyState.InitializeResearchedTechs(initResearchedTechsList);
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
        public bool IsTechResearchAvailable(TechnologySO tech)
        {
            // Add a check if the Tech is Next in Line to shorten check.
            if (IsTechnologyResearched(tech))
                return false;
            foreach (var techRequirement in tech.ResearchRequirements.TechnologiesRequiredToResearch)
                if (!IsTechnologyResearched(techRequirement))
                    return false;
            if (AreTechResearchResourcesAvailable(tech))
                return true;
            return false;
        }
        public bool IsTechnologyResearched(TechnologySO tech)
            => TechnologyState.IsTechnologyResearched(tech);
        public bool IsTechnologyInResearchProgress(TechnologySO tech)
            => TechnologyState.IsTechnologyInResearchProgress(tech);
        public bool AreTechResearchResourcesAvailable(TechnologySO tech)
        {
            foreach (var resourceRequirement in tech.ResearchRequirements.ResourceOneTimeCost)
                if (!ResourceManager.Instance.HasResourceAmount(resourceRequirement.Resource, resourceRequirement.Amount))
                    return false;
            Debug.Log($"Resources Available for: {tech.ID}");
            return true;
        }
        public string GetReseachButtonLabelText(TechnologySO tech)
        {
            if (IsTechnologyResearched(tech))
                return "Исследовано"; // Change to Localized String
            else
                return "Исследовать";
        }
        public bool IsTechnologyResearchInProgress()
            => GameManager.Instance.CurrentTechnologyResearchState == GameManager.TechnologyResearchState.RESEARCHING ? true : false;
        public TechnologySO GetCurrentResearchInProgressTechnology()
            => TechnologyState.GetCurrentResearchInProgressTechnology();
        public float GetCurrentReseachProgressBarValue()
            => TechnologyState.GetCurrentReseachProgressBarValue();
        public void UpdateTechResearchProgressBar()
        {
            if (GameManager.Instance.CurrentTechnologyResearchState == GameManager.TechnologyResearchState.NOT_RESEARCHING)
                return;
            TechnologyState.AddTechnologyResearchProgress(10f); // CalculateProgressHere
        }
        public void CheckForTechReseachStatus()
        {
            if (GameManager.Instance.IsResearchInProgress())
                TechnologyState.CheckForTechReseachStatus();
        }
        public void QueueTechnologyResearch(TechnologySO tech)
        {
            TechnologyState.StartTechResearch(tech);
            GameManager.Instance.SetTechnologyResearchState(GameManager.TechnologyResearchState.RESEARCHING);
        }
    }
}