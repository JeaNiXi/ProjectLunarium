using System.IO;
using UnityEngine;
namespace Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public string SavePath
            => Path.Combine(Application.persistentDataPath, "ProjectLunarium", "Saves");
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        public bool HasSave()
            => File.Exists(SavePath);
        public void SaveProfile(string profileID)
        {
            var profilePath = Path.Combine(SavePath, profileID);
            Directory.CreateDirectory(profilePath);
            var json = JsonUtility.ToJson(ResourceManager.Instance.SaveState(), true);
            File.WriteAllText(Path.Combine(profilePath, ResourceManager.Instance.SaveDataFileName), json);
        }
    }
}