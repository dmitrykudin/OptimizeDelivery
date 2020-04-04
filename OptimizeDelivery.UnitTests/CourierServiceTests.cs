using System;
using System.Collections.Generic;
using Common.Abstractions.Services;
using Common.Helpers;
using Common.Models.BusinessModels;
using NUnit.Framework;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.UnitTests
{
    public class CourierServiceTests
    {
        public CourierServiceTests()
        {
            CourierService = new CourierService();
            DistrictService = new DistrictService();
        }

        private ICourierService CourierService { get; }

        private IDistrictService DistrictService { get; }

        [Test]
        [Repeat(5)]
        public void CreateCourierTest()
        {
            var timetableDays = new List<TimetableDay>(7);
            for (var i = 1; i <= 7; i++)
                if (i < 6)
                {
                    var workingDay = new TimetableDay
                    {
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        IsWeekend = false,
                        DayOfWeek = (DayOfWeek) i
                    };
                    timetableDays.Add(workingDay);
                }
                else
                {
                    var weekendDay = new TimetableDay
                    {
                        IsWeekend = true,
                        DayOfWeek = (DayOfWeek) i
                    };
                    timetableDays.Add(weekendDay);
                }

            var districts = DistrictService.GetAllDistricts();
            var rand = new Random();
            var (firstName, lastName) = RandHelper.GetRandomFirstAndLastName();

            var courier = new Courier
            {
                Name = firstName,
                Surname = lastName,
                WorkingDistrictId = districts[rand.Next(districts.Length)].Id,
                WorkingDays = timetableDays.ToArray()
            };

            var createdCourier = CourierService.CreateCourier(courier);

            Assert.AreEqual(courier.Name, createdCourier.Name);
            Assert.AreEqual(courier.Surname, createdCourier.Surname);
        }
    }
}