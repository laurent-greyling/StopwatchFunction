using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using StopwatchFunction.Entities;
using StopwatchFunction.Helpers;
using StopwatchFunction.Services;

namespace StopwatchFunction
{
    public static class GetStopwatch
    {
        [FunctionName("GetStopwatch")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetStopwatch")]HttpRequestMessage req,
            TraceWriter log)
        {
            var configuration = new HttpConfiguration();
            req.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey] = configuration;

            var data = await req.Content.ReadAsStringAsync();
            var requestBody = JsonConvert.DeserializeObject<UserDetailsEntity>(data);

            if (string.IsNullOrEmpty(requestBody.UserName) && string.IsNullOrEmpty(requestBody.StopWatchName))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest,
                    "Please pass a username and/or stopwatchname on the query string or in the request body");
            }

            IAzureService azureService = new AzureService();
            IStopwatchDetails stopwatchDetails = new StopwatchDetails(azureService);
            var stopwatchStatus = new GetStatus(stopwatchDetails);
            var userDetails = JsonConvert.SerializeObject(await stopwatchStatus.RetrieveElapsedTime(requestBody)); //If webjob is running change this to .Retrieve

            return req.CreateResponse(HttpStatusCode.OK, userDetails);
        }
    }
}
