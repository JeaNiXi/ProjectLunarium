using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "TechnologyManager", menuName = "Scriptable Objects/Technology/Technology Manager")]
    public class TechnologyManagerSO : ScriptableObject
    {
        public List<TechnologySO> allTechnologies = new List<TechnologySO>();
        public TechnologySO startingTech;
        public List<TechnologySO> GetTechByTier(int tier) => allTechnologies.Where(t => t.Tier == tier).ToList();
        public int GetMaxTechTier() => allTechnologies.Max(t => t.Tier);
    }
}