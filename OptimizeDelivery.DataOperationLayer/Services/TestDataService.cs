using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.ConvertHelpers;
using Common.DbModels;
using Common.Models.DbMappedModels;
using OptimizeDelivery.DataAccessLayer.GenericRepository;
using RandomNameGen;

namespace OptimizeDelivery.DataAccessLayer.Services
{
    public class TestDataService
    {
        public void SimulateParcelsForToday()
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var depot = context.Set<DbDepot>().FirstOrDefault();
                
                var rand = new Random(DateTime.Now.Second);
                for (var i = 0; i < 100; i++)
                {
                    var tenOClock = DateTime.Now.Date.AddHours(10);
                    var dateTimeFrom = tenOClock.AddHours(2 * rand.Next(0, 5));
                    var dateTimeTo = dateTimeFrom.AddHours(2);

                    context.Set<DbParcel>().Add(new DbParcel
                    {
                        DepotId = depot.Id,
                        Location = Rand.LocationInSPb(),
                        DeliveryDateTimeFromUtc = dateTimeFrom,
                        DeliveryDateTimeToUtc = dateTimeTo,
                    });
                }

                context.SaveChanges();
            }
        }
        
        public void CreateTestData()
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var depot = context.Set<DbDepot>().Add(new DbDepot
                {
                    Location = Constants.DefaultDepotCoordinate
                });

                var rand = new Random(DateTime.Now.Second);
                // var randomName = new RandomName(rand);
                for (var i = 0; i < 10; i++)
                {
                    // var name = randomName.Generate(Sex.Male).Split(' ');
                    context.Set<DbCourier>().Add(new DbCourier
                    {
                        Name = "Courier " + i,
                        Surname = i.ToString(),
                    });
                }

                context.SaveChanges();

                for (var i = 0; i < 100; i++)
                {
                    var dateTimeFromOffset = rand.Next(24);
                    var dateTimeToOffset = dateTimeFromOffset + 2;
                    context.Set<DbParcel>().Add(new DbParcel
                    {
                        DepotId = depot.Id,
                        Location = Rand.LocationInSPb(),
                        DeliveryDateTimeFromUtc = DateTime.Now.AddHours(dateTimeFromOffset),
                        DeliveryDateTimeToUtc = DateTime.Now.AddHours(dateTimeToOffset)
                    });
                }

                context.SaveChanges();
            }
        }

        public Depot GetLastDepot()
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDepot>()
                    .FirstOrDefault(x => x.Location.SpatialEquals(Constants.DefaultDepotCoordinate))
                    .ToDepot();
            }
        }
    }
}