using System;
using System.Collections.Generic;
namespace Data
{
    [Serializable]
    public class SaveData
    {
        public string SaveID;
        public string CreatedAt;
        public string LastPlayedAt;
        public string GameVersion;
    }
    [Serializable]
    public class SaveMetaData
    {
        public string KingdomName;
    }
    [Serializable]
    public class ResourceSaveData
    {
        public List<string> ResourceIDs;
        public List<int> Amounts;
    }
}