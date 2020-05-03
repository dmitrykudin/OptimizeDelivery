using System;
using Common.Abstractions.Services;
using Common.Helpers;
using Common.Models.BusinessModels;
using Itinero;
using NUnit.Framework;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.UnitTests
{
    public class ParcelServiceTests
    {
        public ParcelServiceTests()
        {
            ParcelService = new ParcelService();
            DistrictService = new DistrictService();
        }
        
        private IParcelService ParcelService { get; set; }
        
        private IDistrictService DistrictService { get; }

        [Test]
        [Repeat(100)]
        [TestCase(false, true)]
        public void CreateParcelTest(bool useDistricts, bool useIdealPoints)
        {
            var rand = new Random();
            var idealCoordinateService = new IdealCoordinateService();
            var depotService = new DepotService();
            var depot = depotService.GetDefaultDepot() ?? depotService.CreateDepot(new Depot
            {
                OriginalLocation = GeographyHelper.GetDbGeographyPoint(idealCoordinateService.GetIdealPoint()),
                WorkingTimeWindow = new WorkingWindow(6, 22),
            });

            int? randomDistrictId = null;
            RouterDb routerDb = null;
            if (useDistricts)
            {
                var districts = DistrictService.GetAllDistricts();
                randomDistrictId = districts[rand.Next(districts.Length)].Id;
            }

            if (!useIdealPoints)
            {
                routerDb = ItineroRouter.GetRouterDb(randomDistrictId);
            }

            var hourFrom = rand.Next(8, 22);
            var hourTo = hourFrom + 2;

            var parcel = new Parcel
            {
                DepotId = depot.Id,
                DistrictId = randomDistrictId,
                OriginalLocation = useIdealPoints 
                    ? GeographyHelper.GetDbGeographyPoint(idealCoordinateService.GetIdealPoint()) 
                    : GeographyHelper.GetDbGeographyPoint(RandHelper.CoordinateFrom(routerDb)),
                Weight = rand.Next(100),
                Volume = rand.Next(100),
                DeliveryTimeWindow = new TimeWindow(DateTime.Today, hourFrom, hourTo)
            };
            var createdParcel = ParcelService.CreateParcel(parcel);
            
            Assert.IsNotNull(createdParcel.RoutableLocation);
            Assert.IsNotNull(createdParcel.RoutableCoordinate);
        }
    }
}