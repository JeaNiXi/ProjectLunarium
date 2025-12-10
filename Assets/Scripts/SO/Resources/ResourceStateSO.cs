using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "ResourceState", menuName = "Scriptable Objects/Resources/Resource State")]
    public class ResourceStateSO : ScriptableObject
    {
        public List<float> resourcesAmountsList = new List<float>();
        public Dictionary<ResourceSO, float> resourceAmountsDictionary = new Dictionary<ResourceSO, float>();
        public void ClearAmountsList() => resourcesAmountsList.Clear();
        public void ClearAmountsDictionary() => resourceAmountsDictionary.Clear();
        public float GetResourceAmount(ResourceSO resource) => resourceAmountsDictionary.TryGetValue(resource, out var value) ? value : 0f;
    }
}