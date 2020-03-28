using System;
using System.Linq;
using Common.Abstractions.Repositories;
using Common.DbModels;
using Common.Models.BusinessModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class TimetableRepository : ITimetableRepository
    {
        public DbTimetable CreateTimetable(Timetable timetable)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var timetableFromDb = context
                    .Set<DbTimetable>()
                    .Add(new DbTimetable
                    {
                        Name = timetable.Name,
                        TimetableDays = timetable.TimetableDays
                            .Select(x =>
                            {
                                var startTime = x.IsWeekend
                                    ? TimeSpan.Zero
                                    : x.StartTime;

                                var endTime = x.IsWeekend
                                    ? TimeSpan.Zero
                                    : x.EndTime;

                                return new DbTimetableDay
                                {
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    DayOfWeek = (int) x.DayOfWeek,
                                    IsWeekend = x.IsWeekend
                                };
                            })
                            .ToList()
                    });

                context.SaveChanges();

                return timetableFromDb;
            }
        }
    }
}