using Localization;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "Race", menuName = "Scriptable Objects/Population/Races/Race")]
    public class RaceSO : ScriptableObject, ILocalizable
    {
        [Header("ID")]
        public string ID;

        public LocalizationCategory Category => LocalizationCategory.Races;
        public List<LocalizedString> LocalizedStrings;
        public IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang)
        {
            foreach (var s in LocalizedStrings)
            {
                if (!string.IsNullOrEmpty(s.Key))
                    yield return new LocalizationEntry(s.Key, s.Get(lang));
            }
        }

        [Header("Localization Keys:")]
        [field: SerializeField] public string NameKey { get; private set; }
        [field: SerializeField] public string DescriptionKey { get; private set; }

        [Header("RU Localization Data")]
        public string NameRU;
        [TextArea(2, 6)]
        public string DescriptionRU;

        [Header("EN Localization Data")]
        public string NameEN;
        [TextArea(2, 6)]
        public string DescriptionEN;

        //public string LocalizationOutputFolder(LocalizationGeneratorSO config)
        //    => config.RacesLocalizationOutputFolder;
        public IEnumerable<LocalizationEntry> GetLocalizationEntriesRU()
            => LocalizationHelper.GetAllEntries(this, "RU");

        public IEnumerable<LocalizationEntry> GetLocalizationEntriesEN()
            => LocalizationHelper.GetAllEntries(this, "EN");
    }
}