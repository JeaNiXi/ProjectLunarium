using Localization;
using SO;
using System.Collections.Generic;

public interface ILocalizable
{
    string LocalizationOutputFolder(LocalizationGeneratorSO config);
    IEnumerable<LocalizationEntry> GetLocalizationEntriesRU();
    IEnumerable<LocalizationEntry> GetLocalizationEntriesEN();
}
