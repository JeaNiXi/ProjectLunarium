using State;
using System;
using UnityEngine;
namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        private TimeState TimeState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            TimeState = new TimeState();
        }

        public string GetCurrentTimeString() => TimeState.GetCurrentTimeString();
        public void GetCurrentDay() => TimeState.GetCurrentDay();
        public void OnTickUpdate()
        {
            TimeState.UpdateTick();
            GameManager.Instance.OnGlobalTick(TimeState);
        }
    }
}