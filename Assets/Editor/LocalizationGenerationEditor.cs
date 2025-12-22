using Localization;
using SO;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(LocalizationGeneratorSO))]
public class LocalizationGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Resource Localization JSON"))
            GenerateLocalizationForType<ResourceSO>();
        if (GUILayout.Button("Generate Technology Localization JSON"))
            Debug.Log("AH, this is not realised");
    }
    private void GenerateLocalizationForType<T>() where T : ScriptableObject, ILocalizable
    {
        var config = (LocalizationGeneratorSO)target;
        var tmpPath = GetOutputPath<T>(config);
        var ru = new List<LocalizationEntry>();
        var en = new List<LocalizationEntry>();

        var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            ru.AddRange(asset.GetLocalizationEntriesRU());
            en.AddRange(asset.GetLocalizationEntriesEN());
        }
        WriteJSON<T>("RU", ru, tmpPath, config);
        WriteJSON<T>("EN", en, tmpPath, config);
        AssetDatabase.Refresh();
        Debug.Log($"Generated JSON for ALL {typeof(T).Name}.");
    }
    private string GetOutputPath<T>(LocalizationGeneratorSO config) where T : ScriptableObject, ILocalizable
    {
        if (typeof(T) == typeof(ResourceSO))
            return config.ResourcesLocalizationOutputFolder;
        if (typeof(T) == typeof(TechnologySO))
            return config.TechnologyLocalizationOutputFolder;
        return config.DefaultPath;
    }
    private void WriteJSON<T>(string language, List<LocalizationEntry> entries, string outputPath, LocalizationGeneratorSO config) where T : ScriptableObject, ILocalizable
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
