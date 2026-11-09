using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Enrolments
{
    public class CreateEnrolmentRequest
    {
        public int EventId { get; set; }

        public int CategoryId { get; set; }
    }
}
