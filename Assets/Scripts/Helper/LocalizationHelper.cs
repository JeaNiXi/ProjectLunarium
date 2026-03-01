using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class LocalizationHelper
{
    public static IEnumerable<LocalizationEntry> GetAllEntries(object obj, string lang)
    {
        if (obj == null)
            yield break;

        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var fields = obj.GetType().GetFields(flags);

        var keyFields = fields.Where(f => f.Name.EndsWith("Key", System.StringComparison.OrdinalIgnoreCase));

        foreach (var keyField in keyFields)
        {
            string key = keyField.GetValue(obj)?.ToString();
            if (string.IsNullOrEmpty(key))
                continue;

            string prefix = keyField.Name.Substring(0, keyField.Name.Length - 3);

            var valField = fields.FirstOrDefault(f => f.Name.Equals(prefix + lang, StringComparison.OrdinalIgnoreCase));

            if (valField != null)
                yield return new LocalizationEntry(key, valField.GetValue(obj)?.ToString());
        }
    }
}
