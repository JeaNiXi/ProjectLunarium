using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "ResourceState", menuName = "Scriptable Objects/Resources/Resource State")]
    public class ResourceStateSO : ScriptableObject
    {
        public List<float> resourcesAmounts = new List<float> ();
    }
}