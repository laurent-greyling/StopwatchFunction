using Xamarin.Forms;
using System.Net.Http;
using MobileStopwatch.Models;
using Newtonsoft.Json;
using MobileStopwatch.ViewModels;
using System.Threading.Tasks;

namespace MobileStopwatch
{
	public partial class MainPage : ContentPage
	{
        public UserStopwatchDetails UserDetails { get; set; }

        public MainPage()
		{
			InitializeComponent();
            
        }

        private async Task View_Stopwatch()
        {
            try
            {

                if (string.IsNullOrEmpty(UserName.Text))
                {
                    await DisplayAlert("error", "supply userName", "ok");
                    return;
                }

                UserDetails = new UserStopwatchDetails(JsonData());

                await Navigation.PushModalAsync(new StopwatchView(UserDetails.UserDetails));
            }
            catch (System.Exception)
            {
                await DisplayAlert("error", "unknown error, try again later", "ok");
            }
        }

        public async Task Create_Stopwatch()
        {
            if (string.IsNullOrEmpty(UserName.Text) && string.IsNullOrEmpty(StopwatchName.Text))
            {
                await DisplayAlert("error", "supply user name and stopwatch name", "ok");
                return;
            }

            UserDetails = new UserStopwatchDetails(JsonData());
        }

        private string JsonData()
        {
            return JsonConvert.SerializeObject(new UserDetailsModel
            {
                UserName = UserName.Text,
                StopWatchName = StopwatchName.Text
            });
        }
    }
}
