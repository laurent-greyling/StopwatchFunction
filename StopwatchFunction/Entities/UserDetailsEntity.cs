using Microsoft.WindowsAzure.Storage.Table;

namespace StopwatchFunction.Entities
{
    public class UserDetailsEntity : TableEntity
    {
        public string UserName { get; set; }
        public string StopWatchName { get; set; }

        public string Status { get; set; }

        public string ElapsedTime { get;set; }
    }
}
