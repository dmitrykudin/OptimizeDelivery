using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using Newtonsoft.Json;

namespace OptimizeDelivery.Services.Services
{
    public class IdealCoordinateService
    {
        private readonly string IdealCoordinatesFilePath = "C:/Development/Projects/OptimizeDelivery/OptimizeDelivery/IdealCoordinates.json";
        
        private static Random random = new Random();

        public List<Coordinate> CurrentCoordinates { get; set; }
        
        private List<RouterPoint> CurrentRouterPoints { get; set; }

        public IdealCoordinateService()
        {
            Deserialize();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await Task.Run(() => RunIdealCoordinateGeneration(cancellationToken));
        }

        public Coordinate GetIdealPoint()
        {
            return CurrentCoordinates.ToArray()[random.Next(CurrentCoordinates.Count)];
        }
        
        private void RunIdealCoordinateGeneration(CancellationToken cancellationToken)
        {
            var routerDb = ItineroRouter.GetRouterDb();
            var router = ItineroRouter.GetRouter();
            var notSavedPoints = 0;
            var attemptCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                attemptCount++;
                var randomCoordinate = RandHelper.CoordinateFrom(routerDb);
                if (CurrentCoordinates.Contains(randomCoordinate))
                {
                    continue;
                }
                
                var randomPoint = router.Resolve(Vehicle.Car.Fastest(), randomCoordinate);
                var resultPoints = CurrentRouterPoints
                    .Append(randomPoint)
                    .ToArray();

                var weightTimeMatrix = ItineroRouter.GetWeightTimeMatrix(resultPoints);
                if (weightTimeMatrix.Any(x => x.Any(y => y == float.MaxValue)))
                {
                    continue;
                }

                CurrentCoordinates.Add(randomCoordinate);
                CurrentRouterPoints.Add(randomPoint);
                Console.WriteLine(DateTime.Now +  " - Point number " + CurrentCoordinates.Count() + " added after " + attemptCount + " attempt(s).");
                attemptCount = 0;
                notSavedPoints++;
                if (notSavedPoints == 10)
                {
                    Serialize();
                    notSavedPoints = 0;
                    Console.WriteLine(DateTime.Now + " - Points saved. Total: " + CurrentCoordinates.Count() + " points.");
                }
            }

            Serialize();
        }
        
        private void Serialize()
        {
            var serializedCoordinates = CurrentCoordinates
                .Select(x => GeographyHelper.GetWktPoint(x.Latitude, x.Longitude))
                .ToArray();
            var points = new JsonPoints { Points = serializedCoordinates };

            if (!File.Exists(IdealCoordinatesFilePath))
            {
                var stream = File.Create(IdealCoordinatesFilePath);
                stream.Close();
            }

            var serializer = new JsonSerializer();
            using (var streamWriter = new StreamWriter(IdealCoordinatesFilePath))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, points);
            }
        }

        private void Deserialize()
        {
            var router = ItineroRouter.GetRouter();
            if (File.Exists(IdealCoordinatesFilePath))
            {
                var fileText = File.ReadAllText(IdealCoordinatesFilePath);
                var coordinates = JsonConvert.DeserializeObject<JsonPoints>(fileText);
                CurrentCoordinates = coordinates.Points
                    .Select(x => x.ToItineroCoordinate())
                    .ToList();
                CurrentRouterPoints = CurrentCoordinates
                    .Select(x => router.Resolve(Vehicle.Car.Fastest(), x))
                    .ToList();
            }
            else
            {
                CurrentCoordinates = new List<Coordinate>();
                CurrentRouterPoints = new List<RouterPoint>();
            }
        }

        private class JsonPoints
        {
            [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
            public string[] Points { get; set; }
        }
    }
}