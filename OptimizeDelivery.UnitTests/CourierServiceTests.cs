using System;
using System.Collections.Generic;
using Common.Abstractions.Services;
using Common.Models.BusinessModels;
using NUnit.Framework;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.Tests
{
    public class CourierServiceTests
    {
        private ICourierService CourierService { get; set; }

        public CourierServiceTests()
        {
            CourierService = new CourierService();
        }
        
        [Test]
        public void CreateCourierTest()
        {
            var timetableDays = new List<TimetableDay>(7);
            for (var i = 1; i <= 7; i++)
            {
                if (i < 6)
                {
                    var workingDay = new TimetableDay
                    {
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        IsWeekend = false,
                        DayOfWeek = (DayOfWeek)i
                    };
                    timetableDays.Add(workingDay);
                }
                else
                {
                    var weekendDay = new TimetableDay
                    {
                        IsWeekend = true,
                        DayOfWeek = (DayOfWeek)i
                    };
                    timetableDays.Add(weekendDay);
                }
            }
            
            var courier = new Courier
            {
                Name = "Alex",
                Surname = "Goldberg",
                Timetable = new Timetable
                {
                    Name = "5/2",
                    TimetableDays = timetableDays.ToArray()
                }
            };

            var createdCourier = CourierService.CreateCourier(courier);
            
            Assert.AreEqual(courier.Name, createdCourier.Name);
            Assert.AreEqual(courier.Surname, createdCourier.Surname);
            Assert.AreEqual(courier.Timetable.Name, createdCourier.Timetable.Name);
        }
    }
}