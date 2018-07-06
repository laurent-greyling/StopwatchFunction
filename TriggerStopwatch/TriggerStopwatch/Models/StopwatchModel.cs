namespace TriggerStopwatch.Models
{
    public class StopwatchModel
    {
        public string UserName { get; set; }
        public string StopWatchName { get; set; }
        public bool Start { get; set; }
        public bool Restart { get; set; }
        public bool Stop { get; set; }
        public bool Reset { get; set; }
    }
}
