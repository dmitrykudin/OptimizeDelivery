using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Common.DbModels
{
    [Table("District")]
    public class DbDistrict
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DbGeography Area { get; set; }
    }
}