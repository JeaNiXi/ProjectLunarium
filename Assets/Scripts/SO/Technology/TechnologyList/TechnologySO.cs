using Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [Serializable]
    public class TechnologySOLocalization
    {
        public LocalizedString Name;
        public LocalizedString Description;
    }
    [CreateAssetMenu(fileName = "Technology", menuName = "Scriptable Objects/Technology/Technology")]
    public class TechnologySO : ScriptableObject, ILocalizable
    {
        [Header("Main Info")]
        public string ID;
        public LocalizationCategory Category => LocalizationCategory.UITechnology;
        public TechnologySOLocalization Localization;
        public IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang)
        {
            yield return new LocalizationEntry(Localization.Name.Key, Localization.Name.Get(lang));
            yield return new LocalizationEntry(Localization.Description.Key, Localization.Description.Get(lang));
        }
        [Header("Technology Info")]
        public int Tier;
        public TechRarity Rarity;
        public Sprite Icon;

        [Header("Meta Data")]
        public int BaseWeight = 100;
        public bool IsHiddenByDefault;

        [Header("Research Data")]
        public float TotalResearchPoints;
        public List<TechMilestone> Milestones;

        [Header("Research Requirements")]
        public ResearchRequirements ResearchRequirements;

        [Header("Breakthrough Techs")]
        public List<BreakthroughChance> BreakthroughChances;

        [Header("Unlocks")]
        public List<ResourceSO> UnlockedResources;
    }
    [Serializable]
    public class ResearchRequirements
    {
        public List<TechnologySO> TechPrerequisites;
        public List<ResourceStack> ResourceOneTimeCost;
        public List<ResourceStack> ResourceDailyCost;
        [SerializeReference]
        public List<ResearchCondition> SpecialRequirements;
    }
    [Serializable]
    public struct BreakthroughChance
    {
        public TechnologySO Technology;
        [Range(0, 100)] public int Chance;
    }
    [Serializable]
    public struct TechMilestone
    {
        [Range(0.1f, 0.9f)] public float ProgressThreshhold;
        public string RewardDescriptionKey;
    }
    public enum TechRarity
    {
        Common,
        Rare
    }
}