using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace nexus.common

{
    static public partial class helpers
    {

        public static bool IsDate(string Date)
        {
            DateTime dout;
            return (DateTime.TryParse(Date, out dout));
        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }
        public static int GetWeekNumber(DateTime date)
        {
            // Get the current culture's calendar
            Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            // Specify the calendar week rule and the first day of the week
            CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek; // or use FirstDay, etc.
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday; // Adjust as necessary
            // Get the week of the year
            return calendar.GetWeekOfYear(date, rule, firstDayOfWeek);
        }

        public static DateTime GetNextSunday(DateTime date)
        {
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)date.DayOfWeek + 7) % 7;
            if (daysUntilSunday == 0) // If today is Sunday, return today
            {
                return date;
            }
            return date.AddDays(daysUntilSunday);
        }

        public static string FormatDateString(string Date, string Format)
        {
            string rval = string.Empty;
            DateTime cdt;

            if (DateTime.TryParse(Date, out cdt)) rval = cdt.ToString(Format);

            return rval;
        }

        public static string CalculateAge(DateTime Dob)
        {
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
            DateTime PastYearDate = Dob.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;

            //int Hours   = Now.Subtract(PastYearDate).Hours;
            //int Minutes = Now.Subtract(PastYearDate).Minutes;
            //int Seconds = Now.Subtract(PastYearDate).Seconds;
            //return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",Years, Months, Days, Hours, Seconds);

            return String.Format("{0} Year(s) {1} Month(s) {2} Day(s) ", Years, Months, Days);

        }
        private static DateTime TimeRoundUp(DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0).AddMinutes(input.Minute % 15 == 0 ? 0 : 15 - input.Minute % 15);
        }
        public static string msToSecs(int Millisecs)
        {
            string ret = "0:00";

            if (Millisecs > 0)
            {
                int Secs = 0;
                int TotalSecs = Millisecs;
                int Mins = 0;

                System.Math.DivRem(Mins, 60, out Mins);

                if (Mins > 60)
                {
                    int Hours = System.Math.DivRem(Mins, 60, out Mins);
                    ret = Hours.ToString("0") + ":" + Mins.ToString("00") + Secs.ToString("00");
                }
                else
                {
                    ret = Mins.ToString("00") + Secs.ToString("00");
                }

            }

            return ret;
        }
        public static string FormatTimeSpan(TimeSpan span)
        {
            string ret = String.Empty;

            if (span.Days > 0)
            {
                ret = span.Days.ToString("0") + "d";
                span = span.Subtract(new TimeSpan(span.Days, 0, 0, 0));
            }
            if (span.Hours > 0)
            {
                ret += (String.IsNullOrEmpty(ret) ? "" : " ") + span.Hours.ToString("00");
                span = span.Subtract(new TimeSpan(0, span.Hours, 0, 0));
            }
            if (span.Minutes > 0)
            {
                ret += (String.IsNullOrEmpty(ret) ? "" : ":") + span.Minutes.ToString("00");
                span = span.Subtract(new TimeSpan(0, 0, span.Minutes, 0));
            }
            else
            {
                ret += (String.IsNullOrEmpty(ret) ? "" : ":") + "00";
            }
            ret += ":" + span.Seconds.ToString("00");

            return ret;
        }
    }

}
