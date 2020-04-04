using System.Collections.Generic;

namespace Common.Models.BusinessModels
{
    public class Courier
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int TelegramId { get; set; }

        public int WorkingDistrictId { get; set; }

        public District WorkingDistrict { get; set; }

        public IEnumerable<TimetableDay> WorkingDays { get; set; }

        public IEnumerable<Route> Routes { get; set; }
    }
}