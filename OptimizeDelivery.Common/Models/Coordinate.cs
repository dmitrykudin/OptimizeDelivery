using System.Globalization;
using Common.Constants;

namespace Common.Models
{
    public class Coordinate
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString()
        {
            return Latitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture) + ", " +
                   Longitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture);
        }

        public string ToStringNoWhitespace()
        {
            return Latitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture) + "," +
                   Longitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture);
        }

        public string ToStringForMultipolygon()
        {
            return Longitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture) + " " +
                   Latitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture);
        }

        public string ToStringInverted()
        {
            return Longitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture) + ", " +
                   Latitude.ToString(Const.DefaultCoordinateOutputFormat, CultureInfo.InvariantCulture);
        }
    }
}