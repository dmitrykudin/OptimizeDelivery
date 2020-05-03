using System.Diagnostics;
using System.Globalization;
using Common.Constants;
using Common.Helpers;
using NUnit.Framework;

namespace OptimizeDelivery.UnitTests
{
    public class RandomTests
    {
        [Test]
        [Ignore("Not used.")]
        public void RandomLocationTest()
        {
            for (var i = 0; i < 500; i++)
            {
                var locationInSPb = RandHelper.LocationInSPb();
                Debug.WriteLine(string.Join(", ", locationInSPb.Longitude.Value.ToString(
                    Const.DefaultCoordinateOutputFormat,
                    CultureInfo.InvariantCulture), locationInSPb.Latitude.Value.ToString(
                    Const.DefaultCoordinateOutputFormat,
                    CultureInfo.InvariantCulture)));
            }
        }
    }
}