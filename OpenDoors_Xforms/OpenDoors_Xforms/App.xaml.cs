using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace OpenDoors_Xforms
{
	public partial class App : Application
	{
        static HubConnection hubConnection = new HubConnection("http://localhost:63764/Hub");
        public static IHubProxy mobileHubProxy = hubConnection.CreateHubProxy("RxHub");


        public App ()
		{
			InitializeComponent();
            MainPage = new OpenDoors_Xforms.MainPage();
		}

		protected override void OnStart ()
		{
            try
            {
                hubConnection.Start();
                mobileHubProxy.Invoke("Send", new object[] { "RxDroidApp", "PepLluis" });
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
