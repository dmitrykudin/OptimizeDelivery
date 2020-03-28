using Common.Models.BusinessModels;

namespace Common.Models.ServiceModels
{
    public class DeliveryCluster
    {
        public int ClusterNumber { get; set; }

        public Parcel[] Parcels { get; set; }
    }
}