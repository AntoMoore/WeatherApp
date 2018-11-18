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
        //Global variables
        double lat;
        double lon;
     
        public MainPage()
        {
            //constructors
            InitializeComponent();
            SetupBackground();
            GetGps();           
        }

        //images embedded reasource
        private void SetupBackground()
        {
            // get the assembly and file location
            var assembly = typeof(MainPage);
            string strFilename = "Solstice.Assets.Images.backgroundimg.png";
            //output background image
            imageBackground.Source = ImageSource.FromResource(strFilename, assembly);
        }
        
        //gps method
        public async void GetGps()
        {
            try
            {
                //get geolocation
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                //test if location is found
                if (location != null)
                {
                    //variables for gps
                    lat = location.Latitude;
                    lon = location.Longitude;

                    //Debug
                    //myOutput.Text = "DEBUG: "+ lat.ToString() + " " + lon.ToString();
                  
                    //JSON call to open weather api (Current Weather)
                    HttpClient current = new HttpClient();
                    var url = String.Format("http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid=c16bcf9b4251e961d8106438b0711041", lat, lon);
                    var responseCurrent = await current.GetStringAsync(url);              

                    //test if responseCurrent is not empty
                    if(responseCurrent != null)
                    {
                        var localWeather = JObject.Parse(responseCurrent);

                        //variables created from the api data                       
                        var city = localWeather["name"];
                        var kTemp = localWeather["main"]["temp"];
                        var weatherType = localWeather["weather"][0]["description"];
                        var windSpeed = localWeather["wind"]["speed"];
                        var windDirection = localWeather["wind"]["deg"];

                        //local variables
                        float cTemp = 0;
                        double speedKph;

                        //tempreture from kelvin to celsius
                        cTemp = ((float)kTemp - 273);
                        //convert m/s into km/h
                        speedKph = (float)windSpeed * 3.6;

                        //outputs Top Grid
                        tpLeft.Text = city.ToString();
                        tpMid.Text = Math.Round(cTemp, 2).ToString() + "°C";
                        tpRight.Text = "Image Here";
                        btmLeft.Text = Math.Round(speedKph, 2).ToString() + " Km/h";
                        btmMid.Text = windDirection.ToString() + "°Deg";
                        btmRight.Text = weatherType.ToString();

                        //DEBUG
                        //myOutput.Text = "DEBUG - Wind: " + Math.Round(speedKph, 2).ToString() + "Km/h " + city.ToString() + "\n " + weatherType.ToString() + " Temp: " + Math.Round(cTemp, 2).ToString() + "­°C";                        
                    }// if responseCurrent
                    else
                    {
                        //JSON empty
                        myOutput.Text = "ERROR - JSON CURRENT NULL";
                    }

                    //JSON call to open weather api (Weather Forecast)
                    HttpClient forecast = new HttpClient();
                    var url2 = string.Format("http://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&appid=c16bcf9b4251e961d8106438b0711041", lat, lon);
                    var responseForecast = await forecast.GetStringAsync(url2);

                    //test if responseForecast is not empty
                    if (responseForecast != null)
                    {
                        var localForecast = JObject.Parse(responseForecast);
                        //make label object
                        Label label;

                        //make loop for 5 day forecast
                        for(int i = 0; i < 5; i++)
                        {
                            //variables created from the api data
                            var theDescription = localForecast["list"][i]["weather"][0]["description"];
                            var highTemp = localForecast["list"][i]["main"]["temp_max"];
                            var lowTemp = localForecast["list"][i]["main"]["temp_min"];

                            //top row
                            if( (label = (Label)FindByName("top_" + (i + 1).ToString())) != null )
                            {
                                label.Text = theDescription.ToString();                               
                            }

                            //middle row
                            if ((label = (Label)FindByName("mid_" + (i + 1).ToString())) != null)
                            {
                                label.Text = highTemp.ToString();
                            }

                            //bottom row
                            if ((label = (Label)FindByName("btm_" + (i + 1).ToString())) != null)
                            {                               
                                label.Text = lowTemp.ToString();
                            }

                        }//for                       
                    }//if
                    else
                    {
                        //JSON empty
                        myOutput.Text = "ERROR - JSON CURRENT NULL";
                    }

                }//if location is found
                else
                {
                    // GPS location not found
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
