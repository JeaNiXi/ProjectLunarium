using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "LocalizationGenerator", menuName = "Scriptable Objects/Localization/Localization Generator")]
    public class LocalizationGeneratorSO : ScriptableObject
    {
        [HideInInspector]
        public string DefaultPath = "Assets/Resources/Localization/Default";
        public string ResourcesLocalizationOutputFolder = "Assets/Resources/Localization/Resources";
        public string TechnologyLocalizationOutputFolder = "Assets/Resources/Localization/Technologies";

        public bool OverwriteFiles = true;
    }
}