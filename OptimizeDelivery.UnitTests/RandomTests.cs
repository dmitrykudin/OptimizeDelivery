using System.Diagnostics;
using System.Globalization;
using Common;
using NUnit.Framework;

namespace OptimizeDelivery.UnitTests
{
    public class RandomTests
    {
        [Test]
        public void RandomLocationTest()
        {
            for (var i = 0; i < 500; i++)
            {
                var locationInSPb = Rand.LocationInSPb();
                Debug.WriteLine(string.Join(", ", locationInSPb.Longitude.Value.ToString(
                    Constants.DefaultCoordinateOutputFormat,
                    CultureInfo.InvariantCulture), locationInSPb.Latitude.Value.ToString(
                    Constants.DefaultCoordinateOutputFormat,
                    CultureInfo.InvariantCulture)));
            }
        }
    }
}