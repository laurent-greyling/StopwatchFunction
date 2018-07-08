using MobileStopwatch.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileStopwatch
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StopwatchView : ContentPage
	{
        public List<StopwatchDetails> UserDetails { get; set; }
		public StopwatchView (string stopwatchDetails)
		{
			InitializeComponent ();
            var details = stopwatchDetails.TrimStart('\"').TrimEnd('\"').Replace("\\", "");
            UserDetails = JsonConvert.DeserializeObject<List<StopwatchDetails>>(details);

            BindingContext = this;
		}

        private async Task Show_Sopwatch(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            var stopwatchItem = (StopwatchDetails)e.Item;

            await Navigation.PushModalAsync(new StopWatchTimerView(stopwatchItem));
        }

    }
}