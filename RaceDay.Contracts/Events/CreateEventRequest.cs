using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Events
{
    public class CreateEventRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime EventDate { get; set; }

        public string Location { get; set; }

        public decimal DistanceKm { get; set; }

        public string EventType { get; set; }
    }
}
