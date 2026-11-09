using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Enrolments
{
    public class EnrolmentDto
    {
        public int EnrolmentId { get; set; }

        public string EventName { get; set; }

        public string CategoryName { get; set; }

        public string Status { get; set; }
    }
}

