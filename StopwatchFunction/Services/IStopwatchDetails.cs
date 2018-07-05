using System.Threading.Tasks;
using StopwatchFunction.Entities;

namespace StopwatchFunction.Services
{
    public interface IStopwatchDetails
    {
        Task Save(StopwatchEntity stopwatchEntity);
    }
}
