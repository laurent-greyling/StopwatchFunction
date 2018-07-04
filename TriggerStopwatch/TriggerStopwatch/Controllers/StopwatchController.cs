using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace TriggerStopwatch.Controllers
{
    public class StopwatchController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]string stopwatchDetails)
        {
            using (var client = new HttpClient())
            {
                const string url = "http://localhost:7071/api/CreateStopwatch";
                var content = new StringContent(stopwatchDetails);
                return await client.PostAsync(url, content);
            }
        }
    }
}
