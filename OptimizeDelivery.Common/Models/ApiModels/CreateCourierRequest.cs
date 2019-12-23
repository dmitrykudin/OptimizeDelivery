namespace Common.Models.ApiModels
{
    public class CreateCourierRequest
    {
        public int TelegramId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}