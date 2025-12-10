using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "TechnologyManager", menuName = "Scriptable Objects/Technology/Technology Manager")]
    public class TechnologyManagerSO : ScriptableObject
    {
        public List<TechnologySO> allTechnologies = new List<TechnologySO>();
        public TechnologySO startingTech;
    }
}