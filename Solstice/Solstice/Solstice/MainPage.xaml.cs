using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using System.IO;

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
        public async void GetGPS()
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
                    //variables for gps
                    lat = location.Latitude;
                    lon = location.Longitude;

                    //concat the weather api string with the 'lat' and 'long' variables
                    var request = HttpWebRequest.Create(string.Format(@"http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid=3ce66444abe0bb9e6dec0fbfecbd27be", lat, lon));
                    request.ContentType = "application/json";
                    request.Method = "GET";

                    //JSON call to open weather api
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            //failed to get response
                            myOutput.Text = "Error fetching data. Server returned status code: {0}";
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            //read in response
                            var content = reader.ReadToEnd();
                            if (string.IsNullOrWhiteSpace(content))
                            {
                                //content empty
                                myOutput.Text = "Response: contained empty body...";
                            }
                            else
                            {
                                //sucessful call
                                var answer = string.Format("Response for: {0} and {1}", Math.Round(lat,2), Math.Round(lon,2));
                                myOutput.Text = answer;

                                

                            }

                        }//streamReader
                    }//using
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
    }
}
