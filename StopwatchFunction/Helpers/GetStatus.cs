using StopwatchFunction.Entities;
using StopwatchFunction.Models;
using StopwatchFunction.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StopwatchFunction.Helpers
{
    public class GetStatus
    {
        private readonly IStopwatchDetails _stopwatchDetails;

        public GetStatus(IStopwatchDetails stopwatchDetails)
        {
            _stopwatchDetails = stopwatchDetails;
        }

        public async Task<List<UserDetailsModel>> Retrieve(UserDetailsEntity userDetails)
        {
            return await _stopwatchDetails.Retrieve(userDetails);
        }

        public async Task<List<UserDetailsModel>> RetrieveElapsedTime(UserDetailsEntity userDetails)
        {
            return await _stopwatchDetails.RetrieveElaspedTime(userDetails);
        }
    }
}
