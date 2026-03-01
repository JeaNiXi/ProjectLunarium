using UnityEngine;
namespace Localization
{
    [System.Serializable]
    public class LocalizedString
    {
        public string Key;
        [TextArea] public string RU;
        [TextArea] public string EN;

        public string Get(string lang)
        {
            return lang switch
            {
                "RU" => RU,
                "EN" => EN,
                _ => ""
            };
        }
    }
}