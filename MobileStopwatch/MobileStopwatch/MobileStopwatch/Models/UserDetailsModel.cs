namespace MobileStopwatch.Models
{
    public class UserDetailsModel
    {
        public string UserName { get; set; }
        public string StopWatchName { get; set; }

        public string Status { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string ElapsedTime { get; set; }
    }
}
