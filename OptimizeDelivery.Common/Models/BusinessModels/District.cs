using NetTopologySuite.Geometries;

namespace Common.Models.BusinessModels
{
    public class District
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Geometry Area { get; set; }
    }
}