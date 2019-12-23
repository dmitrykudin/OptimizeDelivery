using System;
using System.Data.Entity.Spatial;
using System.Threading;

namespace Common
{
    public static class Rand
    {
        private static readonly Random random = new Random(DateTime.Now.Second);

        public static DbGeography LocationInSPb()
        {
            var spb = Constants.SaintPetersburg;
            var pointCount = spb.PointCount;
            if (!pointCount.HasValue) return null;

            var inWater = true;
            DbGeography resultPoint = null;
            while (inWater)
            {
                var (firstPoint, secondPoint) = GetTwoRandomPointsFrom(spb, pointCount.Value);
                resultPoint = GetRandomPointBetween(firstPoint, secondPoint);
                inWater = resultPoint.Distance(Constants.SaintPetersburgBorderLine) <
                          resultPoint.Distance(Constants.SaintPetersburg);
            }

            return resultPoint;
        }

        private static (DbGeography, DbGeography) GetTwoRandomPointsFrom(DbGeography geography, int pointCount)
        {
            var pointsEquals = true;
            DbGeography firstPoint = null, secondPoint = null;
            while (pointsEquals)
            {
                firstPoint = geography.PointAt(random.Next(pointCount));
                Thread.Sleep(random.Next(50));
                secondPoint = geography.PointAt(random.Next(pointCount));
                pointsEquals = firstPoint.SpatialEquals(secondPoint);
            }

            return (firstPoint, secondPoint);
        }

        private static DbGeography GetRandomPointBetween(DbGeography firstPoint, DbGeography secondPoint)
        {
            var diameter = firstPoint.Distance(secondPoint);
            var randomShift = random.Next(Convert.ToInt32(Math.Truncate(diameter.Value)));

            var latitudeDiff = Math.Abs(firstPoint.Latitude.Value - secondPoint.Latitude.Value);
            var longitudeDiff = Math.Abs(firstPoint.Longitude.Value - secondPoint.Longitude.Value);

            var secondToTheTop = secondPoint.Latitude.Value >= firstPoint.Latitude.Value;
            var secondToTheRight = secondPoint.Longitude.Value >= firstPoint.Longitude.Value;

            var helperPoint = GeographyHelper.GetPoint(firstPoint.Latitude.Value, secondPoint.Longitude.Value);

            var a = firstPoint.Distance(helperPoint);
            var b = secondPoint.Distance(helperPoint);

            var sinA = Math.Sin(a.Value / diameter.Value);
            var sinB = Math.Sin(b.Value / diameter.Value);

            var latitudeDelta = sinB * randomShift;
            var longitudeDelta = sinA * randomShift;

            var latitudeDeltaInCoordinates = latitudeDelta / b.Value * latitudeDiff;
            var longitudeDeltaInCoordinates = longitudeDelta / a.Value * longitudeDiff;

            var latitude = secondToTheTop
                ? firstPoint.Latitude.Value + latitudeDeltaInCoordinates
                : firstPoint.Latitude.Value - latitudeDeltaInCoordinates;

            var longitude = secondToTheRight
                ? firstPoint.Longitude.Value + longitudeDeltaInCoordinates
                : firstPoint.Longitude.Value - longitudeDeltaInCoordinates;

            return GeographyHelper.GetPoint(latitude, longitude);
        }
    }
}