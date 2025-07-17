using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace nexus.common

{
    static public partial class helpers
    {
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
        public static int ToInt(string Number)
        {
            try { return Convert.ToInt32(Number); } catch (Exception) { }

            try { return Convert.ToInt32(Convert.ToDouble(Number)); } catch (Exception) { }

            return 0;
        }
        public static decimal ToDecimal(string Number)
        {
            try { return Convert.ToDecimal(Number); } catch (Exception) { }
            return 0;
        }

        public static DateTime ToDate(string Date)
        {
            DateTime dout;
            DateTime.TryParse(Date, out dout);
            return dout;
        }
        public static bool ToBool(string Number)
        {
            bool ret;
            bool.TryParse(Number, out ret);
            return ret;
        }
        public static Double ToDbl(string Number)
        {
            Double ret;
            Double.TryParse(Number, out ret);
            return ret;

        }
        public static Double LongToCurrency(long Number)
        {
            Double ret;
            Double.TryParse(Number.ToString(), out ret);
            return ret / 100;
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
                        return helpers.ToDbl(Number).ToString();
                    else
                        return Convert.ToInt32(Number).ToString();
                }
                catch (Exception) { return "0"; }
            }
        }

        public static string TitleCase(string Text)
        {
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo TextInfo = cultureInfo.TextInfo;
            return TextInfo.ToTitleCase(Text.ToLower());
        }

    }

}
