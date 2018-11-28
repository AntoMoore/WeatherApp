using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.StyleSheets;
using System.Reflection;

namespace Solstice
{
    public partial class MainPage : ContentPage
    {
        //Global variables
        double lat = 0;
        double lon = 0;
        float cTemp = 0;
        float fTemp = 0;
        float cTempHigh = 0;
        float cTempLow = 0;
        float fTempHigh = 0;
        float fTempLow = 0;
        float speedKph = 0;
        float speedMph = 0;

        //class objects
        Calculations myCalc = new Calculations();
        ImageGenerator myImg = new ImageGenerator();

        //main constructors
        public MainPage()
        {            
            InitializeComponent();
            SetupStyling();
            GetGps();
            
        }


        //load embedded CSS
        private void SetupStyling()
        {
            this.Resources.Add(StyleSheet.FromAssemblyResource(
            IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly,
            "Solstice.Assets.style.css"));
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
                    //local variables for gps
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
                        //JSON object
                        var localWeather = JObject.Parse(responseCurrent);

                        //variables created from the api data                       
                        var city = localWeather["name"];
                        var kTemp = localWeather["main"]["temp"];
                        var weatherType = localWeather["weather"][0]["main"];
                        var windSpeed = localWeather["wind"]["speed"];
                        var windDirection = localWeather["wind"]["deg"];

                        //check if wind Direction is null (ie. There is no wind)
                        if(windDirection == null)
                        {
                            windDirection = 0;
                        }
                                              
                        //calculation class methods                       
                        cTemp = myCalc.toCelsius((float)kTemp);
                        fTemp = myCalc.toFahrenheit((float)kTemp);
                        speedKph = myCalc.toKilometers((float)windSpeed);
                        speedMph = myCalc.toMiles((float)windSpeed);

                        //ImageGenerator class method - BACKGROUND
                        var assembly = typeof(MainPage);
                        string strFilename = myImg.getBackgroundUrl((float)cTemp, (string)weatherType);
                        imageBackground.Source = ImageSource.FromResource(strFilename, assembly);

                        //ImageGenerator class method - ICONS (current weather)
                        string strFilename1 = myImg.getIconUrl((string)weatherType);
                        tpMid.Source = ImageSource.FromResource(strFilename1, assembly);

                        //ImageGenerator class method - COMPASS
                        string strFilename2 = myImg.getCompassUrl((float)windDirection);
                        tpRight.Source = ImageSource.FromResource(strFilename2, assembly);

                        //white transparent tiles
                        string strFilename3 = "Solstice.Assets.Images.whiteBackground.png";
                        transparentImage1.Source = ImageSource.FromResource(strFilename3, assembly);
                        transparentImage2.Source = ImageSource.FromResource(strFilename3, assembly);

                        // Data outputs Top Grid
                        tpLeft.Text = city.ToString();
                        btmLeft.Text = Math.Round(cTemp, 1).ToString() + " °C";
                        btmRight.Text = Math.Round(speedKph, 1).ToString() + " Km/h";                      
                        btmMid.Text = weatherType.ToString();

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
                        //JSON object
                        var localForecast = JObject.Parse(responseForecast);

                        //make label & image object
                        Label label;
                        Image image;

                        //make loop for 5 day forecast
                        for(int i = 0; i < 5; i++)
                        {
                            //variables created from the api data
                            var theDescription = localForecast["list"][i]["weather"][0]["main"];
                            var highTemp = localForecast["list"][i]["main"]["temp_max"];
                            var lowTemp = localForecast["list"][i]["main"]["temp_min"];

                            //calculation class methods                       
                            cTempHigh = myCalc.toCelsius((float)highTemp);
                            cTempLow = myCalc.toCelsius((float)lowTemp);
                            fTempHigh = myCalc.toFahrenheit((float)highTemp);
                            fTempLow = myCalc.toFahrenheit((float)lowTemp);

                            //ImageGenerator class method - ICONS (forecast)
                            string strFilename = myImg.getIconUrl((string)theDescription);
                            var assembly = typeof(MainPage);

                            //top row
                            if ((image = (Image)FindByName("top_" + (i + 1).ToString())) != null)
                            {
                                image.Source = ImageSource.FromResource(strFilename, assembly);
                            }

                            //middle row
                            if ((label = (Label)FindByName("mid_" + (i + 1).ToString())) != null)
                            {
                                label.Text = Math.Round(cTempHigh, 2).ToString() + "°C";
                            }

                            //bottom row
                            if ((label = (Label)FindByName("btm_" + (i + 1).ToString())) != null)
                            {
                                label.Text = Math.Round(cTempLow, 2).ToString() + "°C";
                            }

                        }//for                       
                    }//if
                    else
                    {
                        //JSON empty
                        myOutput.Text = "ERROR - JSON CURRENT NULL";
                    }
                }//if GPS location is found
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
                myOutput.Text = "ERROR: " + ex.ToString();
            }
          
        }//getGPS
    }//mainpage
}//namespace
