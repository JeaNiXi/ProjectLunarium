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

        [Header("Localization Keys:")]
        [field: SerializeField] public string NameKey { get; private set; }
        [field: SerializeField] public string DescriptionKey {  get; private set; }

        [Header("RU Localization Data")]
        public string NameRU;
        [TextArea(2, 6)]
        public string DescriptionRU;

        [Header("EN Localization Data")]
        public string NameEN;
        [TextArea(2, 6)]
        public string DescriptionEN;

        public string LocalizationOutputFolder(LocalizationGeneratorSO config)
            => config.RacesLocalizationOutputFolder;
        public IEnumerable<LocalizationEntry> GetLocalizationEntriesRU()
        {
            yield return new(NameKey, NameRU);
            yield return new(DescriptionKey, DescriptionRU);
        }

        public IEnumerable<LocalizationEntry> GetLocalizationEntriesEN()
        {
            yield return new(NameKey, NameEN);
            yield return new(DescriptionKey, DescriptionEN);
        }
    }
}