using Managers;
using System.IO;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SaveManager))]
public class SaveSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var saveManager = (SaveManager)target;
        if (saveManager == null)
            return;

        if (GUILayout.Button("Open Saves Location"))
        {
            var path = saveManager.SavePath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            EditorUtility.RevealInFinder(path);
        }
        if (GUILayout.Button("Create Temp Save"))
        {
            if (!Application.isEditor)
            {
                Debug.Log("EnterPlayMode To Create Save!");
                return;
            }
            saveManager.SaveProfile("testProfile");
        }
    }
}
