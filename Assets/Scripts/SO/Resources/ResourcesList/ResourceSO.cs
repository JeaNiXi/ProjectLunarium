using Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [Serializable]
    public class ResourceSOLocalization
    {
        public LocalizedString Name;
        public LocalizedString Description;
    }
    [CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resources/Resource")]
    public class ResourceSO : ScriptableObject, ILocalizable
    {
        [Header("Main Info")]
        public string ID;
        public LocalizationCategory Category => LocalizationCategory.UIResources;
        public ResourceSOLocalization Localization;
        public IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang)
        {
            yield return new LocalizationEntry(Localization.Name.Key, Localization.Name.Get(lang));
            yield return new LocalizationEntry(Localization.Description.Key, Localization.Description.Get(lang));
        }

        [Header("Visualisation")]
        public List<Sprite> AnimationSprites = new List<Sprite>();

        [Header("Unlock Conditions")]
        public List<TechnologySO> TechNeeded;
    }
}