using System;
using System.Collections.Generic;
using UnityEngine;
namespace Localization
{
    public class LocalizationData
    {
        private Dictionary<string, string> ResourceLocalization;
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
            RaceLocalization = new Dictionary<string, string>();
            LoadTextAssets();
        }
        private void LoadTextAssets()
        {
            TextAsset ResourceLocalizationAsset = Resources.Load<TextAsset>($"Localization/Resources/{CurrentLocalization}");
            ResourceLocalization = GetLocalizationDictionary(ResourceLocalizationAsset);
            TextAsset RaceLocalizationAsset = Resources.Load<TextAsset>($"Localization/Population/Races/{CurrentLocalization}");
            RaceLocalization = GetLocalizationDictionary(RaceLocalizationAsset);
        }
        private Dictionary<string, string> GetLocalizationDictionary(TextAsset textAsset)
            => JsonUtility.FromJson<LocalizationWrapper>(textAsset.text).ToDictionary();
        public void SetLocalization(Localizations language)
            => CurrentLocalization = language;
        public string GetLocalizedResourceName(string resourceNameKey)
            => ResourceLocalization.TryGetValue(resourceNameKey, out string value) ? value : "ERR. No Localization Name KEY";
        public string GetLocalizedResourceDescription(string resourceDescriptionKey)
            => ResourceLocalization.TryGetValue(resourceDescriptionKey, out string value) ? value : "ERR. No Localization Description Key";
        public string GetLocalizedRaceDescription(string raceDescriptionKey)
            => RaceLocalization.TryGetValue(raceDescriptionKey, out string value) ? value : "ERR. No Localization Description Key";
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
                Debug.Log($"Added To Locale - Key: {entry.Key}, Value: {entry.Value}.");
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