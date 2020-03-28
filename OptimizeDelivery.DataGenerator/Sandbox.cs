using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using Common;
using Common.Helpers;
using Common.Models;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.DataGenerator
{
    public static class Sandbox
    {
        public static void CreateTestData()
        {
            var service = new TestDataService();
            service.SimulateParcelsForToday();
        }

        private static void NormalizePolygon(DbGeography polygon)
        {
            var distances = new List<double?>();
            for (var i = 1; i < polygon.PointCount; i++)
            {
                var currentPoint = polygon.PointAt(i);
                var nextPoint = polygon.PointAt(i + 1);

                distances.Add(currentPoint.Distance(nextPoint));
            }

            var averageDistance = distances.Average();
            var coordinateList = new List<Coordinate>();
            for (var i = 1; i < polygon.PointCount; i++)
            {
                var currentPoint = polygon.PointAt(i);
                coordinateList.Add(new Coordinate
                {
                    Latitude = currentPoint.Latitude.Value,
                    Longitude = currentPoint.Longitude.Value
                });
                var nextPoint = polygon.PointAt(i + 1);
                CreateIntermediatePoints(currentPoint, nextPoint, averageDistance.Value, coordinateList);
            }

            var firstPoint = polygon.PointAt(1);
            var lastPoint = polygon.PointAt(polygon.PointCount.Value);

            CreateIntermediatePoints(lastPoint, firstPoint, averageDistance.Value, coordinateList);

            coordinateList.Add(new Coordinate
            {
                Latitude = firstPoint.Latitude.Value,
                Longitude = firstPoint.Longitude.Value
            });

            var filePath = @"c:\newSpbMultiPolygon2.txt";

            File.WriteAllText(filePath, GeographyHelper.GetMultipolygonString(coordinateList));

            var newSpbPolygon = DbGeography.FromText(GeographyHelper.GetMultipolygonString(coordinateList));
        }

        private static void CreateIntermediatePoints(DbGeography firstPoint, DbGeography secondPoint,
            double averageDistance,
            List<Coordinate> coordinateList)
        {
            var diameter = firstPoint.Distance(secondPoint);
            var latitudeDiff = Math.Abs(firstPoint.Latitude.Value - secondPoint.Latitude.Value);
            var longitudeDiff = Math.Abs(firstPoint.Longitude.Value - secondPoint.Longitude.Value);

            var secondToTheTop = secondPoint.Latitude.Value >= firstPoint.Latitude.Value;
            var secondToTheRight = secondPoint.Longitude.Value >= firstPoint.Longitude.Value;

            if (diameter > averageDistance)
            {
                var helperPoint = GeographyHelper.GetPoint(firstPoint.Latitude.Value, secondPoint.Longitude.Value);

                var a = firstPoint.Distance(helperPoint);
                var b = secondPoint.Distance(helperPoint);

                var sinA = Math.Sin(a.Value / diameter.Value);
                var sinB = Math.Sin(b.Value / diameter.Value);

                var numberOfIntermediatePoints = Convert.ToInt32(Math.Truncate(diameter.Value / averageDistance));
                for (var j = 1; j <= numberOfIntermediatePoints; j++)
                {
                    var currentDiameter = averageDistance * j;

                    var latitudeDelta = sinB * currentDiameter;
                    var longitudeDelta = sinA * currentDiameter;

                    var latitudeDeltaInCoordinates = latitudeDelta / b.Value * latitudeDiff;
                    var longitudeDeltaInCoordinates = longitudeDelta / a.Value * longitudeDiff;

                    var latitude = secondToTheTop
                        ? firstPoint.Latitude.Value + latitudeDeltaInCoordinates
                        : firstPoint.Latitude.Value - latitudeDeltaInCoordinates;

                    var longitude = secondToTheRight
                        ? firstPoint.Longitude.Value + longitudeDeltaInCoordinates
                        : firstPoint.Longitude.Value - longitudeDeltaInCoordinates;

                    if (double.IsNaN(latitude)) latitude = firstPoint.Latitude.Value;

                    if (double.IsNaN(longitude)) longitude = firstPoint.Longitude.Value;
                    coordinateList.Add(new Coordinate
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    });
                }
            }
        }
    }
}