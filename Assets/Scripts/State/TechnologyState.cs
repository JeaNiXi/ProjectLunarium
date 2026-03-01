using SO;
using System;
using System.Collections.Generic;
namespace State
{
    public class TechnologyState
    {
        public TechnologySO CurrentResearch;
        public Dictionary<TechnologySO, float> ResearchProgress;
        public HashSet<TechnologySO> ResearchedTechnologies;
        public List<TechnologySO> OfferedTechnologies;

        public event Action OnOfferedTechsRefresh;

        public TechnologyState(TechnologyManagerSO data)
        {
            ResearchProgress = new Dictionary<TechnologySO, float>();
            ResearchedTechnologies = new HashSet<TechnologySO>();
            OfferedTechnologies = new List<TechnologySO>();
        }
        public bool IsResearched(TechnologySO tech) =>
            tech != null && ResearchedTechnologies.Contains(tech);
        public bool IsOffererd(TechnologySO tech) =>
            tech != null && OfferedTechnologies.Contains(tech);
        public float GetProgressPercent(TechnologySO tech)
        {
            if (tech == null || tech.TotalResearchPoints == 0)
                return 0f;
            if (!ResearchProgress.TryGetValue(tech, out float value))
                return 0f;
            return value / tech.TotalResearchPoints;
        }
        public void ClearOffers() =>
            OfferedTechnologies.Clear();
        public void AddTechToResearched(TechnologySO tech) =>
            ResearchedTechnologies.Add(tech);
        public void AddTechToOffered(List<TechnologySO> techsList)
        {
            OfferedTechnologies = techsList;
            OnOfferedTechsRefresh?.Invoke();
        }
        public List<TechnologySO> GetOfferedTechnologies() =>
            OfferedTechnologies;
        public void RemoveTechFromOffered(TechnologySO tech)
        {
            if (OfferedTechnologies.Contains(tech))
                OfferedTechnologies.Remove(tech);
        }
        public void SetCurrentResearch(TechnologySO tech) =>
            CurrentResearch = tech;
        public TechnologySO GetCurrentResearchTech() =>
            CurrentResearch;
    }
}