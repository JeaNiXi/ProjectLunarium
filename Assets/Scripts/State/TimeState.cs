using UnityEngine;
namespace State
{
    public class TimeState
    {
        public int CurrentDay;
        public int CurrentMonth;
        public int CurrentYear;

        public TimeState()
        {
            CurrentDay = 1;
            CurrentMonth = 1;
            CurrentYear = 1;
        }
        public string GetCurrentTimeString() => $"Current Day: {CurrentDay}, Current Month: {CurrentMonth}, Current Year: {CurrentYear}.";
        public int GetCurrentDay() => CurrentDay;
        public int GetCurrentMonth() => CurrentMonth;
        public int GetCurrentYear() => CurrentYear;
        public void UpdateTick()
        {
            AddDay();
        }
        private void AddDay()
        {
            CurrentDay++;
            if (CurrentDay == 31)
            {
                CurrentDay = 1;
                CurrentMonth++;
                if (CurrentMonth == 13)
                {
                    CurrentMonth = 1;
                    CurrentYear++;
                }
            }
        }
    }
}
