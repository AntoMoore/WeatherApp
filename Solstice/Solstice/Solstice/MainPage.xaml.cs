using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Solstice
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            //constructors
            InitializeComponent();
            SetupImagesOnPage();
            GetWeather();
            GetTime();
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

        //get time and date
        public void GetTime()
        {
            var time = DateTime.Now;
            btmLeft.Text = time.ToString();
        }

        //gps method
        public async void GetWeather()
        {
            //local variables
            double lat;
            double lon;
         
            try
            {
                //get geolocation
                var location = await Geolocation.GetLastKnownLocationAsync();

                //test if location is not empty
                if (location != null)
                {
                    //variables for gps
                    lat = location.Latitude;
                    lon = location.Longitude;

                    //Debug
                    //myOutput.Text = "DEBUG: "+ lat.ToString() + " " + lon.ToString();

                    //JSON call to open weather api
                    HttpClient client = new HttpClient();
                    var url = String.Format("http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid=3ce66444abe0bb9e6dec0fbfecbd27be", lat, lon);
                    var response = await client.GetStringAsync(url);

                    //JSON call for 5 day weather

                    //test if response is not empty
                    if(response != null)
                    {
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

                        //outputs
                        tpLeft.Text = city.ToString();
                        tpMid.Text = Math.Round(cTemp, 2).ToString() + "°C";
                        tpRight.Text = "Image Here";
                        //btmLeft is the Time/Date
                        btmMid.Text = Math.Round(speedKph, 2).ToString() + " Km/h";
                        btmRight.Text = weatherType.ToString();

                        //DEBUG
                        //myOutput.Text = "DEBUG - Wind: " + Math.Round(speedKph, 2).ToString() + "Km/h " + city.ToString() + "\n " + weatherType.ToString() + " Temp: " + Math.Round(cTemp, 2).ToString() + "­°C";                        
                    }
                    else
                    {
                        //JSON empty
                        myOutput.Text = "ERROR - JSON NULL";
                    }
                }//if
                else
                {
                    //location empty
                    myOutput.Text = "ERROR - GPS NULL";
                }
            }//try
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                myOutput.Text = "ERROR - Not Supported on Device: " + fnsEx.ToString();
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                myOutput.Text = "ERROR - Permission Exception: " + pEx.ToString();
            }
            catch (Exception ex)
            {
                // Unable to get location
                myOutput.Text = "ERROR - Unable to Get Location: " + ex.ToString();
            }
          
        }//getGPS
    }//mainpage
}//namespace
