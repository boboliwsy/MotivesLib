namespace SimsMotivePrototype
{
    public class Clock
    {
        public int Minutes { get; private set; }
        public int Hours { get; private set; }

        public Clock()
        {
            Minutes = 0;
            Hours = 12;
        }

        public Clock(int startHour, int startMinutes)
        {
            Minutes = startMinutes;
            Hours = startHour;
        }

        public void AddMinutes(int minutes)
        {
            Minutes += minutes;
            if (Minutes > 58)
            {
                Minutes = 0;
                Hours++;
                if (Hours > 24) Hours = 1;
            }
        }

        public void AddHours(int hours)
        {
            //TODO: Allow negative?
            if (hours < 1) return;

            Hours += hours;
            if (Hours > 24)
                Hours -= 24;

        }
    }
}
