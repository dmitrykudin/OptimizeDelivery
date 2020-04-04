﻿using System;
using System.Linq;
using Common.DbModels;
using Common.Helpers;
using Common.Models.BusinessModels;

namespace Common.ConvertHelpers
{
    public static class ConvertHelperFromBusinessToDbModels
    {
        public static DbCourier ToDbCourier(this Courier courier)
        {
            return courier == null
                ? null
                : new DbCourier
                {
                    Name = courier.Name,
                    Surname = courier.Surname,
                    WorkingDistrictId = courier.WorkingDistrictId,
                    WorkingDays = courier.WorkingDays
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
                };
        }

        public static DbDistrict ToDbDistrict(this District district)
        {
            return district == null
                ? null
                : new DbDistrict
                {
                    Name = district.Name,
                    Area = GeographyHelper.WktToDbGeography(district.Area.ToText())
                };
        }
    }
}