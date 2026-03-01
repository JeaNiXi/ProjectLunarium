using Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [Serializable]
    public struct ResourceStack
    {
        public ResourceSO Resource;
        public ulong Amount;
    }

    [CreateAssetMenu(fileName = "WorkPlaceType", menuName = "Scriptable Objects/Work Place/Work Place Type")]
    public class WorkPlaceTypeSO : ScriptableObject, ILocalizable
    {
        [Header("Main Info")]
        [field: SerializeField] public string ID { get; private set; }
        public LocalizationCategory Category => LocalizationCategory.WorkPlaceTypeSO;
        public List<LocalizedString> LocalizedStrings;
        public IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang)
        {
            foreach (var s in LocalizedStrings)
            {
                if (!string.IsNullOrEmpty(s.Key))
                    yield return new LocalizationEntry(s.Key, s.Get(lang));
            }
        }
        [Header("Unlock Conditions")]

        [field: SerializeField] public List<TechnologySO> TechNeeded { get; private set; }

        [Header("Production Data")]
        public List<ResourceStack> ProducedResources;
        public List<ResourceStack> RequiredResources;

        [Header("Localization Keys:")]
        [field: SerializeField] public string NameKey { get; private set; }

        [Header("RU Localization Data")]
        public string NameRU;

        [Header("EN Localization Data")]
        public string NameEN;

        public IEnumerable<LocalizationEntry> GetLocalizationEntriesRU()
            => LocalizationHelper.GetAllEntries(this, "RU");
        public IEnumerable<LocalizationEntry> GetLocalizationEntriesEN()
            => LocalizationHelper.GetAllEntries(this, "EN");

        public string LocalizationOutputFolder(LocalizationGeneratorSO config)
            => config.WorkPlaceTypeSOLocalizationOutputFolder;
    }
}