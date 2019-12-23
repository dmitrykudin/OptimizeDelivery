using GoogleMapsApi.Entities.DistanceMatrix.Request;

namespace OptimizeDelivery.MapsAPIIntegration
{
    public static class DistanceMatrix
    {
        public static DistanceMatrixRequest CreateRequest(string apiKey)
        {
            var distanceMatrixRequest = new DistanceMatrixRequest {ApiKey = apiKey};
            return distanceMatrixRequest;
        }
    }
}