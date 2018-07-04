using StopwatchFunction.Entities;
using StopwatchFunction.Services;

namespace StopwatchFunction.Helpers
{
    public class SaveStatus
    {
        private readonly IStopwatchDetails _stopwatchDetails;

        public SaveStatus(IStopwatchDetails stopwatchDetails)
        {
            _stopwatchDetails = stopwatchDetails;
        }

        public void Save(StopwatchEntity stopwatchEntity)
        {
            _stopwatchDetails.Save(stopwatchEntity);
        }
    }
}
