using MobileStopwatch.Helpers;
using MobileStopwatch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

namespace MobileStopwatch.ViewModels
{
    public class ElapsedTimeViewModel : INotifyPropertyChanged
    {
        private TimeSpan _timeSpan;
        private string _timerString;
        public StopwatchDetails _details { get; set; }
        public string Title { get; set; }

        public TimeSpan TimeSpanx
        {
            get
            {
                return _timeSpan;
            }
            set
            {
                if (_timeSpan != value)
                {
                    _timeSpan = value;
                    OnPropertyChanged("TimeSpanx");
                }
            }
        }

        public string TimerString
        {
            get
            {
                return _timerString;
            }
            set
            {
                if (_timerString != value)
                {
                    _timerString = value;
                    OnPropertyChanged("TimerString");
                }
            }
        }

        public StopwatchDetails Details
        {
            get
            {
                return _details;
            }
            set
            {
                if (_details != value)
                {
                    _details = value;
                    OnPropertyChanged("Details");
                }
            }
        }

        public ElapsedTimeViewModel(StopwatchDetails stopwatchDetails)
        {
            Title = $"{stopwatchDetails.UserName} {stopwatchDetails.StopWatchName}";
            var time = TimeSpan.TryParse(stopwatchDetails.ElapsedTime, out TimeSpan serverTimeSpan);
            _timeSpan = serverTimeSpan;
            _timerString = stopwatchDetails.ElapsedTime;

            if (stopwatchDetails.Status != StopwatchStatus.Stop.ToString())
            {
                var sw = new Timer(0.001);
                sw.Elapsed += Elapsed_Time;

                sw.Start();
            }            

            Details = stopwatchDetails;
        }

        private void Elapsed_Time(object sender, ElapsedEventArgs e)
        {
            var timer = TimeSpan.FromSeconds(0.001);
            TimeSpanx = TimeSpanx.Add(timer);
            TimerString = TimeSpanx.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
