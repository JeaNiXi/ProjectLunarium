using Localization;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "WorkPlace", menuName = "Scriptable Objects/Work Place/Work Place")]
    public class WorkPlaceSO : ScriptableObject, ILocalizable
    {
        [Header("Main Info")]
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public List<WorkPlaceTypeSO> WorkPlaceTypes { get; private set; }
        [field: SerializeField] public ulong DefaultWorkPlaceCapacity { get; private set; }

        public LocalizationCategory Category => LocalizationCategory.WorkPlaceSO;
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

        [Header("RU Localization Data")]
        public string NameRU;

        [Header("EN Localization Data")]
        public string NameEN;


        //public string LocalizationOutputFolder(LocalizationGeneratorSO config)
        //    => config.UIMenuLocalizationOutputFolder;
    }
}