using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Xamarin.Forms;

namespace OpenDoors_Xforms
{
	public partial class MainPage : ContentPage
	{
        public Boolean readInside = false;
        public class DoorLog
        {
            public int ID { get; set; }
            public string State { get; set; }
        }

        public MainPage()
		{            
			InitializeComponent();
            this.BackgroundImage = "Assets/Splash.png";
            btnOpen.Clicked += BtnOpen_Clicked;
            btnClose.Clicked += BtnClose_Clicked;
            btnStop.Clicked += BtnStop_Clicked;
            btnListen.Clicked += BtnListen_Clicked;
            }
        async void readMode()
        {
            lblStatus.Text = "Listening";
            try
            {
                await Task.Run(async () =>
                {
                    var channel = await App.hubConnection.StreamAsync("StreamDoors", typeof(DoorLog), new object[] { "AZ3166_Alex" });
                    while (readInside)
                    {
                        var b = await channel.ReadAsync();
                        if (b != null)
                        {
                            Debug.Print(((DoorLog)b).State.ToString());
                            Device.BeginInvokeOnMainThread(() => {
                                lblData.Text = ((DoorLog)b).State.ToString();
                            });
                        }
                        await Task.Delay(10);
                    }
                });
            }
            catch (Exception)
            {
            }
            lblStatus.Text = "Silence";
        }
        private void BtnListen_Clicked(object sender, EventArgs e)
        {
            readInside = !readInside;
            if(readInside)
            {
                readMode();
                btnListen.BackgroundColor = Color.Green;
            }
            else
            {
                btnListen.BackgroundColor = Color.Red;
            }
        }

        private async void BtnStop_Clicked(object sender, EventArgs e)
        {
            await App.hubConnection.SendAsync("Send", new object[] { "** STOP **" });
        }

        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            await App.hubConnection.SendAsync("Send", new object[] { "** CLOSE **" });
        }

        private async void BtnOpen_Clicked(object sender, EventArgs e)
        {
                await App.hubConnection.SendAsync("Send", new object[] { "** OPEN **" });
        }
	}
}
