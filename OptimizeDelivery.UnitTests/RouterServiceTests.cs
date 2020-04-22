using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using Common.Helpers;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using NUnit.Framework;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.UnitTests
{
    public class RouterServiceTests
    {
        [Test]
        [TestCase(@"D:\Maps.pbf\Pbf\spb-central-district.osm.pbf", @"D:\Maps.pbf\RouterDb\spb-central-district.routerdb")]
        public void CreateRouterDbFileTest(string filePath, string savePath)
        {
            RouterService.CreateRouterDbFile(filePath, savePath);
        }
        
        [Test]
        public void GetTimeMatrixTest()
        {
            var router = RouterService.GetRouter();
            
            /*
            var points = new DbGeography[10]; 
            for (int i = 0; i < 10; i++)
            {
                points[i] = RandHelper.LocationInCentralDistrict();
                Console.WriteLine(points[i].ToStringNoWhitespace());
            }

            var coordinates = points.Select(x => x.ToItineroCoordinate()).ToArray();
            */

            var coordinates = new[]
            {
                new Coordinate(59.9202541272f, 30.3386695619f),
                new Coordinate(59.9190939589f, 30.3402878697f),
                new Coordinate(59.9183610857f, 30.3409329032f),
                new Coordinate(59.9295678254f, 30.3201181123f),
                new Coordinate(59.9334973694f, 30.3139640327f),
                new Coordinate(59.9380210072f, 30.3126046687f),
            };

            var result = RouterService.GetWeightTimeMatrix(coordinates
                .Select(x => router.Resolve(Vehicle.Car.Fastest(), x, 200F))
                .ToArray());
            int rowLength = result.GetLength(0);
            int colLength = result.GetLength(1);

            Console.WriteLine("\nUsing GetWeightTimeMatrix");
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write($"{result[i,j], 12} ");
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            
            var result1 = RouterService.GetTimeMatrix(coordinates
                .Select(x => router.Resolve(Vehicle.Car.Fastest(), x, 200F))
                .ToArray());
            rowLength = result1.GetLength(0);
            colLength = result1[0].GetLength(0);

            Console.WriteLine("\nUsing GetTimeMatrix");
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write($"{result1[i][j], 12} ");
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        [Test]
        public void TryResolveTest()
        {
            var router = RouterService.GetRouter();
            var coordinates = new[]
            {
                new Coordinate(Convert.ToSingle(59.9216610989), Convert.ToSingle(30.3706012271)),
                new Coordinate(Convert.ToSingle(59.9225322240), Convert.ToSingle(30.3460378367)),
                new Coordinate(Convert.ToSingle(59.9479724778), Convert.ToSingle(30.3508321709)),
                new Coordinate(Convert.ToSingle(59.9218572227), Convert.ToSingle(30.3911097174)),
                new Coordinate(Convert.ToSingle(59.9435730581), Convert.ToSingle(30.3661007493)),
                new Coordinate(Convert.ToSingle(59.9448236160), Convert.ToSingle(30.3438447170)),
                new Coordinate(Convert.ToSingle(59.9211824031), Convert.ToSingle(30.3516340839)),
                new Coordinate(Convert.ToSingle(59.9447073010), Convert.ToSingle(30.3580799250)),
                new Coordinate(Convert.ToSingle(59.9237099400), Convert.ToSingle(30.3572230040)),
                new Coordinate(Convert.ToSingle(59.9359486280), Convert.ToSingle(30.3818775840)),
                new Coordinate(Convert.ToSingle(59.9213500041), Convert.ToSingle(30.3948024286)),
                new Coordinate(Convert.ToSingle(59.9160898921), Convert.ToSingle(30.3534265316)),
                new Coordinate(Convert.ToSingle(59.9360099634), Convert.ToSingle(30.3138953743)),
            };

            foreach (var coordinate in coordinates)
            {
                Console.WriteLine("Resolving point: " + coordinate.Latitude + ", " + coordinate.Longitude);
                var result1 = router.TryResolve(new[] { Vehicle.Car.Fastest() },  coordinate.Latitude, coordinate.Longitude,
                    x => true);
                Console.WriteLine(result1.IsError 
                    ? "Result 1 completed with error: " + result1.ErrorMessage 
                    : "Result 1 resolved with coordinate: " + result1.Value);
                var result2 = router.TryResolve(new[] { Vehicle.Car.Fastest() }, coordinate.Latitude, coordinate.Longitude,
                    x => false);
                Console.WriteLine(result2.IsError 
                    ? "Result 2 completed with error: " + result2.ErrorMessage 
                    : "Result 2 resolved with coordinate: " + result2.Value);
                var result3 = router.TryResolve(new[] { Vehicle.Car.Fastest() }, coordinate.Latitude, coordinate.Longitude,
                    x => false, 100F);
                Console.WriteLine(result3.IsError 
                    ? "Result 3 completed with error: " + result3.ErrorMessage 
                    : "Result 3 resolved with coordinate: " + result3.Value);
                var result4 = router.TryResolve(new[] { Vehicle.Car.Fastest() }, coordinate.Latitude, coordinate.Longitude,
                    x => false, 200F);
                Console.WriteLine(result4.IsError 
                    ? "Result 4 completed with error: " + result4.ErrorMessage 
                    : "Result 4 resolved with coordinate: " + result4.Value);
                try
                {
                    var result = router.Resolve(Vehicle.Car.Fastest(), coordinate);
                    Console.WriteLine("Result resolved with coordinate: " + result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Result completed with error: " + e.Message);
                }
            }
        }

        [Test]
        public void OptimizeRoutesTest()
        {
            var router = RouterService.GetRouter();
            var optimizationService = new OptimizationService();
            var coordinates = new[]
            {
                new Coordinate(59.9202541272f, 30.3386695619f),
                new Coordinate(59.9190939589f, 30.3402878697f),
                new Coordinate(59.9183610857f, 30.3409329032f),
                new Coordinate(59.9295678254f, 30.3201181123f),
                new Coordinate(59.9334973694f, 30.3139640327f),
                new Coordinate(59.9380210072f, 30.3126046687f),
            };

            var timeWindows = new long[,]
            {
                {0, 360},
                {0, 600},
                {420, 720},
                {240, 660},
                {780, 1200},
                {360, 420},
            };

            var timeMatrix = RouterService.GetWeightTimeMatrix(coordinates
                .Select(x => router.Resolve(Vehicle.Car.Fastest(), x, 200F))
                .ToArray());
            
            int rowLength = timeMatrix.GetLength(0);
            int colLength = timeMatrix.GetLength(1);

            Console.WriteLine("\nTime Matrix");
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write($"{timeMatrix[i,j], 12} ");
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.WriteLine("\n");
            
            optimizationService.OptimizeRoutesWithTimeMatrix(timeMatrix, timeWindows, 2, 0);
        }
        
        private void BuildAndOutputTimeMatrix(RouterPoint[] routerPoints)
        {
            var router = RouterService.GetRouter();
            var timeMatrix = new float[routerPoints.Length, routerPoints.Length];
            for (var i = 0; i < routerPoints.Length; i++)
            {
                for (var j = 0; j < routerPoints.Length; j++)
                {
                    if (i == j)
                    {
                        timeMatrix[i, j] = 0;
                    }
                    else
                    {
                        try
                        {
                            var route = router.Calculate(Vehicle.Car.Fastest(), routerPoints[i], routerPoints[j]);
                            timeMatrix[i, j] = route.TotalTime;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Route for locations: " + routerPoints[i] + "; " + routerPoints[j] + " not found. Error: " + e.Message);
                            timeMatrix[i, j] = float.MaxValue;
                        }
                    }
                }
            }
            
            int rowLength = timeMatrix.GetLength(0);
            int colLength = timeMatrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write($"{timeMatrix[i, j]} ");
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
    }
}