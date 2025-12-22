using Localization;
using Managers;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resources/Resource")]
    public class ResourceSO : ScriptableObject, ILocalizable
    {
        [Header("ID")]
        public string ID;

        [Header("Localization Keys:")]
        [field: SerializeField] public string NameKey { get; private set; }
        [field: SerializeField] public string DescriptionKey { get; private set; }

        [Header("Visualisation")]
        public List<Sprite> AnimationSprites = new List<Sprite>();

        [Header("Unlock Conditions")]
        public List<TechnologySO> TechNeeded;

        [Header("RU Localization Data")]
        public string NameRU;
        [TextArea(2, 6)]
        public string DescriptionRU;

        [Header("EN Localization Data")]
        public string NameEN;
        [TextArea(2, 6)]
        public string DescriptionEN;
        public string LocalizationOutputFolder(LocalizationGeneratorSO config)
            => config.ResourcesLocalizationOutputFolder;
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