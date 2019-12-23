using System.Diagnostics;
using System.Globalization;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OptimizeDelivery.Tests
{
    [TestClass]
    public class RandomTests
    {
        [TestMethod]
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