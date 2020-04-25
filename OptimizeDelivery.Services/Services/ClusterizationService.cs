using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Common.Constants;
using Common.Helpers;
using Common.Models.BusinessModels;
using Common.Models.ServiceModels;
using NetTopologySuite.Algorithm.Locate;
using NetTopologySuite.Geometries;

namespace OptimizeDelivery.Services.Services
{
    public class ClusterizationService
    {
        public (Dictionary<District, List<Parcel>>, Parcel[]) ClusterParcelsByDistricts(IEnumerable<District> districts,
            IEnumerable<Parcel> parcels)
        {
            var availableLocation = new[] {Location.Interior, Location.Boundary};
            var locators = districts.Select(x => new
                {
                    District = x,
                    AreaLocator = new IndexedPointInAreaLocator(x.Area)
                })
                .ToArray();

            var clusterResult = districts
                .ToDictionary(
                    x => x,
                    x => new List<Parcel>());

            var nonClusteredParcels = new List<Parcel>(parcels.Count());

            foreach (var parcel in parcels)
            {
                foreach (var locator in locators)
                    if (availableLocation.Contains(
                        locator.AreaLocator.Locate(parcel.OriginalLocation.ToNtsCoordinate())))
                    {
                        clusterResult[locator.District].Add(parcel);
                        break;
                    }

                nonClusteredParcels.Add(parcel);
            }

            return (clusterResult, nonClusteredParcels.ToArray());
        }

        public DeliveryCluster[] ClusterParcelLocationsBalanced(IEnumerable<Parcel> parcels)
        {
            var parcelsArray = parcels.ToArray();
            var parcelLocations = parcelsArray
                .Select(x => new[]
                {
                    x.OriginalLocation.Latitude.Value,
                    x.OriginalLocation.Longitude.Value
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