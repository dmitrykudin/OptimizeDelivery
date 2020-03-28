using Common.Models.BusinessModels;

namespace Common.Models.ServiceModels
{
    public class DeliveryLocation
    {
        public int ClusterNumber { get; set; }

        public Parcel Parcel { get; set; }
    }
}