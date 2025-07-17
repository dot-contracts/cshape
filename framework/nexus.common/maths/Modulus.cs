
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nexus.common.helper
{
    public static partial class Math
    {
        static readonly  int[] p10 = new  int[10] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        public static Int32 P10( Int32 idx )
        {
            if ( (idx < 0) || (idx > 9) ) throw new ArgumentException( "Power index must have a value of 0 thru 9" );

            return ( p10[idx] );
        }

        public static Int64 P10( Int64 idx )
        {
            Int64 val = p10[idx%10];

            if ( (idx < 0) || (idx > 18) ) throw new ArgumentException( "Power index must have a value of 0 thru 18" );

            return ( idx < 10 ? val : val*10000000000 );
        }

        [Obsolete]
        public static Int32 PosModulus( Int32 dividend, Int32 modulus )
        {
            return ( Wrap(dividend,modulus) );
        }
        public static Int32 Wrap( Int32 dividend, Int32 modulus )
        {
            Int32 tmp = dividend % modulus;
            if ( tmp < 0 ) tmp += modulus;

            return ( tmp );
        }

        [Obsolete]
        public static Int64 PosModulus( Int64 dividend, Int64 modulus )
        {
            return ( Wrap(dividend,modulus) );
        }
        public static Int64 Wrap( Int64 dividend, Int64 modulus )
        {
            Int64 tmp = dividend % modulus;
            if ( tmp < 0 ) tmp += modulus;

            return ( tmp );
        }

        public enum RoundMode
        {
            Ceiling,    // Round up   (away from zero)
            Floor,      // Round down (toward zero)
            Bisect      // Round by splitting difference [even modulii by default roundUp bisector]
        }

        public static Int32 Round( Int32 dividend, Int32 modulus, out Int32 rem, RoundMode mode = RoundMode.Bisect, bool midZero=false )
        {
            Int32 bal;
            Int32 mid = modulus/2;
            bool  neg = dividend < 0;

            bal = 0;
            rem = 0;

            // Check for a valid modulus
            if ( modulus <= 1 ) return ( dividend );

            // Want to do everything symetrically wrt to zero
            if ( neg ) dividend *= -1;

            // Compute the rounding residue
            rem = (dividend % modulus);
            bal = dividend - rem;

            switch ( mode )
            {
                case RoundMode.Ceiling    : // Round up
                                            if ( rem > 0 )
                                            {
                                                rem = modulus  - rem;
                                                bal = dividend + rem;
                                                rem *= -1;
                                            }
                                            break;

                case RoundMode.Floor      : // Round down
                                            break;

                case RoundMode.Bisect     : // Round up/down from mid
                                            bal = dividend - rem;

                                            // Fix for round up situations
                                            if ( mid < rem )
                                            {
                                                bal += modulus;
                                                rem -= modulus;
                                            }
                                            
                                            // Fix for even bisector toward zero
                                            if ( midZero && (modulus%2==0) && (rem==-mid) )
                                            {
                                                bal -= modulus;
                                                rem += modulus;
                                            }
                                            break;
            }
/*
            // Perform a round down!
            rem = (dividend % modulus);
            bal = dividend - rem;

            if (rem != 0)
            {
                // Fix for round up situations
                if ( modulus/2 < rem )
                {
                    bal += modulus;
                    rem -= modulus;
                }
            }
*/
            // Restore original sign
            if ( neg )
            {
                bal *= -1;
                rem *= -1;
            }

            return ( bal );
        }
    }
}
