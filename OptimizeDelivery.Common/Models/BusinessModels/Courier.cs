using System.Collections.Generic;

namespace Common.Models.BusinessModels
{
    public class Courier
    {
        public int Id { get; set; }

        public int TelegramId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public Timetable Timetable { get; set; }

        public IEnumerable<Route> Routes { get; set; }
    }
}