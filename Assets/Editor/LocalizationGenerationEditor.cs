using Localization;
using SO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(LocalizationGeneratorSO))]
public class LocalizationGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Localization", GUILayout.Height(40f)))
            GenerateAll();
    }
    private void GenerateAll()
    {
        var config = (LocalizationGeneratorSO)target;
        var localizableTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ILocalizable).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

        foreach (var type in localizableTypes)
        {
            GenerateForType(type, config);
        }
        AssetDatabase.Refresh();
        Debug.Log("<color=green><b>[Localization]</b> All Localizations Generated Succsessfully! </color>");
    }
    private void GenerateForType(System.Type type, LocalizationGeneratorSO config)
    {
        var guids = AssetDatabase.FindAssets($"t:{type.Name}");
        if (guids.Length == 0)
            return;

        var first = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
            AssetDatabase.GUIDToAssetPath(guids[0])) as ILocalizable;
        var path = GetPathByCategory(config, first.Category);

        var ru = new List<LocalizationEntry>();
        var en = new List<LocalizationEntry>();

        foreach (var guid in guids)
        {
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                AssetDatabase.GUIDToAssetPath(guid)) as ILocalizable;

            ru.AddRange(asset.GetLocalizationEntries("RU"));
            en.AddRange(asset.GetLocalizationEntries("EN"));
        }
        WriteJSON("RU", ru, path, config);
        WriteJSON("EN", en, path, config);
    }
    private string GetPathByCategory(LocalizationGeneratorSO config, LocalizationCategory category)
    {
        return category switch
        {
            LocalizationCategory.UIMenu => config.UIMenuLocalizationOutputFolder,
            LocalizationCategory.UIWorkPlace => config.UIWorkPlaceLocalizationOutputFolder,
            LocalizationCategory.UIResources => config.ResourcesLocalizationOutputFolder,
            LocalizationCategory.UITechnology => config.TechnologyLocalizationOutputFolder,
            LocalizationCategory.Races => config.RacesLocalizationOutputFolder,
            LocalizationCategory.WorkPlaceSO => config.WorkPlaceSOLocalizationOutputFolder,
            LocalizationCategory.WorkPlaceCategorySO => config.WorkPlaceCategorySOLocalizationOutputFolder,
            LocalizationCategory.WorkPlaceTypeSO => config.WorkPlaceTypeSOLocalizationOutputFolder,
            _ => config.DefaultPath
        };
    }
    private void WriteJSON(string language, List<LocalizationEntry> entries, string outputPath, LocalizationGeneratorSO config)
    {
        var wrapper = new LocalizationWrapper { Entries = entries };
        var json = JsonUtility.ToJson(wrapper, true);
        Directory.CreateDirectory(outputPath);
        var filePath = Path.Combine(outputPath, $"{language}.json");
        if (File.Exists(filePath) && !config.OverwriteFiles)
        {
            Debug.Log($"File {filePath} already exists!");
            return;
        }
        File.WriteAllText(filePath, json);
        Debug.Log($"Created JSON for: {filePath}.");
    }
}
