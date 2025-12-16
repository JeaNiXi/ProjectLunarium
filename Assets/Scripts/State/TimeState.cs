namespace State
{
    public class TimeState
    {
        private int currentDay;
        private int currentMonth;
        private int currentYear;

        public TimeState()
        {
            currentDay = 0;
            currentMonth = 1;
            currentYear = 1;
        }
        public string GetCurrentTimeString() => $"Current Day: {currentDay}, Current Month: {currentMonth}, Current Year: {currentYear}.";
        public int GetCurrentDay() => currentDay;
        public int GetCurrentMonth() => currentMonth;
        public int GetCurrentYear() => currentYear;
        public void UpdateTick()
        {
            AddDay();
        }
        private void AddDay()
        {
            currentDay++;
            if (currentDay == 31)
            {
                currentDay = 1;
                currentMonth++;
                if (currentMonth == 13)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }
        }
    }
}
