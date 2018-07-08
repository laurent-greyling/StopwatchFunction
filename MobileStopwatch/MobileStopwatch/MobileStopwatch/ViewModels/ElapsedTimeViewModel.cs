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
        public List<StopwatchDetails> _details { get; set; }

        public List<StopwatchDetails> Details
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

        public ElapsedTimeViewModel(List<StopwatchDetails> stopwatchDetails)
        {
            var sw = new Timer
            {
                Interval = 1
            };
            sw.Start();
            foreach (var item in stopwatchDetails)
            {                
                var time = TimeSpan.TryParse(item.ElapsedTime, out _timeSpan);
                sw.Elapsed += Elapsed_Time;
                item.ElapsedTime = _timeSpan.ToString();
            }

            Details = stopwatchDetails;
        }

        private void Elapsed_Time(object sender, ElapsedEventArgs e)
        {
            var second = TimeSpan.FromSeconds(1);
            _timeSpan.Add(second);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
