namespace Common.Models.ServiceModels
{
    public class RoutePlanDestination
    {
        public int DestinationId { get; set; }

        public long ArrivalTimeFrom { get; set; }
        
        public long ArrivalTimeTo { get; set; }
    }
}