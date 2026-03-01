using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "WorkPlaceManagerSO", menuName = "Scriptable Objects/Work Place/Work Place Manager")]
    public class WorkPlaceManagerSO : ScriptableObject
    {
        [Header("Main Data")]
        public List<WorkPlaceCategorySO> WorkPlaceCategories = new List<WorkPlaceCategorySO>();
    }
}