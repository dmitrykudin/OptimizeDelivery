using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using Common.Models;
using Microsoft.SqlServer.Types;

namespace Common.Helpers
{
    public static class GeographyHelper
    {
        public static string GetPointString(double latitude, double longitude)
        {
            return "POINT(" + longitude.ToString(CultureInfo.InvariantCulture) +
                   " " + latitude.ToString(CultureInfo.InvariantCulture) +
                   ")";
        }

        public static DbGeography GetPoint(double latitude, double longitude)
        {
            return DbGeography.PointFromText(GetPointString(latitude, longitude), Constants.Const.DefaultCoordinateSystemId);
        }

        public static string GetMultipolygonString(IEnumerable<Coordinate> coordinates)
        {
            var coordinatesString = string.Join(",", coordinates.Select(x => x.ToStringForMultipolygon()));
            return $"MULTIPOLYGON((({coordinatesString})))";
        }

        public static DbGeography CreatePolygon(string wellKnownText)
        {
            //First, get the area defined by the well-known text using left-hand rule
            var sqlGeography =
                SqlGeography.STGeomFromText(new SqlChars(wellKnownText), DbGeography.DefaultCoordinateSystemId)
                    .MakeValid();

            //Now get the inversion of the above area
            var invertedSqlGeography = sqlGeography.ReorientObject();

            //Whichever of these is smaller is the enclosed polygon, so we use that one.
            if (sqlGeography.STArea() > invertedSqlGeography.STArea()) sqlGeography = invertedSqlGeography;
            return DbSpatialServices.Default.GeographyFromProviderValue(sqlGeography);
        }

        public static string ToStringNoWhitespace(this DbGeography location)
        {
            return location == null
                ? null
                : location.Latitude.Value.ToString(Constants.Const.DefaultCoordinateOutputFormat,
                      CultureInfo.InvariantCulture) + "," +
                  location.Longitude.Value.ToString(Constants.Const.DefaultCoordinateOutputFormat,
                      CultureInfo.InvariantCulture);
        }
    }
}