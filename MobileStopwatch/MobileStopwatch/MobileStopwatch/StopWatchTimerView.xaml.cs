using MobileStopwatch.Helpers;
using MobileStopwatch.Models;
using MobileStopwatch.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileStopwatch
{    
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StopWatchTimerView : ContentPage
	{
        public ElapsedTimeViewModel Details { get; set; }
        private StopwatchDetails UserDetails { get; set; }

        public StopWatchTimerView (StopwatchDetails details)
		{
			InitializeComponent ();
            UserDetails = details;
            Details = new ElapsedTimeViewModel(details);

            BindingContext = Details;
        }

        private void Stop_Timer()
        {
            UserDetails.Status = StopwatchStatus.Stop.ToString();
            UserDetails.ElapsedTime = TimeString.Text;
            UserDetails.Stop = true;
            UserDetails.Restart = false;
            UserDetails.Start = false;
            var userRequest = JsonConvert.SerializeObject(UserDetails);
            var setTimer = SetTimer(userRequest);

            Details = new ElapsedTimeViewModel(UserDetails);

            BindingContext = Details;
        }

        private void Restart_Timer()
        {
            UserDetails.Status = StopwatchStatus.Restart.ToString();
            var userRequest = JsonConvert.SerializeObject(UserDetails);
            UserDetails.Restart = true;
            UserDetails.Start = false;
            UserDetails.Stop = false;
            var setTimer = SetTimer(userRequest);

            Details = new ElapsedTimeViewModel(UserDetails);

            BindingContext = Details;
        }

        private void Start_Timer()
        {
            UserDetails.Status = StopwatchStatus.Start.ToString();
            var userRequest = JsonConvert.SerializeObject(UserDetails);
            UserDetails.Start = true;
            UserDetails.Restart = false;
            UserDetails.Stop = false;
            var setTimer = SetTimer(userRequest);

            Details = new ElapsedTimeViewModel(UserDetails);

            BindingContext = Details;
        }

        private string SetTimer(string userRequest)
        {
            var request = WebRequest.Create(new Uri(AppConstants.CreateStopwatchUrl));
            request.ContentType = "application/json";
            request.Method = "POST";

            request.Timeout = 15000;

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(userRequest);
                writer.Flush();
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}