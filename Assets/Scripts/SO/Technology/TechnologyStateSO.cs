using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "TechnologyState", menuName = "Scriptable Objects/Technology/Technology State")]
    public class TechnologyStateSO : ScriptableObject
    {
        public List<TechnologySO> researchedTechnologies = new List<TechnologySO>();

        public void ClearResearchedTechList() => researchedTechnologies.Clear();
    }
}