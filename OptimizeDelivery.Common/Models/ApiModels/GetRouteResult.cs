namespace Common.Models.ApiModels
{
    public class GetRouteResult
    {
        public string Status { get; set; }

        public string RouteUrl { get; set; }

        public RouteStep[] Steps { get; set; }
    }
}