
using System;

namespace nexus.common.helper
{
    public static class Currency
    {
        static readonly String[] Digits  = {"Zero", "One",    "Two",    "Three",    "Four",     "Five",    "Six",     "Seven",     "Eight",    "Nine"};
        static readonly String[] Teens   = {"Ten",  "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"};
        static readonly String[] Decades = {"Zero", "Ten",    "Twenty", "Thirty",   "Forty",    "Fifty",   "Sixty",   "Seventy",   "Eighty",   "Ninety"};

        /// <summary>
        /// Converts currency value into a word string
        /// </summary>
        /// <param name="val">The currency value to convert</param>
        /// <returns>The string detailing the value in words</returns>
        public static String   Words( Int32 val )
        {
            string   txt = "";
            string[] blk;
            bool     tag = false;

            blk = WordBlocks( val );

            if ( (blk[0] != "Zero") )
            {
                txt += blk[0] + " hundred ";        // Hundred thousands
                tag = true;
            }

            if ( tag || (blk[1] != "Zero") )
            {
                if ( tag && (blk[1] != "Zero") ) txt += " and ";

                if (blk[1] != "Zero") txt += blk[1];
                txt += " thousand ";                // Thousands
                tag = true;
            }

            if ( (blk[2] != "Zero") )
            {
                txt += blk[2] + " hundred ";        // Hundreds
                tag = true;
            }

            if ( tag || (blk[3] != "Zero") )
            {
                if ( tag && (blk[3] != "Zero") ) txt += " and ";
                
                if (blk[3] != "Zero") txt += blk[3];
                txt += " dollar";                   // Unit

                if ( tag || (blk[3] != "One") )
                {
                    txt += "s";                     // Dollars
                }
                txt += " ";
                tag = true;
            }
            else
            {
                txt += " Zero dollars";
                tag = true;
            }

            if ( (blk[4] != "Zero") )
            {
                if ( tag && (blk[4] != "Zero") ) txt += " and ";

                if (blk[4] != "Zero") txt += blk[4];
                txt += " cent";                     // Unit

                if ( (blk[4] != "One") )
                {
                    txt += "s";                    // Cents
                }
                tag = true;
            }
            else
            {
                txt += " only";
            }

            txt = txt.Replace("  ", " ");
            txt = txt.Trim();

            if ( !tag ) txt += "nothing";

            return ( clean(txt) );
        }

        /// <summary>
        /// Converts currency value into a word blocks for hundthou, thou, hund, dolars and cents
        /// </summary>
        /// <param name="val">The currency value to convert</param>
        /// <returns>The string[] listing the block word values</returns>
        public static String[] WordBlocks( Int32 val )
        {
            string[] words;
            int      doll, cent;

//          if ( depth > 4 ) throw new ArgumentException( "Can not process currency in excess of $999,999.99 - set digit depth to 0 thru 4" );
//          if ( depth < 0 ) throw new ArgumentException( "Can not support a negative digit depth" );

            doll = val / 100;
            cent = val % 100;

            if ( doll > 999999 ) throw new ArgumentOutOfRangeException( "Currancy value is in excess of $999,999.99 - can not convert amounts grreater than this" );

            words = new string[5];

            words[0] = units( doll / 100000 );
            doll = doll % 100000;

            words[1] = units( doll / 1000 );
            doll = doll % 1000;

            words[2] = units( doll / 100 );
            doll = doll % 100;

            words[3] = units( doll );

            words[4] = units( cent );

            return ( words );
        }

        /// <summary>
        /// Converts currency into digit words for the dollars and a word block for cents
        /// </summary>
        /// <param name="val">The currency value to convert</param>
        /// <param name="digit">The number of digits (plus cents) to convert</param>
        /// <returns>The string[] listing the digit words</returns>
        public static String[] WordDigits(int val, int digit )
        {
            string[] txt;
            string   str;
            int      doll, cent;
            int      i;

            if ( digit > 6 ) throw new ArgumentException( "Can not process currency in excess of $999,999.99 - set digit count to 0 thru 6" );
            if ( digit < 0 ) throw new ArgumentException( "Can not support a negative digit count" );

            doll = val / 100;
            cent = val % 100;

            if ( doll >= 1000000 )         throw new ArgumentOutOfRangeException( "Currency value is in excess of $999,999.99 - can not convert amounts grreater than this" );
            //if ( doll >= Math.P10(digit) ) throw new ArgumentOutOfRangeException( "Currency value is greater than can be processed into the specified number of digits" );

            txt = new string[digit+1];

            str = String.Format( "{0:D}", doll ).PadLeft(digit,'0');

            for ( i=0; i<digit; i++ )
            {
                txt[i] = Digits[str[i]-'0'];    // Each dollar digit column tabs to next (or finally to cents)
            }

            txt[i] = units( cent );

            return ( txt );
        }

        private static string units ( int val )
        {
            string txt = "";

            if ( val >= 20 )
            {
                txt += Decades[val/10];
                if ((val % 10) != 0) txt += " " + Digits [val%10];
            }
            else
            {
                if (val >= 10)       txt += Teens [val-10];
                                else txt += Digits[val];
            }

            return ( txt );
        }
        private static string clean ( string str )
        {
            string[] x;
            string   y = "";

            x = str.Split( new char[1]{' '}, StringSplitOptions.RemoveEmptyEntries );
            
            foreach( string s in x ) y += s + " ";

            return ( y.Substring(0, y.Length-1) );
        }
    }
}
