using Localization;
using System.Collections.Generic;

public enum LocalizationCategory
{
    UIMenu,
    UIWorkPlace,
    UIResources,
    UITechnology,
    UIWorkers,
    Races,
    WorkPlaceSO,
    WorkPlaceCategorySO,
    WorkPlaceTypeSO
}
public interface ILocalizable
{
    LocalizationCategory Category { get; }
    IEnumerable<LocalizationEntry> GetLocalizationEntries(string lang);
}
