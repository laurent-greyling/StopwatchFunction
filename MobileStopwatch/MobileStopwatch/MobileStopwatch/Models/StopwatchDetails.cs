using System;
using System.Collections.Generic;
using System.Text;

namespace MobileStopwatch.Models
{
    public class StopwatchDetails
    {
        public string UserName { get; set; }
        public string StopWatchName { get; set; }

        public string Status { get; set; }

        public string ElapsedTime { get; set; }
    }
}
