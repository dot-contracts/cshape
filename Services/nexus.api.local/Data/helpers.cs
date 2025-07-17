using System;
using System.Globalization;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace nexus.common

{
    static public class helpers
    {
        public static bool IsDate(string Date)
        {
            DateTime dout;
            return (DateTime.TryParse(Date, out dout));
        }
        public static DateTime ToDate(string Date)
        {
            DateTime dout;
            DateTime.TryParse(Date, out dout);
            return dout;
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
            int Days    = Now.Subtract(PastYearDate.AddMonths(Months)).Days;

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


        public static string CurrencyToInteger(string Number)
        {
            double dout = 0;
            if (!Number.Equals(""))
                double.TryParse(Number, out dout);
            return CurrencyToInteger(dout);
        }
        public static string CurrencyToInteger(double Number)
        {
            Number = (Number * 100);
            return (Number).ToString("0");
        }
        public static int    ToInt(string Number)
        {
            int ret;
            int.TryParse(Number, out ret);
            return ret;
        }
        public static decimal ToDecimal(string Number)
        {
            decimal ret;
            decimal.TryParse(Number, out ret);
            return ret;
        }
        public static double ToDbl(string Number)
        {
            double ret;
            double.TryParse(Number, out ret);
            return ret;
        }

        public static bool IsGreaterThanZero(string Value)
        {
            int Int;
            int.TryParse(Value, out Int);
            return Int > 0;
        }
        public static string ValidateNumeric(string Number)
        {
            if (Number.Equals(""))
                return "0";
            else
            {
                try
                {
                    if (Number.Contains("."))
                        return Convert.ToDouble(Number).ToString();
                    else
                        return Convert.ToInt32(Number).ToString();
                }
                catch (Exception ex) { return "0"; }
            }
        }
        public static string TitleCase(string Text)
        {
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo TextInfo = cultureInfo.TextInfo;
            return TextInfo.ToTitleCase(Text.ToLower());
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
                ret = (String.IsNullOrEmpty(ret) ? "" : " ") + span.Hours.ToString("00");
                span = span.Subtract(new TimeSpan(0, span.Hours, 0, 0));
            }
            if (span.Minutes > 0)
            {
                ret = (String.IsNullOrEmpty(ret) ? "" : ":") + span.Minutes.ToString("00");
                span = span.Subtract(new TimeSpan(0, 0, span.Minutes, 0));
            }
            else
            {
                ret = (String.IsNullOrEmpty(ret) ? "" : ":") + "00";
            }
            ret = ":" + span.Seconds.ToString("00");

            return ret;
        }

        public static DataTable CreateDataTable(string source)
        {
            DataTable Data = new DataTable();
            Data.Columns.Add("Set", System.Type.GetType("System.Boolean"));
            Data.Columns.Add("Description", System.Type.GetType("System.String"));
            Data.Columns.Add("Id", System.Type.GetType("System.String"));
            Data.AcceptChanges();

            Array Arr = source.Split(new char[] { ';' });
            for (int i = 0; i <= Arr.GetLength(0) - 1; i++)
            {
                try
                {
                    string Name = Arr.GetValue(i).ToString();
                    Name = Name + "," + Name;
                    Array Art = Name.Split(new char[] { ',' });

                    DataRow newRow = Data.NewRow();
                    newRow["Set"] = false;
                    newRow["Description"] = Art.GetValue(0).ToString();
                    newRow["Id"] = Art.GetValue(1).ToString();
                    Data.Rows.Add(newRow);
                }
                catch (Exception) { }
            }

            return Data;
        }

        public static void WriteToLog(string Message)
        {
            if (!Directory.Exists(@"C:\Nexus\logging")) Directory.CreateDirectory(@"C:\Nexus\logging");
            string LogFile = "C:\\Nexus\\logging\\nexus_api_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            using (StreamWriter w = System.IO.File.AppendText(LogFile)) w.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "|| " + Message);
        }


    }

}
