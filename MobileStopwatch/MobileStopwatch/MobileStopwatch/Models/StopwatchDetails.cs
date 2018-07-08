
namespace MobileStopwatch.Models
{
    public class StopwatchDetails
    {
        public string UserName { get; set; }
        public string StopWatchName { get; set; }

        public string Status { get; set; }

        public string ElapsedTime { get; set; }

        public string StartTime { get; set; }

        public bool Start { get; set; }
        public bool Restart { get; set; }
        public bool Stop { get; set; }
    }
}
