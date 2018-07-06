using Microsoft.WindowsAzure.Storage.Table;
using StopwatchFunction.Helpers;

namespace StopwatchFunction.Entities
{
    public class StopwatchEntity : TableEntity
    {
        public string OperationId { get; set; }
        public string UserName { get; set; }
        public string StopWatchName { get; set; }

        public string Status { get; set; }

        public string StartTime { get; set; }

        public bool Start { get; set; }
        public bool Restart { get; set; }
        public bool Stop { get; set; }
        public bool Reset { get; set; }
    }
}
