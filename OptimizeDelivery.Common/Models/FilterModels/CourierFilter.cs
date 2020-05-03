using System;

namespace Common.Models.FilterModels
{
    public class CourierFilter
    {
        public int? WorkingDistrictId { get; set; }

        public DateTime? WorkingDay { get; set; }
    }
}