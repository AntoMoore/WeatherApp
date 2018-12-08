using System;
using System.Collections.Generic;
using System.Text;

namespace Solstice
{
    class ImageGenerator
    {
        //member variables
        float temperature;
        float windDirection;
        string weather;
        string backgroundUrl;
        string iconUrl;
        string compassUrl;

        //constructor
        public ImageGenerator()
        {
            this.temperature = 0;
            this.weather = "";
        }

        //======== member methods ==============

        //IconUrls
        public string getIconUrl(string description)
        {
            this.weather = description;
            //assign icon url
            if (this.weather == "Clear")
            {
                iconUrl = "Solstice.Assets.Images.iconClear.png";
            }
            else if (this.weather == "Clouds")
            {
                iconUrl = "Solstice.Assets.Images.iconCloud.png";
            }
            else if (this.weather == "Rain")
            {
                iconUrl = "Solstice.Assets.Images.iconRain.png";
            }
            else
            {
                iconUrl = "Solstice.Assets.Images.iconMixed.png";
            }
            return iconUrl;
        }

        public string getCompassUrl(float direction)
        {
            this.windDirection = direction;
            //assign compass icon
            if (this.windDirection > 337.5)
            {
                compassUrl = "Solstice.Assets.Images.north.png";
            }
            else if (this.windDirection > 292.5)
            {
                compassUrl = "Solstice.Assets.Images.nwest.png";
            }
            else if (this.windDirection > 247.5)
            {
                compassUrl = "Solstice.Assets.Images.west.png";
            }
            else if (this.windDirection > 202.5)
            {
                compassUrl = "Solstice.Assets.Images.swest.png";
            }
            else if (this.windDirection > 157.5)
            {
                compassUrl = "Solstice.Assets.Images.south.png";
            }
            else if (this.windDirection > 112.5)
            {
                compassUrl = "Solstice.Assets.Images.seast.png";
            }
            else if (this.windDirection > 67.5)
            {
                compassUrl = "Solstice.Assets.Images.east.png";
            }
            else if (this.windDirection > 22.5)
            {
                compassUrl = "Solstice.Assets.Images.neast.png";
            }
            else
            {
                compassUrl = "Solstice.Assets.Images.north.png";
            }

            return compassUrl;
        }

        //backgroundUrls
        public string getBackgroundUrl(float temp, string description)
        {
            this.weather = description;
            this.temperature = temp;
            //assign background url
            if(this.temperature > 30)
            {
                backgroundUrl = "Solstice.Assets.Images.backHot.png";

            }
            else if (this.temperature > 25)
            {
                backgroundUrl = "Solstice.Assets.Images.backSummer.png";

            }
            else if (this.temperature > 20)
            {
                backgroundUrl = "Solstice.Assets.Images.backWarm.png";

            }
            else if (this.temperature > 15)
            {
                backgroundUrl = "Solstice.Assets.Images.backMild.png";

            }
            else if (this.temperature > 10)
            {
                if(this.weather == "Rain")
                {
                    backgroundUrl = "Solstice.Assets.Images.backCoolRain.png";
                }
                else
                {
                    backgroundUrl = "Solstice.Assets.Images.backCool.png";
                }

            }
            else if (this.temperature > 5)
            {
                if(this.weather == "Rain")
                {
                    backgroundUrl = "Solstice.Assets.Images.backColdRain.png";

                }
                else
                {                    
                    backgroundUrl = "Solstice.Assets.Images.backCold.png";
                }

            }
            else if (this.temperature > 0)
            {
                backgroundUrl = "Solstice.Assets.Images.backVeryCold.png";

            }
            else
            {
                backgroundUrl = "Solstice.Assets.Images.backWinter.png";
            }

            return backgroundUrl;
        }
        
    }//class
}// namespace
