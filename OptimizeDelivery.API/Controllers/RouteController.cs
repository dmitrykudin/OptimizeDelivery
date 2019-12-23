using System.Configuration;
using System.Web.Http;
using OptimizeDelivery.MapsAPIIntegration;

namespace OptimizeDelivery.API.Controllers
{
    public class RouteController : ApiController
    {
        private readonly string GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];

        public string Test()
        {
            var request = DistanceMatrix.CreateRequest(GoogleMapsApiKey);
            return request.ToString();
        }
    }
}