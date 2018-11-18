using System;
using System.Collections.Generic;
using System.Text;

namespace Solstice
{
    public class Calculations
    {
        //member variables
        float tempKelvin;
        float tempCelsius;
        float tempFahrenheit;

        float windSpeedMs;
        float windSpeedKmph;
        float windSpeedMph;

        //constructor
        public Calculations()
        {
            this.tempKelvin = 0;           
            this.windSpeedMs = 0;
        }
                 
        //===== member methods =========
        public float toCelsius(float input)
        {
            this.tempKelvin = input;
            this.tempCelsius = tempKelvin - 273;
            return tempCelsius;
        }

        public float toFahrenheit(float input)
        {
            this.tempKelvin = input;
            this.tempFahrenheit = (float)(tempKelvin * (9 / 5) - 459.67);
            return tempFahrenheit;
        }

        public float toKilometers(float input)
        {
            this.windSpeedMs = input;
            windSpeedKmph = (float)(windSpeedMs * 3.6);
            return windSpeedKmph;
        }

        public float toMiles(float input)
        {
            this.windSpeedMs = input;
            windSpeedMph = (float)(windSpeedMs * 2.2369);
            return windSpeedMph;
        }
    }
}
