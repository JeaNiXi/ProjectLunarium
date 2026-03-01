using SO;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Localization
{
    public class LocalizationData
    {
        private Dictionary<string, string> UIMenuLocalization;
        private Dictionary<string, string> UIWorkPlaceLocalization;
        private Dictionary<string, string> ResourceLocalization;
        private Dictionary<string, string> TechnologySOLocalization;
        private Dictionary<string, string> WorkPlaceSOLocalization;
        private Dictionary<string, string> WorkPlaceCategorySOLocalization;
        private Dictionary<string, string> WorkPlaceTypeSOLocalization;
        private Dictionary<string, string> RaceLocalization;
        public enum Localizations
        {
            RU,
            EN
        }
        public Localizations CurrentLocalization { get; private set; }
        public LocalizationData(Localizations currentLocalization)
        {
            CurrentLocalization = currentLocalization;
            InitializeLocalizationFiles();
        }
        private void InitializeLocalizationFiles()
        {
            ResourceLocalization = new Dictionary<string, string>();
            TechnologySOLocalization = new Dictionary<string, string>();
            RaceLocalization = new Dictionary<string, string>();
            UIMenuLocalization = new Dictionary<string, string>();
            UIWorkPlaceLocalization = new Dictionary<string, string>();
            WorkPlaceSOLocalization = new Dictionary<string, string>();
            WorkPlaceCategorySOLocalization = new Dictionary<string, string>();
            WorkPlaceTypeSOLocalization = new Dictionary<string, string>();
            LoadTextAssets();
        }
        private void LoadTextAssets()
        {
            TextAsset UIMenuLocalizationAsset = Resources.Load<TextAsset>($"Localization/UI/Menu/{CurrentLocalization}");
            UIMenuLocalization = GetLocalizationDictionary(UIMenuLocalizationAsset);
            TextAsset UIWorkPlaceLocalizationAsset = Resources.Load<TextAsset>($"Localization/UI/WorkPlace/{CurrentLocalization}");
            UIWorkPlaceLocalization = GetLocalizationDictionary(UIWorkPlaceLocalizationAsset);
            TextAsset ResourceLocalizationAsset = Resources.Load<TextAsset>($"Localization/Resources/{CurrentLocalization}");
            ResourceLocalization = GetLocalizationDictionary(ResourceLocalizationAsset);
            TextAsset TechnologySOLocalizationAsset = Resources.Load<TextAsset>($"Localization/Technologies/{CurrentLocalization}");
            TechnologySOLocalization = GetLocalizationDictionary(TechnologySOLocalizationAsset);
            TextAsset WorkPlaceLocalizationAsset = Resources.Load<TextAsset>($"Localization/WorkPlace/{CurrentLocalization}");
            WorkPlaceSOLocalization = GetLocalizationDictionary(WorkPlaceLocalizationAsset);
            TextAsset WorkPlaceCategorySOLocalizationAsset = Resources.Load<TextAsset>($"Localization/WorkPlaceCategory/{CurrentLocalization}");
            WorkPlaceCategorySOLocalization = GetLocalizationDictionary(WorkPlaceCategorySOLocalizationAsset);
            TextAsset WorkPlaceTypeSOLocalizationAsset = Resources.Load<TextAsset>($"Localization/WorkPlaceType/{CurrentLocalization}");
            WorkPlaceTypeSOLocalization = GetLocalizationDictionary(WorkPlaceTypeSOLocalizationAsset);
            TextAsset RaceLocalizationAsset = Resources.Load<TextAsset>($"Localization/Population/Races/{CurrentLocalization}");
            RaceLocalization = GetLocalizationDictionary(RaceLocalizationAsset);
        }
        private Dictionary<string, string> GetLocalizationDictionary(TextAsset textAsset)
            => JsonUtility.FromJson<LocalizationWrapper>(textAsset.text).ToDictionary();
        public void SetLocalization(Localizations language)
            => CurrentLocalization = language;
        public string GetLocalizedResourceData(string resourceSODataKey)
            => ResourceLocalization.TryGetValue(resourceSODataKey, out string value) ? value : $"ERR. {resourceSODataKey}";
        public string GetLocalizedTechnologySOData(string technologySODataKey) =>
            TechnologySOLocalization.TryGetValue(technologySODataKey, out string value) ? value : $"ERR. {technologySODataKey}";
        public string GetLocalizedWorkPlaceSOData(string workPlaceSODataKey)
            => WorkPlaceSOLocalization.TryGetValue(workPlaceSODataKey, out string value) ? value : $"ERR. {workPlaceSODataKey}";
        public bool GetLocalizedWorkPlaceCategorySOData(string workPlaceCategorySODataKey, out string value) =>
            GetLocalizedData(WorkPlaceCategorySOLocalization, workPlaceCategorySODataKey, out value);
        public bool GetLocalizedWorkPlaceTypeSOData(string workPlaceSODataKey, out string value) =>
            GetLocalizedData(WorkPlaceTypeSOLocalization, workPlaceSODataKey, out value);
        public bool GetLocalizedRaceData(string raceDataKey, out string value)
            => GetLocalizedData(RaceLocalization, raceDataKey, out value);


        public string GetLocalizedUIMenuData(string menuDataKey)
            => UIMenuLocalization.TryGetValue(menuDataKey, out string value) && !string.IsNullOrEmpty(value)
                ? value
                : $"ERR. {menuDataKey}";
        public string GetLocalizedUIWorkPlaceData(string uiWorkPlaceDataKey) =>
            GetLocalizedData(UIWorkPlaceLocalization, uiWorkPlaceDataKey);

        public bool GetLocalizedData(Dictionary<string, string> localizationDictionary, string dataKey, out string value)
        {
            if (localizationDictionary.TryGetValue(dataKey, out value) && !string.IsNullOrEmpty(value))
                return true;
            else
            {
                value = $"[ERR. {dataKey}]";
                return false;
            }
        }
        public string GetLocalizedData(Dictionary<string, string> localizationDictionary, string dataKey)
        {
            if (localizationDictionary.TryGetValue(dataKey, out var value) && !string.IsNullOrEmpty(value))
                return value;
            else
                return $"[ERR. {dataKey}]";
        }
    }
    [Serializable]
    public class LocalizationWrapper
    {
        public List<LocalizationEntry> Entries;
        public Dictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>(Entries.Count);
            foreach (var entry in Entries)
            {
                dictionary[entry.Key] = entry.Value;
            }
            return dictionary;
        }
    }
    [Serializable]
    public class LocalizationEntry
    {
        public string Key;
        public string Value;
        public LocalizationEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}