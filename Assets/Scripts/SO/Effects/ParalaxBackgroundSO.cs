using System.Collections.Generic;
using UnityEngine;
namespace Effect
{
    [CreateAssetMenu(fileName = "ParalaxBackground", menuName = "Scriptable Objects/Effects/Paralax Background")]
    public class ParalaxBackgroundSO : ScriptableObject
    {
        public string Name;
        public List<Sprite> BackgroundSprites;
    }
}