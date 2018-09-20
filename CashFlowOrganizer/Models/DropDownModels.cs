using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Models
{
    public class DropDownModels
    {
        public Dictionary<string, string> MonthDDL
        {
            get
            {
                Dictionary<string, string> months = new Dictionary<string, string>();

                months.Add("01", "01 - January");
                months.Add("02", "02 - February");
                months.Add("03", "03 - March");
                months.Add("04", "04 - April");
                months.Add("05", "05 - May");
                months.Add("06", "06 - June");
                months.Add("07", "07 - July");
                months.Add("08", "08 - August");
                months.Add("09", "09 - September");
                months.Add("10", "10 - October");
                months.Add("11", "11 - November");
                months.Add("12", "12 - December");

                return months;
            }
        }

        public Dictionary<string, string> YearDDL
        {
            get
            {
                Dictionary<string, string> years = new Dictionary<string, string>();

                int currentYear = DateTime.Now.Year;

                years.Add(currentYear.ToString(), currentYear.ToString());
                years.Add((currentYear + 1).ToString(), (currentYear + 1).ToString());
                years.Add((currentYear + 2).ToString(), (currentYear + 2).ToString());
                years.Add((currentYear + 3).ToString(), (currentYear + 3).ToString());
                years.Add((currentYear + 4).ToString(), (currentYear + 4).ToString());
                years.Add((currentYear + 5).ToString(), (currentYear + 5).ToString());
                years.Add((currentYear + 6).ToString(), (currentYear + 6).ToString());
                years.Add((currentYear + 7).ToString(), (currentYear + 7).ToString());
                years.Add((currentYear + 8).ToString(), (currentYear + 8).ToString());
                years.Add((currentYear + 9).ToString(), (currentYear + 9).ToString());
                years.Add((currentYear + 10).ToString(), (currentYear + 10).ToString());
                years.Add((currentYear + 11).ToString(), (currentYear + 11).ToString());

                return years;
            }
        }

        public Dictionary<string, string> StateDDL
        {
            get
            {
                Dictionary<string, string> states = new Dictionary<string, string>();

                states.Add("Alabama", "Alabama");
                states.Add("Alaska", "Alaska");
                states.Add("Arizona", "Arizona");
                states.Add("Arkansas", "Arkansas");
                states.Add("California", "California");
                states.Add("Colorado", "Colorado");
                states.Add("Connecticut", "Connecticut");
                states.Add("Delaware", "Delaware");
                states.Add("Florida", "Florida");
                states.Add("Georgia", "Georgia");
                states.Add("Hawaii", "Hawaii");
                states.Add("Idaho", "Idaho");
                states.Add("Illinois", "Illinois");
                states.Add("Indiana", "Indiana");
                states.Add("Iowa", "Iowa");
                states.Add("Kansas", "Kansas");
                states.Add("Kentucky", "Kentucky");
                states.Add("Louisiana", "Louisiana");
                states.Add("Maine", "Maine");
                states.Add("Maryland", "Maryland");
                states.Add("Massachusetts", "Massachusetts");
                states.Add("Michigan", "Michigan");
                states.Add("Minnesota", "Minnesota");
                states.Add("Mississippi", "Mississippi");
                states.Add("Missouri", "Missouri");
                states.Add("Montana", "Montana");
                states.Add("Nebraska", "Nebraska");
                states.Add("Nevada", "Nevada");
                states.Add("New Hampshire", "New Hampshire");
                states.Add("New Jersey", "New Jersey");
                states.Add("New Mexico", "New Mexico");
                states.Add("New York", "New York");
                states.Add("North Carolina", "North Carolina");
                states.Add("North Dakota", "North Dakota");
                states.Add("Ohio", "Ohio");
                states.Add("Oklahoma", "Oklahoma");
                states.Add("Oregon", "Oregon");
                states.Add("Pennsylvania", "Pennsylvania");
                states.Add("Rhode Island", "Rhode Island");
                states.Add("South Carolina", "South Carolina");
                states.Add("South Dakota", "South Dakota");
                states.Add("Tennessee", "Tennessee");
                states.Add("Texas", "Texas");
                states.Add("Utah", "Utah");
                states.Add("Vermont", "Vermont");
                states.Add("Virginia", "Virginia");
                states.Add("Washington", "Washington");
                states.Add("West Virginia", "West Virginia");
                states.Add("Wisconsin", "Wisconsin");
                states.Add("Wyoming", "Wyoming");

                return states;
            }
        }

        public Dictionary<string, string> DayOfMonthDDL
        {
            get
            {
                Dictionary<string, string> days = new Dictionary<string, string>();

                days.Add("1", "1st");
                days.Add("2", "2nd");
                days.Add("3", "3rd");
                days.Add("4", "4th");
                days.Add("5", "5th");
                days.Add("6", "6th");
                days.Add("7", "7th");
                days.Add("8", "8th");
                days.Add("9", "9th");
                days.Add("10", "10th");
                days.Add("11", "11th");
                days.Add("12", "12th");
                days.Add("13", "13th");
                days.Add("14", "14th");
                days.Add("15", "15th");
                days.Add("16", "16th");
                days.Add("17", "17th");
                days.Add("18", "18th");
                days.Add("19", "19th");
                days.Add("20", "20th");
                days.Add("21", "21st");
                days.Add("22", "22nd");
                days.Add("23", "23rd");
                days.Add("24", "24th");
                days.Add("25", "25th");
                days.Add("26", "26th");
                days.Add("27", "27th");
                days.Add("28", "28th");
                days.Add("29", "29th");
                days.Add("30", "30th");
                days.Add("31", "31st");

                return days;
            }
        }

        public Dictionary<string, string> DayOfTwoWeeksDDL
        {
            get
            {
                Dictionary<string, string> days = new Dictionary<string, string>();

                var today = DateTime.Now;

                string code;

                var cal = new GregorianCalendar();

                if (cal.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) % 2 == 0) { code = "EVE"; } else { code = "ODD"; }
                days.Add(code + today.DayOfWeek.ToString(), "Today");

                for (int i = 1; i < 14; i++)
                {
                    if (cal.GetWeekOfYear(today.AddDays(i), CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) % 2 == 0) { code = "EVE"; } else { code = "ODD"; }
                    days.Add(code + today.AddDays(i).DayOfWeek.ToString(), today.AddDays(i).ToString("dddd (M/dd)"));
                }

                return days;
            }
        }

        public Dictionary<string, string> DayOfWeekDDL
        {
            get
            {
                Dictionary<string, string> days = new Dictionary<string, string>();                

                days.Add("1", "Sunday");
                days.Add("2", "Monday");
                days.Add("3", "Tuesday");
                days.Add("4", "Wednesday");
                days.Add("5", "Thursday");
                days.Add("6", "Friday");
                days.Add("7", "Saturday");                

                return days;
            }
        }

        public Dictionary<string, string> FinanceTypeDDL
        {
            get
            {
                Dictionary<string, string> types = new Dictionary<string, string>();

                types.Add("MON", "Monthly");
                types.Add("SEM", "Semi-Monthly");
                types.Add("BWE", "Bi-Weekly");
                types.Add("WEE", "Weekly");
                types.Add("DAY", "Daily");

                return types;
            }
        }

        public Dictionary<string, string> AccountTypeDDL
        {
            get
            {
                Dictionary<string, string> types = new Dictionary<string, string>();

                types.Add("POS", "Positvie");
                types.Add("NEG", "Negative");

                return types;
            }
        }

        public Dictionary<int, string> CashFlowMonthsDDL
        {
            get
            {
                Dictionary<int, string> months = new Dictionary<int, string>();

                months.Add(1, "1 Month");
                months.Add(2, "2 Months");
                months.Add(3, "3 Months");
                months.Add(6, "6 Months");
                months.Add(9, "9 Months");
                months.Add(12, "12 Months");
                months.Add(18, "18 Months");
                months.Add(24, "24 Months");

                return months;
            }
        }


    }
}