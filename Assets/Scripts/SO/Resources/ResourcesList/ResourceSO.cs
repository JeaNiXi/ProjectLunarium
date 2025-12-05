using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resources/Resource")]
    public class ResourceSO : ScriptableObject
    {
        public string ID;
        [Header("Main Info:")]
        public string NameKey;
        public string DescriptionKey;
        [Header("Visualisation")]
        public List<Sprite> AnimationSprites = new List<Sprite>();

    }
}