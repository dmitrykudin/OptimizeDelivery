using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using Common.Constants;
using Common.Models;
using Itinero;
using Itinero.Osm.Vehicles;
using Microsoft.SqlServer.Types;
using ItineroCoordinate = Itinero.LocalGeo.Coordinate;
using NTSCoordinate = NetTopologySuite.Geometries.Coordinate;

namespace Common.Helpers
{
    public static class GeographyHelper
    {
        private static RouterDb DbRouter { get; set; }
        
        public static string GetPointString(double latitude, double longitude)
        {
            return "POINT(" + longitude.ToString(CultureInfo.InvariantCulture) +
                   " " + latitude.ToString(CultureInfo.InvariantCulture) +
                   ")";
        }

        public static DbGeography GetPoint(double latitude, double longitude)
        {
            return DbGeography.PointFromText(GetPointString(latitude, longitude), Const.DefaultCoordinateSystemId);
        }

        public static string GetMultipolygonString(IEnumerable<LocalCoordinate> coordinates)
        {
            var coordinatesString = string.Join(",", coordinates.Select(x => x.ToStringForMultipolygon()));
            return $"MULTIPOLYGON((({coordinatesString})))";
        }

        public static DbGeography WktToDbGeography(string wellKnownText)
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
                : location.Latitude.Value.ToString(Const.DefaultCoordinateOutputFormat,
                      CultureInfo.InvariantCulture) + "," +
                  location.Longitude.Value.ToString(Const.DefaultCoordinateOutputFormat,
                      CultureInfo.InvariantCulture);
        }

        // Latitude = Y
        // Longitude = X
        public static bool IsPointInPolygon(DbGeography polygon, DbGeography testPoint)
        {
            if (polygon?.PointCount == null)
            {
                return false;
            }

            var result = false;
            var j = polygon.PointCount.Value;
            for (var i = 1; i <= polygon.PointCount; i++)
            {
                var prevPoint = polygon.PointAt(j);
                var currPoint = polygon.PointAt(i);
                
                if (currPoint.Latitude < testPoint.Latitude && prevPoint.Latitude >= testPoint.Latitude
                    || prevPoint.Latitude < testPoint.Latitude && currPoint.Latitude >= testPoint.Latitude)
                {
                    if (currPoint.Longitude + (testPoint.Latitude - currPoint.Latitude)
                        / (prevPoint.Latitude - currPoint.Latitude) 
                        * (prevPoint.Longitude - currPoint.Longitude) < testPoint.Longitude)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static NTSCoordinate ToNtsCoordinate(this DbGeography geography)
        {
            return geography?.PointCount == null
                   || geography.PointCount.Value == 0
                   || !geography.Longitude.HasValue
                   || !geography.Latitude.HasValue
                ? null
                : new NTSCoordinate(geography.Longitude.Value, geography.Latitude.Value);
        }

        public static ItineroCoordinate ToItineroCoordinate(this DbGeography geography)
        {
            if (geography?.PointCount == null
                || geography.PointCount.Value == 0
                || !geography.Longitude.HasValue
                || !geography.Latitude.HasValue)
            {
                throw new ArgumentException("Bad DbGeography object.");
            }

            return new ItineroCoordinate(Convert.ToSingle(geography.Latitude.Value),
                Convert.ToSingle(geography.Longitude.Value));
        }

        public static Router GetCentralDistrictRouter()
        {
            RouterDb routerDb;
            using (var stream = new FileInfo(@"D:/Maps.pbf/RouterDb/spb-central-district.routerdb").OpenRead())
            {
                routerDb = RouterDb.Deserialize(stream);
            }

            routerDb.AddContracted(Vehicle.Car.Fastest());

            return new Router(routerDb);
        }

        public static DbGeography GetDbGeographyFromFile(string filePath)
        {
            return WktToDbGeography(File.ReadAllText(filePath));
        }
    }
}