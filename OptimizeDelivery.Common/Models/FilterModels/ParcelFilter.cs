using System;
using Common.Models.BusinessModels;

namespace Common.Models.FilterModels
{
    public class ParcelFilter
    {
        public ParcelFilter()
        {
            RouteId = new FilterValue<int?>();
            DistrictId = new FilterValue<int?>();
            DeliveryDate = new FilterValue<DateTime?>();
        }
        
        public FilterValue<int?> RouteId { get; set; }

        public FilterValue<int?> DistrictId { get; set; }

        public FilterValue<DateTime?> DeliveryDate { get; set; }
    }
}