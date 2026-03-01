using Localization;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "Work Place Category", menuName = "Scriptable Objects/Work Place/Work Place Category")]
    public class WorkPlaceCategorySO : ScriptableObject, ILocalizable
    {
        [Header("Main Info")]
        public string CategoryID;
        public List<WorkPlaceSO> WorkPlaces = new List<WorkPlaceSO>();
        public LocalizationCategory Category => LocalizationCategory.WorkPlaceCategorySO;
        public List<LocalizedString> LocalizedStrings;
        public IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang)
        {
            foreach (var s in LocalizedStrings)
            {
                if (!string.IsNullOrEmpty(s.Key))
                    yield return new LocalizationEntry(s.Key, s.Get(lang));
            }
        }

        [Header("Localization Keys")]
        [field: SerializeField] public string CategoryNameKey { get; private set; }

        [Header("RU Localization Data")]
        public string CategoryNameRU;

        [Header("EN Localization Data")]
        public string CategoryNameEN;
        public IEnumerable<LocalizationEntry> GetLocalizationEntriesRU()
            => LocalizationHelper.GetAllEntries(this, "RU");
        public IEnumerable<LocalizationEntry> GetLocalizationEntriesEN()
            => LocalizationHelper.GetAllEntries(this, "EN");
        //public string LocalizationOutputFolder(LocalizationGeneratorSO config) =>
        //    config.WorkPlaceCategorySOLocalizationOutputFolder;
    }
}