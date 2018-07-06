using System;
using System.Net.Http;
using System.Text;
using CommandLine;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using TriggerStopwatch.Models;

namespace TriggerStopwatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(async opt =>
                {
                    const string baseAddress = "http://localhost:9000/";

                    var stopwatchDetails = JsonConvert.SerializeObject(new StopwatchModel
                    {
                        UserName = opt.UserName,
                        StopWatchName = opt.StopwatchName,
                        Start = opt.Start,
                        Stop = opt.Stop,
                        Reset = opt.Reset,
                        Restart = opt.Restart
                    });
                    using (WebApp.Start<Startup>(url: baseAddress))
                    {
                        using (var client = new HttpClient())
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(stopwatchDetails), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync($"{baseAddress}/api/Stopwatch", content);

                            Console.WriteLine(response);
                        }
                    }
                })
                .WithNotParsed((err) => Console.WriteLine(err));

            Console.ReadLine();
        }
    }
}
