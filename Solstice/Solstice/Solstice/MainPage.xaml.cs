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
using System.IO;

namespace Solstice
{
    public partial class MainPage : ContentPage
    {
        //constant
        private const string OUTPUT_FILE = "MyWeatherData.txt";

        //Global variables
        string firstURL, secondURL;
        double lat = 0;
        double lon = 0;
        float[] kTempArr = new float[40];
        float cTemp = 0;
        float fTemp = 0;
        float cTempHigh = 0;
        float fTempHigh = 0;
        float speedKph = 0;
        float speedMph = 0;
        string date;
        string day;
        string citySearch = null;
        int epoch;
        int isMetric;       
        
        //class objects
        Calculations myCalc = new Calculations();
        ImageGenerator myImg = new ImageGenerator();
        FindDate myUnix = new FindDate();

        //main constructors
        public MainPage()
        {            
            InitializeComponent();
            LoadFile();
            SetupStyling();
            GetWeather();           
        }

        //load settings from file
        private void LoadFile()
        {
            // check if a file exists and try load it
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                // "MyWeatherData.txt"
                string fileName = Path.Combine(path, OUTPUT_FILE);
                //read file
                string textInput = File.ReadAllText(fileName);
                isMetric = Convert.ToInt32(textInput);

                // 1 or 0 (1 = true, 0 = false)
                if (isMetric == 1)
                {
                    //set button style blue
                    toggleBtn.BackgroundColor = Color.FromRgb(0, 8, 255);
                    toggleBtn.Text = "Metric";                   
                }
                else
                {
                    //set button style orange
                    toggleBtn.BackgroundColor = Color.FromRgb(255, 165, 0);
                    toggleBtn.Text = "Imperial";
                }
            }
            catch
            {
                //set button style blue
                toggleBtn.BackgroundColor = Color.FromRgb(0, 8, 255);
                toggleBtn.Text = "Metric";
                isMetric = 1;
            }
                       
        }//loadFile

        //load embedded CSS
        private void SetupStyling()
        {
            this.Resources.Add(StyleSheet.FromAssemblyResource(
            IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly,
            "Solstice.Assets.style.css"));

        }//setupStyling

        //weather method
        public async void GetWeather()
        {
            //reset error prompts
            myOutput.Text = "";

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
                    
                    //check if text entry is empty or not
                    if(String.IsNullOrEmpty(citySearch))
                    {
                        //use gps
                        firstURL = String.Format("http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid=c16bcf9b4251e961d8106438b0711041", lat, lon);
                    }
                    else
                    {
                        //use search term
                        firstURL = String.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&appid=c16bcf9b4251e961d8106438b0711041", citySearch);
                    }

                    // Http request
                    HttpClient current = new HttpClient();

                    //JSON call to open weather api (Current Weather)
                    var responseCurrent = await current.GetStringAsync(firstURL);              

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
                        
                        if(isMetric == 1)
                        {
                            //if in metric                           
                            btmLeft.Text = Math.Round(cTemp, 1).ToString() + "°C";
                            btmRight.Text = Math.Round(speedKph, 0).ToString() + " Km/h";

                        }
                        else
                        {
                            //if in imperial
                            btmLeft.Text = Math.Round(fTemp, 1).ToString() + "°F";
                            btmRight.Text = Math.Round(speedMph, 0).ToString() + " Mp/h";
                        }
                                              
                        btmMid.Text = weatherType.ToString();

                    }// if responseCurrent
                    else
                    {
                        //JSON empty
                        myOutput.Text = "Unable to Find Weather";
                    }
                    
                    //check if entry is blank
                    if (String.IsNullOrEmpty(citySearch))
                    {
                        //use gps
                        secondURL = string.Format("http://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&appid=c16bcf9b4251e961d8106438b0711041", lat, lon);
                    }
                    else
                    {
                        //use search term
                        secondURL = string.Format("http://api.openweathermap.org/data/2.5/forecast?q={0}&appid=c16bcf9b4251e961d8106438b0711041", citySearch);
                    }

                    // Http request
                    HttpClient forecast = new HttpClient();

                    //JSON call to open weather api (5 day Weather Forecast)
                    var responseForecast = await forecast.GetStringAsync(secondURL);

                    //test if responseForecast is not empty
                    if (responseForecast != null)
                    {
                        //JSON object
                        var localForecast = JObject.Parse(responseForecast);

                        //make label & image object
                        Label label;
                        Image image;

                        //make loop for 5 day forecast - NB: loop in increments of 8 because data is every 3 hours (3 * 8 = 24hrs)
                        for(int i = 0; i < 40; i += 8)
                        {
                            //variables created from the api data
                            var unixTime = localForecast["list"][i]["dt"];
                            var theDescription = localForecast["list"][i]["weather"][0]["main"];
                            kTempArr[i] = (float)localForecast["list"][i]["main"]["temp_max"];

                            //FindDate class methods
                            epoch = (int)unixTime;
                            date = myUnix.unixToString(epoch);
                            day = myUnix.dateToDay(date);

                            //calculation class methods                       
                            cTempHigh = myCalc.toCelsius(kTempArr[i]);                            
                            fTempHigh = myCalc.toFahrenheit(kTempArr[i]);                            

                            //ImageGenerator class method - ICONS (forecast)
                            string strFilename = myImg.getIconUrl((string)theDescription);
                            var assembly = typeof(MainPage);
                           
                            //top row - Day
                            if ((label = (Label)FindByName("top_" + (i + 1).ToString())) != null)
                            {
                                label.Text = day.ToString();
                            }

                            //mid row - Icon
                            if ((image = (Image)FindByName("mid_" + (i + 1).ToString())) != null)
                            {
                                image.Source = ImageSource.FromResource(strFilename, assembly);
                            }

                            //bottom row - Temperature
                            if ((label = (Label)FindByName("btm_" + (i + 1).ToString())) != null)
                            {
                                if (isMetric == 1)
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
                        myOutput.Text = "Unable to Find Weather";
                    }
                }//if GPS location is found
                else
                {
                    // GPS location not found
                    myOutput.Text = "ERROR - GPS NULL";
                }
            }
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
                myOutput.Text = "Not Supported on Device: ";
            }
            catch (PermissionException)
            {
                // Handle permission exception
                myOutput.Text = "Enable Permission Access: ";
            }
            catch (Exception)
            {
                // Unable to get location
                myOutput.Text = "Unable to Find Location: " ;
            }
          
        }//getWeather

        //change metric/imperial when clicked
        private void toggleBtn_Clicked(object sender, EventArgs e)
        {
            //check if button has been pressed before
            if (isMetric == 1)
            {
                //change button
                toggleBtn.BackgroundColor = Color.FromRgb(255, 165, 0);
                toggleBtn.Text = "Imperial";

                //change temp/wind speed
                btmLeft.Text = Math.Round(fTemp, 1).ToString() + "°F";
                btmRight.Text = Math.Round(speedMph, 0).ToString() + " Mp/h";

                //change 5 day forecast
                for (int i = 0; i < 40; i += 8)
                {
                    //label object
                    Label label;

                    //calculation class methods                       
                    fTempHigh = myCalc.toFahrenheit(kTempArr[i]);

                    //bottom row - Temperature
                    if ((label = (Label)FindByName("btm_" + (i + 1).ToString())) != null)
                    {
                        label.Text = Math.Round(fTempHigh, 1).ToString() + "°F";
                    }
                }

                //change value
                isMetric = 0;
            }
            else
            {
                //change button
                toggleBtn.BackgroundColor = Color.FromRgb(0, 8, 255);
                toggleBtn.Text = "Metric";

                //change current temp/windspeed
                btmLeft.Text = Math.Round(cTemp, 1).ToString() + "°C";
                btmRight.Text = Math.Round(speedKph, 0).ToString() + " Km/h";

                //change 5 day forecast
                for (int i = 0; i < 40; i += 8)
                {
                    //label object
                    Label label;

                    //calculation class methods                       
                    cTempHigh = myCalc.toCelsius(kTempArr[i]);

                    //bottom row - Temperature
                    if ((label = (Label)FindByName("btm_" + (i + 1).ToString())) != null)
                    {
                        label.Text = Math.Round(cTempHigh, 1).ToString() + "°C";
                    }
                }

                //change value
                isMetric = 1;
            }

            //call savefile method
            SaveFile();
           
        }//toggleBtn

        //save settings to file
        private void SaveFile()
        {         
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // "MyWeatherData.txt"
            string fileName = Path.Combine(path, OUTPUT_FILE);
            // 1 or 0 (1 = true, 0 = false)
            string outputString = isMetric.ToString();
            //output to file
            File.WriteAllText(fileName, outputString);

        }//saveFile

        // when "Search City" has pressed ENTER
        public void InputCity_Completed(object sender, EventArgs e)
        {
            //read input from search box
            citySearch = inputCity.Text;
            //reload getWeather
            GetWeather();

        }//inputCity

    }//mainpage
}//namespace
