namespace Common.Models.ServiceModels
{
    public class RouteDestination
    {
        public int DestinationId { get; set; }

        public long ArrivalTimeFrom { get; set; }
        
        public long ArrivalTimeTo { get; set; }
    }
}