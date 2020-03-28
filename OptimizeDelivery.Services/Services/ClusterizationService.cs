using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Common;
using Common.Models.BusinessModels;
using Common.Models.ServiceModels;
using Const = Common.Constants.Const;

namespace OptimizeDelivery.Services.Services
{
    public class ClusterizationService
    {
        public DeliveryCluster[] ClusterParcelLocations(IEnumerable<Parcel> parcels)
        {
            var parcelsArray = parcels.ToArray();
            var parcelLocations = parcelsArray
                .Select(x => new[]
                {
                    x.Location.Latitude.Value,
                    x.Location.Longitude.Value
                })
                .ToArray();
            var numberOfClusters =
                Convert.ToInt32(Math.Ceiling((double) parcelLocations.Length / Const.DefaultParcelsPerDay));
            var kMeans = new KMeans(numberOfClusters);

            var clusters = kMeans.Learn(parcelLocations);
            var labels = clusters.Decide(parcelLocations);

            var deliveryLocations = labels
                .Select((t, i) => new DeliveryLocation
                {
                    ClusterNumber = t,
                    Parcel = parcelsArray[i]
                })
                .ToArray();

            var deliveryClusters = new List<DeliveryCluster>();

            for (var i = 0; i < numberOfClusters; i++)
                deliveryClusters.Add(new DeliveryCluster
                {
                    ClusterNumber = i,
                    Parcels = deliveryLocations
                        .Where(x => x.ClusterNumber == i)
                        .Select(x => x.Parcel)
                        .ToArray()
                });

            return deliveryClusters.ToArray();
        }

        public DeliveryCluster[] ClusterParcelLocationsBalanced(IEnumerable<Parcel> parcels)
        {
            var parcelsArray = parcels.ToArray();
            var parcelLocations = parcelsArray
                .Select(x => new[]
                {
                    x.Location.Latitude.Value,
                    x.Location.Longitude.Value
                })
                .ToArray();
            var numberOfClusters =
                Convert.ToInt32(Math.Ceiling((double) parcelLocations.Length / Const.DefaultParcelsPerDay));

            var balancedKMeans = new BalancedKMeans(numberOfClusters)
            {
                MaxIterations = 100
            };
            var clusters = balancedKMeans.Learn(parcelLocations);
            var labels = clusters.Decide(parcelLocations);

            var deliveryLocations = labels
                .Select((t, i) => new DeliveryLocation
                {
                    ClusterNumber = t,
                    Parcel = parcelsArray[i]
                })
                .ToArray();

            var deliveryClusters = new List<DeliveryCluster>();

            for (var i = 0; i < numberOfClusters; i++)
                deliveryClusters.Add(new DeliveryCluster
                {
                    ClusterNumber = i,
                    Parcels = deliveryLocations
                        .Where(x => x.ClusterNumber == i)
                        .Select(x => x.Parcel)
                        .ToArray()
                });

            return deliveryClusters.ToArray();
        }
    }
}