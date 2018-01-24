using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Forms;

namespace OpenDoors_Xforms
{
	public partial class MainPage : ContentPage
	{
     
		public MainPage()
		{            
			InitializeComponent();
            btnOpen.Clicked += BtnOpen_Clicked;
		}

        private void BtnOpen_Clicked(object sender, EventArgs e)
        {
            try
            {
                string a = "Hola";                      
                App.mobileHubProxy.Invoke("Send", new object[] { "RxDroidApp", "Darta Abre la puerta" });
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
	}
}
