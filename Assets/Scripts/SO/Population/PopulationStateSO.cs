using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "PopulationState", menuName = "Scriptable Objects/Population/Population State")]
    public class PopulationStateSO : ScriptableObject
    {
        [field: SerializeField] public int CurrentPopulation { get; private set; }
        public void SetCurrentPopulation(int population) => CurrentPopulation = population;
    }
}