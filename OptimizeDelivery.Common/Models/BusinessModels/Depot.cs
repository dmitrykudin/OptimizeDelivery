using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace Common.Models.BusinessModels
{
    public class Depot
    {
        public int Id { get; set; }

        public DbGeography Location { get; set; }

        public IEnumerable<Parcel> Parcels { get; set; }
    }
}