using System;

namespace Common.Models.BusinessModels
{
    public class WorkingWindow
    {
        public WorkingWindow(int hourFrom, int hourTo)
        {
            TimeFrom = new TimeSpan(hourFrom, 0, 0);
            TimeTo = new TimeSpan(hourTo, 0, 0);
        }

        public WorkingWindow(TimeSpan timeFrom, TimeSpan timeTo)
        {
            TimeFrom = timeFrom;
            TimeTo = timeTo;
        }

        public TimeSpan TimeFrom { get; set; }

        public TimeSpan TimeTo { get; set; }

        public long[] GetWindow()
        {
            return new[] {Convert.ToInt64(TimeFrom.TotalSeconds), Convert.ToInt64(TimeTo.TotalSeconds)};
        }
    }
}