using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Solstice
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            SetupImagesOnPage();
            GetWeather();
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
        public async void GetWeather()
        {
            //local variables
            double lat;
            double lon;
           
            try
            {
                //request user permission for geolocation
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    //variables for gps
                    lat = location.Latitude;
                    lon = location.Longitude;

                    //JSON call to open weather api
                    HttpClient client = new HttpClient();
                    var url = String.Format("http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid=3ce66444abe0bb9e6dec0fbfecbd27be", lat, lon);
                    var response = await client.GetStringAsync(url);
                    var localWeather = JObject.Parse(response);
                    
                    //variables created from the api data
                    var weatherType = localWeather["weather"][0]["description"];
                    var windSpeed = localWeather["wind"]["speed"];
                    var city = localWeather["name"];
                    var kTemp = localWeather["main"]["temp"];

                    //local variables
                    float cTemp = 0;
                    double speedKph;

                    //tempreture from kelvin to celsius
                    cTemp = ((float)kTemp - 273);                  
                    //convert m/s into km/h
                    speedKph = (float)windSpeed * 3.6;

                    //Test outputs
                    myOutput.Text = "Wind: " + Math.Round(speedKph, 2).ToString() + "Km/h " + city.ToString() + "\n " + weatherType.ToString() + " Temp: " + Math.Round(cTemp, 2).ToString() + "­°C";

                }//if
            }//try
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
          
        }//getGPS
    }//mainpage
}//namespace
