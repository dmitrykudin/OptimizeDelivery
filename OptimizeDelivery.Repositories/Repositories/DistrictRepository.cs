﻿using System.Linq;
using Common.Abstractions.Repositories;
using Common.ConvertHelpers;
using Common.DbModels;
using Common.Models.BusinessModels;

namespace OptimizeDelivery.DataAccessLayer.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        public int CreateDistrict(District district)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                var districtFromDb = context
                    .Set<DbDistrict>()
                    .Add(district.ToDbDistrict());

                context.SaveChanges();

                return districtFromDb.Id;
            }
        }

        public District GetDistrict(int districtId)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDistrict>()
                    .FirstOrDefault(x => x.Id == districtId)
                    .ToDistrict();
            }
        }

        public District GetDistrict(string districtName)
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDistrict>()
                    .FirstOrDefault(x => x.Name == districtName)
                    .ToDistrict();
            }
        }

        public District[] GetAllDistricts()
        {
            using (var context = new OptimizeDeliveryContext())
            {
                return context
                    .Set<DbDistrict>()
                    .ToArray()
                    .Select(x => x.ToDistrict())
                    .ToArray();
            }
        }
    }
}