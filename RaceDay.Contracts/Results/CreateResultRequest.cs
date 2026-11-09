using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Results
{
    public class CreateResultRequest
    {
        public int EnrolmentId { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int Position { get; set; }
    }
}
