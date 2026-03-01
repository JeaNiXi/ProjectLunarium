using State;
using System;
using UnityEngine;
namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        private TimeState TimeState;

        public event Action<int> OnDayChangedEvent;
        public event Action<int> OnMonthChangedEvent;
        public event Action<int> OnYearChangedEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitizalizeDataOnAwake();
        }
        private void InitizalizeDataOnAwake()
        {
            TimeState = new TimeState();
            InitializeConnections();
        }
        private void InitializeConnections()
        {
            TimeState.OnDayChanged += OnDayChanged;
            TimeState.OnMonthChanged += OnMonthChanged;
            TimeState.OnYearChanged += OnYearChanged;
        }
        private void OnDayChanged(int day) =>
            OnDayChangedEvent?.Invoke(day);
        private void OnMonthChanged(int month) =>
            OnMonthChangedEvent?.Invoke(month);
        private void OnYearChanged(int year) =>
            OnYearChangedEvent?.Invoke(year);

        public string GetCurrentTimeString() => TimeState.GetCurrentTimeString();
        public int GetCurrentDay() =>
            TimeState.GetCurrentDay();
        public int GetCurrentMonth() =>
            TimeState.GetCurrentMonth();
        public int GetCurrentYear() =>
            TimeState.GetCurrentYear();
        public void OnTickUpdate()
        {
            TimeState.UpdateTick();
            GameManager.Instance.OnGlobalTick(TimeState);
        }
    }
}