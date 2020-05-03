using System;
using System.Collections.Generic;
using System.Threading;
using Common.Helpers;
using Common.Models.BusinessModels;
using OptimizeDelivery.Services.Services;

namespace OptimizeDelivery.DataGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RunIdealCoordinateGeneration();
        }

        private static void RunIdealCoordinateGeneration()
        {
            var idealCoordinateService = new IdealCoordinateService();
            using (var source = new CancellationTokenSource())
            {
                var token = source.Token;
                var task = idealCoordinateService.Run(token);

                Console.WriteLine("Press any key to finish the operation...");
                Console.ReadKey();
                source.Cancel();
                task.Wait();
            }

            Console.WriteLine("Operation finished.");
        }

        private static void FillDistrictsTable()
        {
            Sandbox.FillDistrictsTable();
        }

        private static void PrintDistrictsLocations(Dictionary<District, List<Parcel>> districtsWithParcels)
        {
            foreach (var district in districtsWithParcels)
            {
                Console.WriteLine("Locations for district: " + district.Key.Name);
                foreach (var parcel in district.Value)
                    Console.WriteLine(parcel.OriginalLocation.ToStringNoWhitespace());
                Console.WriteLine("\n\n");
            }
        }
    }
}