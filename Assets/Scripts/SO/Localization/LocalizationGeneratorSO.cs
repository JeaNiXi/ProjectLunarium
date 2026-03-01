using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "LocalizationGenerator", menuName = "Scriptable Objects/Localization/Localization Generator")]
    public class LocalizationGeneratorSO : ScriptableObject
    {
        [HideInInspector]
        public string DefaultPath = "Assets/Resources/Localization/Default";
        public string RacesLocalizationOutputFolder = "Assets/Resources/Localization/Population/Races";
        public string ResourcesLocalizationOutputFolder = "Assets/Resources/Localization/Resources";
        public string TechnologyLocalizationOutputFolder = "Assets/Resources/Localization/Technologies";
        public string WorkPlaceSOLocalizationOutputFolder = "Assets/Resources/Localization/WorkPlace";
        public string WorkPlaceCategorySOLocalizationOutputFolder = "Assets/Resources/Localization/WorkPlaceCategory";
        public string WorkPlaceTypeSOLocalizationOutputFolder = "Assets/Resources/Localization/WorkPlaceType";
        public string UIMenuLocalizationOutputFolder = "Assets/Resources/Localization/UI/Menu";
        public string UIWorkPlaceLocalizationOutputFolder = "Assets/Resources/Localization/UI/WorkPlace";
        public bool OverwriteFiles = true;
    }
}