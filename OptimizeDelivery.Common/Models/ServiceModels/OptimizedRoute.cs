namespace Common.Models.ServiceModels
{
    public class OptimizedRoute
    {
        public long TotalTime { get; set; }

        public RouteDestination[] OrderedDestinations { get; set; }
    }
}