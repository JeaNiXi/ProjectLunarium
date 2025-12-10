using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "Technology", menuName = "Scriptable Objects/Technology/Technology")]
    public class TechnologySO : ScriptableObject
    {
        public string ID;
        [Header("Main Info:")]
        public string NameKey;
        public string DescriptionKey;
        [Header("Visualisation")]
        public List<ResourceSO> opensResources;
    }
}