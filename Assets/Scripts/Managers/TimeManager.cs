using State;
using System;
using UnityEngine;
namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        private TimeState timeState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            timeState = new TimeState();
        }

        public string GetCurrentTimeString() => timeState.GetCurrentTimeString();
        public void GetCurrentDay() => timeState.GetCurrentDay();
        public void OnTickUpdate() => timeState.UpdateTick();
    }
}