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

        private List<Coordinate> CurrentCoordinates { get; set; }

        public IdealCoordinateService()
        {
            Deserialize();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await Task.Run(() => RunIdealCoordinateGeneration(cancellationToken));
        }

        public void Try(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Haha");
                Thread.Sleep(5000);
            }
            
            Console.WriteLine("Cancelled");
        }
        
        public void RunIdealCoordinateGeneration(CancellationToken cancellationToken)
        {
            /*
            Thread.Sleep(5000);
            return Task.CompletedTask;
            */
            
            
            var routerDb = RouterService.GetRouterDb();
            var router = RouterService.GetRouter();
            var notSavedPoints = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var randomCoordinate = RandHelper.CoordinateFrom(routerDb);
                if (CurrentCoordinates.Contains(randomCoordinate))
                {
                    continue;
                }

                var coordinates = CurrentCoordinates
                    .Append(randomCoordinate)
                    .ToArray();
                var resultPoints = coordinates
                    .Select(x => router.Resolve(Vehicle.Car.Fastest(), x))
                    .ToArray();

                var weightTimeMatrix = RouterService.GetWeightTimeMatrix(resultPoints);
                if (weightTimeMatrix.Any(x => x.Any(y => y == float.MaxValue)))
                {
                    continue;
                }

                CurrentCoordinates.Add(randomCoordinate);
                notSavedPoints++;
                if (notSavedPoints == 10)
                {
                    Serialize();
                    notSavedPoints = 0;
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
            using var streamWriter = new StreamWriter(IdealCoordinatesFilePath);
            using (var writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, points);
            }
        }

        private void Deserialize()
        {
            if (File.Exists(IdealCoordinatesFilePath))
            {
                var fileText = File.ReadAllText(IdealCoordinatesFilePath);
                var coordinates = JsonConvert.DeserializeObject<JsonPoints>(fileText);
                CurrentCoordinates = coordinates.Points
                    .Select(x => x.ToItineroCoordinate())
                    .ToList();
            }
            else
            {
                CurrentCoordinates = new List<Coordinate>();
            }
        }

        private class JsonPoints
        {
            [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
            public string[] Points { get; set; }
        }
    }
}