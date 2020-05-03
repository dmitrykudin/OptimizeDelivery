namespace Common.Models.ServiceModels
{
    public class OptimalRoutePlan
    {
        public long TotalTime { get; set; }

        public RoutePlanDestination[] OrderedDestinations { get; set; }
    }
}