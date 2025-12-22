using Localization;
using SO;
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
        public string GetLocalizedResourceName(string resourceNameKey)
            => localizationData.GetLocalizedResourceName(resourceNameKey);
        public string GetLocalizedResourceDescription(string resourceDescriptionKey)
            => localizationData.GetLocalizedResourceDescription(resourceDescriptionKey);
    }
}