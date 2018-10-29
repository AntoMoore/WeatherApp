using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Solstice
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            SetupImagesOnPage();
            GetGPS();
        }

        //images method
        private void SetupImagesOnPage()
        {
            // get the assembly and file location
            var assembly = typeof(MainPage);
            string strFilename = "Solstice.Assets.Images.mysunflower.png";
            //output background image
            imageBackground.Source = ImageSource.FromResource(strFilename, assembly);
        }

        //gps method
        private async void GetGPS()
        {
            //variables
            double lat;
            double lon;

            //request user permission for geolocation
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    lat = location.Latitude;
                    lon = location.Longitude;

                    lblLatitude.Text = "Latitude: " + Math.Round(lat, 2);
                    lblLongitude.Text = "Longitude: " + Math.Round(lon, 2);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }

            
        }
    }
}
