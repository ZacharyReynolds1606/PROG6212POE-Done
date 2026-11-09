namespace RaceDay.Api.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}

