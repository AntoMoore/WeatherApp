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
        float fTempHigh = 0;
        float speedKph = 0;
        float speedMph = 0;
        string date;
        string day;
        int epoch;
        bool isSwitched = false;
        int isMetric = 0;
        int counter = 0;

        //class objects
        Calculations myCalc = new Calculations();
        ImageGenerator myImg = new ImageGenerator();
        FindDate myUnix = new FindDate();

        //main constructors
        public MainPage()
        {            
            InitializeComponent();
            SetupStyling();
            GetWeather();            
        }
       
        //load embedded CSS
        private void SetupStyling()
        {
            this.Resources.Add(StyleSheet.FromAssemblyResource(
            IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly,
            "Solstice.Assets.style.css"));
        }

        //weather method
        public async void GetWeather()
        {
            try
            {
                //get geolocation
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                counter++;
                                  
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

                        //ImageGenerator class method - WIND COMPASS
                        string strFilename2 = myImg.getCompassUrl((float)windDirection);
                        tpRight.Source = ImageSource.FromResource(strFilename2, assembly);

                        //white transparent tiles
                        string strFilename3 = "Solstice.Assets.Images.tileWhite.png";
                        transparentImage1.Source = ImageSource.FromResource(strFilename3, assembly);
                        transparentImage2.Source = ImageSource.FromResource(strFilename3, assembly);

                        // Data outputs Top Grid
                        tpLeft.Text = city.ToString();
                        
                        if(isSwitched == false)
                        {
                            //if in metric
                            btmLeft.Text = Math.Round(cTemp, 1).ToString() + "°C";
                            btmLeft.Text = Math.Round(cTemp, 1).ToString() + "°C";
                            btmRight.Text = Math.Round(speedKph, 0).ToString() + " Km/h";

                        }
                        else
                        {
                            //if in imperial
                            btmLeft.Text = Math.Round(fTemp, 1).ToString() + "°F";
                            btmLeft.Text = Math.Round(fTemp, 1).ToString() + "°F";
                            btmRight.Text = Math.Round(speedMph, 0).ToString() + " Mp/h";
                        }
                                              
                        btmMid.Text = weatherType.ToString();

                        //DEBUG
                        //myOutput.Text = "DEBUG - Wind: " + Math.Round(speedKph, 2).ToString() + "Km/h " + city.ToString() + "\n " + weatherType.ToString() + " Temp: " + Math.Round(cTemp, 2).ToString() + "­°C"; 
                        
                    }// if responseCurrent
                    else
                    {
                        //JSON empty
                        myOutput.Text = "ERROR - JSON CURRENT NULL";
                    }

                    //JSON call to open weather api (5 day Weather Forecast)
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

                        //make loop for 5 day forecast - NB: loop in increments of 8 because data is every 3 hours (3 * 8 = 24)
                        for(int i = 0; i < 40; i += 8)
                        {
                            //variables created from the api data
                            var unixTime = localForecast["list"][i]["dt"];
                            var theDescription = localForecast["list"][i]["weather"][0]["main"];
                            var highTemp = localForecast["list"][i]["main"]["temp_max"];
                            var lowTemp = localForecast["list"][i]["main"]["temp_min"];

                            //find day of the week
                            epoch = (int)unixTime;
                            date = myUnix.unixToString(epoch);
                            day = myUnix.dateToDay(date);

                            //calculation class methods                       
                            cTempHigh = myCalc.toCelsius((float)highTemp);                            
                            fTempHigh = myCalc.toFahrenheit((float)highTemp);                            

                            //ImageGenerator class method - ICONS (forecast)
                            string strFilename = myImg.getIconUrl((string)theDescription);
                            var assembly = typeof(MainPage);
                           
                            //top row
                            if ((label = (Label)FindByName("top_" + (i + 1).ToString())) != null)
                            {
                                label.Text = day.ToString();
                            }

                            //mid row
                            if ((image = (Image)FindByName("mid_" + (i + 1).ToString())) != null)
                            {
                                image.Source = ImageSource.FromResource(strFilename, assembly);
                            }

                            //bottom row
                            if ((label = (Label)FindByName("btm_" + (i + 1).ToString())) != null)
                            {
                                if (isSwitched == false)
                                {
                                    //if in metric
                                    label.Text = Math.Round(cTempHigh, 1).ToString() + "°C";
                                }
                                else
                                {
                                    //if in imperial
                                    label.Text = Math.Round(fTempHigh, 1).ToString() + "°F";
                                }
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
            }
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

        //change metric/imperial when clicked
        private void toggleBtn_Clicked(object sender, EventArgs e)
        {
            //check if button has been pressed before
            if (isSwitched == false)
            {
                //change output
                toggleBtn.BackgroundColor = Color.FromRgb(255, 165, 0);
                toggleBtn.Text = "Imperial";
                isSwitched = true;
            }
            else
            {
                //change output
                toggleBtn.BackgroundColor = Color.FromRgb(0, 8, 255);
                toggleBtn.Text = "Metric";
                isSwitched = false;
            }

            //reload weather
            GetWeather();


        }

    }//mainpage
}//namespace
