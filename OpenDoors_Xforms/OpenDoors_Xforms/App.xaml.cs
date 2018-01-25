using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OpenDoors_Xforms
{
	public partial class App : Application
	{
        public static HubConnection hubConnection;

        public App ()
		{
			InitializeComponent();
            MainPage = new OpenDoors_Xforms.MainPage();            
		}

		protected async override void OnStart ()
		{
            try
            {
                hubConnection = new HubConnectionBuilder().WithUrl("https://rxconnectsite-20180125102749.azurewebsites.net/doors").Build();
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            // Handle when your app starts            
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
