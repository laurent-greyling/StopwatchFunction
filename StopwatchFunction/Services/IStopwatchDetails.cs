using StopwatchFunction.Entities;

namespace StopwatchFunction.Services
{
    public interface IStopwatchDetails
    {
        void Save(StopwatchEntity stopwatchEntity);
    }
}
