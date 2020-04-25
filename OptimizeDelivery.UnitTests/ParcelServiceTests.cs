using System;
using Common.Abstractions.Services;
using Common.Helpers;
using Common.Models.BusinessModels;
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
        [Repeat(500)]
        public void CreateParcelTest()
        {
            var testDataService = new TestDataService();
            var depot = testDataService.GetLastDepot();
            var rand = new Random();

            var districts = DistrictService.GetAllDistricts();
            var randomDistrictId = districts[rand.Next(districts.Length)].Id;
            var routerDb = RouterService.GetRouterDb(randomDistrictId);
            var hourFrom = rand.Next(8, 22);
            var hourTo = hourFrom + 2;

            var parcel = new Parcel
            {
                DepotId = depot.Id,
                DistrictId = randomDistrictId,
                OriginalLocation = RandHelper.GeographyFrom(routerDb),
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