using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "Technology", menuName = "Scriptable Objects/Technology/Technology")]
    public class TechnologySO : ScriptableObject
    {
        public string ID;
        [Header("Main Info:")]
        public string NameKey;
        public string DescriptionKey;
        [Header("Technology Info:")]
        public int Tier;
        public List<TechnologySO> NextTech;
        public List<TechnologySO> PreviousTech;
        public List<ResourceSO> OpensResources;
        [Header("Research Requirements")]
        public ResearchRequirements ResearchRequirements;
    }
    [Serializable]
    public class ResearchRequirements
    {
        [Header("Technology")]
        public List<TechnologySO> TechnologiesRequiredToResearch;
        [Header("Resources")]
        public List<TechnologyResearchResourceCost> ResourceOneTimeCost;
        public List<TechnologyResearchResourceCost> ResourceDailyCost;
    }
    [Serializable]
    public class TechnologyResearchResourceCost
    {
        [field: SerializeField] public ResourceSO Resource { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        public TechnologyResearchResourceCost(ResourceSO resource, int cost)
        {
            Resource = resource;
            Amount = cost;
        }
    }
}