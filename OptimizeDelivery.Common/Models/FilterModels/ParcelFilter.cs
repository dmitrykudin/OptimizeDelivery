using System;

namespace Common.Models.FilterModels
{
    public class ParcelFilter
    {
        public int? DistrictId { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}