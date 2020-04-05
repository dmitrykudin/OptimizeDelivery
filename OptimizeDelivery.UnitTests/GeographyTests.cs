using System.Data.Entity.Spatial;
using Common.Constants;
using Common.Helpers;
using NetTopologySuite.Algorithm.Locate;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace OptimizeDelivery.UnitTests
{
    public class GeographyTests
    {
        [Test]
        [Repeat(100)]
        public void PointInPolygonTest()
        {
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(Const.SaintPetersburg.AsText());
            var locator = new IndexedPointInAreaLocator(geometry);

            var locationInSpb = RandHelper.LocationInSPb();
            var coordinate = new Coordinate(locationInSpb.Longitude.Value, locationInSpb.Latitude.Value);
            var location = locator.Locate(coordinate);

            Assert.Contains(location, new[] {Location.Interior, Location.Boundary});
        }

        [Test]
        public void IsPointInPolygonTest()
        {
            var polygon = DbGeography.FromText("POLYGON ((4 -3, 2 3, -2 4, -3 -1, 4 -3))");
            var testPoint = DbGeography.FromText("POINT (-2 2)");
            
            Assert.IsTrue(GeographyHelper.IsPointInPolygon(polygon, testPoint));
        }
    }
}