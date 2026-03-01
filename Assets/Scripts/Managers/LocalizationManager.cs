using Localization;
using UnityEngine;
using static Localization.LocalizationData;
namespace Managers
{
    public class LocalizationManager : MonoBehaviour
    {
        public Localizations CurrentLocalization;
        public static LocalizationManager Instance { get; private set; }
        private LocalizationData localizationData;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            localizationData = new LocalizationData(CurrentLocalization);
        }
        public void SetLocalization(LocalizationData.Localizations language)
            => localizationData.SetLocalization(language);
        public string GetLocalizedResourceData(string resourceDataKey)
            => localizationData.GetLocalizedResourceData(resourceDataKey);
        public string GetLocalizedTechnologySOData(string techDataKey) =>
            localizationData.GetLocalizedTechnologySOData(techDataKey);
        public string GetLocalizedWorkPlaceSOData(string workPlaceSODataKey)
            => localizationData.GetLocalizedWorkPlaceSOData(workPlaceSODataKey);
        public bool GetLocalizedWorkPlaceCategorySOData(string workPlaceCategorySODataKey, out string value) =>
            localizationData.GetLocalizedWorkPlaceCategorySOData(workPlaceCategorySODataKey, out value);
        public bool GetLocalizedWorkPlaceTypeSOData(string workPlaceTypeSODataKey, out string value) =>
            localizationData.GetLocalizedWorkPlaceTypeSOData(workPlaceTypeSODataKey, out value);
        public bool GetLocalizedRaceData(string raceDataKey, out string value)
        {
            if (localizationData.GetLocalizedRaceData(raceDataKey, out value))
                return true;
            else
                return false;
        }


        public string GetLocalizedUIMenuData(string uiMenuDataKey)
            => localizationData.GetLocalizedUIMenuData(uiMenuDataKey);
        public string GetLocalizedUIWorkPlaceData(string uiWorkPlaceData) =>
            localizationData.GetLocalizedUIWorkPlaceData(uiWorkPlaceData);
    }
}