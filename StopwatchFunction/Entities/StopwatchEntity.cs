using Microsoft.WindowsAzure.Storage.Table;
using StopwatchFunction.Helpers;

namespace StopwatchFunction.Entities
{
    public class StopwatchEntity : TableEntity
    {
        public string UserName { get; set; }
        public string StopWatchName { get; set; }

        public string Status { get; set; }
    }
}
