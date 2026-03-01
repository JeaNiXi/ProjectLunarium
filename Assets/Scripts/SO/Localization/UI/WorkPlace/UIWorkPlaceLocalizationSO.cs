using Localization;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "UIWorkPlaceLocalization", menuName = "Scriptable Objects/Localization/UI/Work Place Localization")]
    public class UIWorkPlaceLocalizationSO : ScriptableObject, ILocalizable
    {
        [Header("Main Data")]
        public LocalizationCategory Category => LocalizationCategory.UIWorkPlace;
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
        [field: SerializeField] public string currentWorkersAmountKey { get; private set; }

        [Header("RU Localization Data")]
        public string currentWorkersAmountRU;

        [Header("EN Localization Data")]
        public string currentWorkersAmountEN;

        public string LocalizationOutputFolder(LocalizationGeneratorSO config) =>
            config.UIWorkPlaceLocalizationOutputFolder;

        public IEnumerable<LocalizationEntry> GetLocalizationEntriesRU() =>
            LocalizationHelper.GetAllEntries(this, "RU");

        public IEnumerable<LocalizationEntry> GetLocalizationEntriesEN() =>
            LocalizationHelper.GetAllEntries(this, "EN");

    }
}