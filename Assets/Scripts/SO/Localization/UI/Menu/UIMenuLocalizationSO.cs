using Localization;
using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "UIMenuLocalization", menuName = "Scriptable Objects/Localization/UI/Menu Localization")]
    public class UIMenuLocalizationSO : ScriptableObject, ILocalizable
    {
        [Header("Main Info")]
        public LocalizationCategory Category => LocalizationCategory.UIMenu;
        public List<LocalizedString> LocalizedStrings;

        public IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang)
        {
            foreach (var s in LocalizedStrings)
            {
                if (!string.IsNullOrEmpty(s.Key))
                    yield return new LocalizationEntry(s.Key, s.Get(lang));
            }
        }

        [Header("Localization Keys")]

        [field: SerializeField] public string CategoryMainMenuKey { get; private set; }
        [field: SerializeField] public string MainMenuNewGameKey { get; private set; }
        [field: SerializeField] public string CategoryPopulationKey { get; private set; }
        [field: SerializeField] public string CategoryWorkPlaceKey { get; private set; }
        [field: SerializeField] public string InfoPanelUpPopLabelKey { get; private set; }
        [field: SerializeField] public string InfoPanelUpPopActiveLabelKey { get; private set; }
        [field: SerializeField] public string InfoPanelUpPopInactiveLabelKey { get; private set; }
        [field: SerializeField] public string InfoCurrentDayKey { get; private set; }
        [field: SerializeField] public string InfoCurrentMonthKey { get; private set; }
        [field: SerializeField] public string InfoCurrentYearKey { get; private set; }

        [Header("RU Localization Data")]
        public string CategoryMainMenuRU;
        public string MainMenuNewGameRU;
        public string CategoryPopulationRU;
        public string CategoryWorkPlaceRU;
        public string InfoPanelUpPopulationLabelRU;
        public string InfoPanelUpPopActiveLabelRU;
        public string InfoPanelUpPopInactiveLabelRU;
        public string InfoCurrentDayRU;
        public string InfoCurrentMonthRU;
        public string InfoCurrentYearRU;

        [Header("EN Localization Data")]
        public string CategoryMainMenuEN;
        public string MainMenuNewGameEN;
        public string CategoryPopulationEN;
        public string CategoryWorkPlaceEN;
        public string InfoPanelUpPopulationLabelEN;
        public string InfoPanelUpPopActiveLabelEN;
        public string InfoPanelUpPopInactiveLabelEN;
        public string InfoCurrentDayEN;
        public string InfoCurrentMonthEN;
        public string InfoCurrentYearEN;

        //public string LocalizationOutputFolder(LocalizationGeneratorSO config)
        //    => config.UIMenuLocalizationOutputFolder;
        //public IEnumerable<LocalizationEntry> GetLocalizationEntriesRU() =>
        //    LocalizationHelper.GetAllEntries(this, "RU");
        //{
        //    yield return new(CategoryMainMenuKey, CategoryMainMenuRU);
        //    yield return new(MainMenuNewGameKey, MainMenuNewGameRU);
        //    yield return new(CategoryPopulationKey, CategoryPopulationRU);
        //    yield return new(CategoryWorkPlaceKey, CategoryWorkPlaceRU);
        //    yield return new(InfoPanelUpPopLabelKey, InfoPanelUpPopulationLabelRU);
        //    yield return new(InfoPanelUpPopActiveLabelKey, InfoPanelUpPopActiveLabelRU);
        //    yield return new(InfoPanelUpPopInactiveLabelKey, InfoPanelUpPopInactiveLabelRU);
        //    yield return new(InfoCurrentDayKey, InfoCurrentDayRU);
        //    yield return new(InfoCurrentMonthKey, InfoCurrentMonthRU);
        //    yield return new(InfoCurrentYearKey, InfoCurrentYearRU);
        //}
        //public IEnumerable<LocalizationEntry> GetLocalizationEntriesEN() =>
        //    LocalizationHelper.GetAllEntries(this, "EN");
        //{
        //    yield return new(CategoryMainMenuKey, CategoryMainMenuEN);
        //    yield return new(MainMenuNewGameKey, MainMenuNewGameEN);
        //    yield return new(CategoryPopulationKey, CategoryPopulationEN);
        //    yield return new(CategoryWorkPlaceKey, CategoryWorkPlaceEN);
        //    yield return new(InfoPanelUpPopLabelKey, InfoPanelUpPopulationLabelEN);
        //    yield return new(InfoPanelUpPopActiveLabelKey, InfoPanelUpPopActiveLabelEN);
        //    yield return new(InfoPanelUpPopInactiveLabelKey, InfoPanelUpPopInactiveLabelEN);
        //    yield return new(InfoCurrentDayKey, InfoCurrentDayEN);
        //    yield return new(InfoCurrentMonthKey, InfoCurrentMonthEN);
        //    yield return new(InfoCurrentYearKey, InfoCurrentYearEN);
        //}
    }
}