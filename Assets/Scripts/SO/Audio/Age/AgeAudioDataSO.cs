using System.Collections.Generic;
using UnityEngine;
namespace SO
{
    [CreateAssetMenu(fileName = "AgeAudioData", menuName = "Scriptable Objects/Audio/BG/Age Audio Data")]
    public class AgeAudioDataSO : ScriptableObject
    {
        [Header("BG Age Audio Clips")]
        public List<AudioClip> AncientAgeBGAudio;
    }
}