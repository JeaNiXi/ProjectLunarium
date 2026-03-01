using SO;
using State;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Managers
{
    public class TechnologyManager : MonoBehaviour
    {
        public static TechnologyManager Instance;
        public TechnologyManagerSO TechnologyManagerSO;
        public TechnologyState TechState;

        public event Action OnOfferedTechsRefreshedEvent;

        public int MaxOfferedTechs = 3;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitializeData();
            InitializeStartingTechs(TechnologyManagerSO.startingTech);
        }
        private void InitializeData()
        {
            TechState = new TechnologyState(TechnologyManagerSO);
            TechState.OnOfferedTechsRefresh += TechState_OnOfferedTechsRefresh;
            Debug.Log("[TechManager] Initializing Tech State");
        }

        private void TechState_OnOfferedTechsRefresh()
        {
            OnOfferedTechsRefreshedEvent?.Invoke();
        }

        public void InitializeStartingTechs(List<TechnologySO> startingTechs)
        {
            foreach (var tech in startingTechs)
            {
                TechState.AddTechToResearched(tech);
                Debug.Log($"Added Starting Tech: {tech}");
            }
            InitializeFirstTimeStart();
        }
        public void InitializeFirstTimeStart() =>
            RefreshTechDeck();

        private void RefreshTechDeck()
        {
            TechState.ClearOffers();
            var pool = TechnologyManagerSO.allTechnologies
                .Where(t => !TechState.IsResearched(t))
                .Where(t => t != TechState.CurrentResearch)
                .Where(t => ArePrerequisitesMet(t))
                .Where(t => AreSpecialConditionsMet(t))
                .ToList();
            TechState.AddTechToOffered(PickRandomTechs(pool, MaxOfferedTechs));
        }
        public bool ArePrerequisitesMet(TechnologySO tech)
        {
            if (tech.ResearchRequirements == null)
                return true;
            if (tech.ResearchRequirements.TechPrerequisites == null || tech.ResearchRequirements.TechPrerequisites.Count == 0)
                return true;
            return tech.ResearchRequirements.TechPrerequisites.All(p => TechState.IsResearched(p));
        }
        public bool AreSpecialConditionsMet(TechnologySO tech)
        {
            // TODO LATER
            return true;
        }
        public List<TechnologySO> PickRandomTechs(List<TechnologySO> pool, int maxOfferedTechs)
        {
            if (pool.Count <= maxOfferedTechs)
                return pool;
            List<TechnologySO> selected = new();
            for (int i = 0; i < maxOfferedTechs; i++)
            {
                float totalWeight = pool.Sum(t => (float)t.BaseWeight);
                float randomValue = UnityEngine.Random.Range(0, totalWeight);
                float currentSum = 0;

                foreach (var tech in pool)
                {
                    currentSum += tech.BaseWeight;
                    if (randomValue <= currentSum)
                    {
                        selected.Add(tech);
                        pool.Remove(tech);
                        break;
                    }
                }
            }
            return selected;
        }
        public void AddResearchPoints(float amount)
        {
            if (TechState.CurrentResearch == null)
                return;
            TechnologySO current = TechState.CurrentResearch;
            if (!TechState.ResearchProgress.ContainsKey(current))
                TechState.ResearchProgress[current] = 0f;
            TechState.ResearchProgress[current] += amount;
            if (TechState.ResearchProgress[current] >= current.TotalResearchPoints)
                CompleteResearch(current);
        }
        public void CompleteResearch(TechnologySO tech)
        {
            TechState.AddTechToResearched(tech);
            if (tech.UnlockedResources != null)
            {
                ResourceManager.Instance.UpdateVisibleUIResources();
            }
            TechState.CurrentResearch = null;

            RefreshTechDeck();
        }
        public void StartResearch(TechnologySO tech)
        {
            if (tech == null)
                return;
            if (!AreResourcesAvailable(tech))
                return;
            SpendResources(tech);
            TechState.SetCurrentResearch(tech);
            TechState.RemoveTechFromOffered(tech);
        }
        public bool AreResourcesAvailable(TechnologySO tech)
        {
            if (tech.ResearchRequirements == null)
                return true;
            return tech.ResearchRequirements.ResourceOneTimeCost
                .All(r => ResourceManager.Instance.HasResourceAmount(r.Resource, (int)r.Amount));
        }

        private void SpendResources(TechnologySO tech)
        {
            if (tech.ResearchRequirements == null || tech.ResearchRequirements.ResourceOneTimeCost == null)
                return;
            foreach (var cost in tech.ResearchRequirements.ResourceOneTimeCost)
            {
                ResourceManager.Instance.SpendResource(cost.Resource, (int)cost.Amount);
            }
        }
        public TechnologySO GetCurrentResearchInProgressTechnology() =>
            TechState.GetCurrentResearchTech();
        public bool IsTechnologyResearched(TechnologySO tech) =>
            TechState.IsResearched(tech);
        public bool IsTechResearchAvailable(TechnologySO tech)
        {
            return true;
        }
        public float GetCurrentReseachProgressPercent(TechnologySO tech) =>
            TechState.GetProgressPercent(tech);
        public List<TechnologySO> GetOfferedTechnologies() =>
            TechState.GetOfferedTechnologies();
        public void OnGlobalTick()
        {
            if (TechState.CurrentResearch == null)
                return;
            AddResearchPoints(10f);
        }
    }
}