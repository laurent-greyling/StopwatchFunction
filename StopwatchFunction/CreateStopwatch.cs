using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using StopwatchFunction.Entities;
using StopwatchFunction.Helpers;
using StopwatchFunction.Services;

namespace StopwatchFunction
{
    public static class CreateStopwatch
    {
        [FunctionName("CreateStopwatch")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "CreateStopwatch")]HttpRequestMessage req,
            TraceWriter log)
        {
            var data = await req.Content.ReadAsStringAsync();
            var requestBody = JsonConvert.DeserializeObject<StopwatchEntity>(data);

            if (string.IsNullOrEmpty(requestBody.UserName) && string.IsNullOrEmpty(requestBody.StopWatchName))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest,
                    "Please pass a username and/or stopwatchname on the query string or in the request body");
            }

            IStopwatchDetails stopwatchDetails = new StopwatchDetails();
            var stopwatchStatus = new SaveStatus(stopwatchDetails);
            stopwatchStatus.Save(requestBody);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
