using System;
using System.Collections.Generic;
using System.Text;

namespace Solstice
{
    class FindDate
    {
        public string unixToString(int unix)
        {
            //add seconds to time since 1970
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix).ToShortDateString();
        }

        public string dateToDay(string date)
        {
            //variables
            int day, month, year;
            string todayIs = "";

            //get values from input
            var dayStr = date.Substring(0, date.IndexOf('/'));
            var monthStr = date.Split('/')[1].Split('/')[0];
            var yearStr = date.Substring(date.LastIndexOf('/') + 1);

            day = Int32.Parse(dayStr);
            month = Int32.Parse(monthStr);
            year = Int32.Parse(yearStr);

            //get date value
            DateTime dateValue = new DateTime(year, month, day);
            
            //find day of week
            if((int)dateValue.DayOfWeek % 7 == 1)
            {
                todayIs = "Sun";
            }
            else if ((int)dateValue.DayOfWeek % 7 == 2)
            {
                todayIs = "Mon";
            }
            else if ((int)dateValue.DayOfWeek % 7 == 3)
            {
                todayIs = "Tue";
            }
            else if ((int)dateValue.DayOfWeek % 7 == 4)
            {
                todayIs = "Wed";
            }
            else if ((int)dateValue.DayOfWeek % 7 == 5)
            {
                todayIs = "Thur";
            }
            else if ((int)dateValue.DayOfWeek % 7 == 6)
            {
                todayIs = "Fri";
            }
            else if ((int)dateValue.DayOfWeek % 7 == 0)
            {
                todayIs = "Sat";
            }

            return todayIs;

        }
    }
}
