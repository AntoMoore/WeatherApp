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
        }

        private void SetupImagesOnPage()
        {
            // get the assembly and file location
            var assembly = typeof(MainPage);
            string strFilename = "Solstice.Assets.Images.mysunflower.png";
            //output background image
            imageBackground.Source = ImageSource.FromResource(strFilename, assembly);
        }
    }
}
