using MobileStopwatch.Helpers;
using MobileStopwatch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileStopwatch.ViewModels
{
    public class UserStopwatchDetails : INotifyPropertyChanged
    {
        public string _userDetails { get; set; }

        public string UserDetails
        {
            get
            {
                return _userDetails;
            }
            set
            {
                if (_userDetails != value)
                {
                    _userDetails = value;
                    OnPropertyChanged("UserDetails");
                }
            }
        }

        public UserStopwatchDetails(string userRequest)
        {
            UserDetails = RetrieveUserDetails(userRequest);
        }

        private string RetrieveUserDetails(string userRequest)
        {
            var request = WebRequest.Create(new Uri(AppConstants.GetStopwatchUrl));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
