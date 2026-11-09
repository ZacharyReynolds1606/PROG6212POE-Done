using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Results
{
    public class ResultDto
    {
        public string EventName { get; set; }

        public string CategoryName { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int Position { get; set; }
    }
}
