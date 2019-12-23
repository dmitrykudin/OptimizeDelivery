namespace Common.Models.ApiModels
{
    public class RouteStep
    {
        public int StepNumber { get; set; }

        public string DestinationAddress { get; set; }

        public string Distance { get; set; }

        public override string ToString()
        {
            return (StepNumber + 1) + ". " + DestinationAddress + ". (" + Distance + ")";
        }
    }
}