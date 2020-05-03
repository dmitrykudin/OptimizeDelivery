using System;

namespace Common.Models.BusinessModels
{
    public class TimeWindow
    {
        public TimeWindow(DateTime from, DateTime to)
        {
            Date = from.Date;
            TimeFrom = new TimeSpan(from.Hour, from.Minute, from.Second);
            TimeTo = new TimeSpan(to.Hour, to.Minute, to.Second);
        }

        public TimeWindow(DateTime date, int hourFrom, int hourTo)
        {
            Date = date;
            TimeFrom = new TimeSpan(hourFrom, 0, 0);
            TimeTo = new TimeSpan(hourTo, 0, 0);
        }

        public TimeWindow(int year, int month, int day, int hourFrom, int hourTo)
            : this(new DateTime(year, month, day), hourFrom, hourTo)
        {
        }
        
        public DateTime Date { get; set; }

        public TimeSpan TimeFrom { get; set; }

        public TimeSpan TimeTo { get; set; }

        public DateTime DateTimeFrom => Date.Add(TimeFrom);
        
        public DateTime DateTimeTo => Date.Add(TimeTo);

        public long[] GetWindow()
        {
            return new[] {Convert.ToInt64(TimeFrom.TotalSeconds), Convert.ToInt64(TimeTo.TotalSeconds)};
        }
    }
}