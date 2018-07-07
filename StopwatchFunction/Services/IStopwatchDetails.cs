using System.Collections.Generic;
using System.Threading.Tasks;
using StopwatchFunction.Entities;
using StopwatchFunction.Models;

namespace StopwatchFunction.Services
{
    public interface IStopwatchDetails
    {
        Task Save(StopwatchEntity stopwatchEntity);
        Task<List<UserDetailsModel>> Retrieve(UserDetailsEntity userDetailsEntity);
        Task<List<UserDetailsModel>> RetrieveElaspedTime(UserDetailsEntity userDetailsEntity);
    }
}
