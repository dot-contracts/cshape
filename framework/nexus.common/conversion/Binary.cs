
using System;
using System.Text;

namespace nexus.common
{
    public static class ConvertBcd
    {
        #region Public Methods
        public static byte[] GetBytes( String str )
        {
            int len = str.Length;

            if ( (len % 2) != 0 ) len++;

            return ( BcdBytes( str.PadLeft(len,'0'), Validate(len/2,10) ) );
        }

        public static byte[] GetBytes( String str, int len )
        {
            return ( BcdBytes( str.Trim().PadLeft(len*2,'0'), Validate(len,10) ) );
        }

        public static byte[] GetBytes( UInt64 val, int len=8 )
        {
            // Assuming len is number of pBcd bytes to generate (1,2,3,4,5,6,7,8,9,10)
            return ( BcdBytes( (UInt64)val, Validate(len,10) ) );
        }

        public static byte[] GetBytes( UInt32 val, int len=4 )
        {
            // Assuming len is number of pBcd bytes to generate (1,2,3,4,5)
            return ( BcdBytes( (UInt64)val, Validate(len,5) ) );
        }

        public static byte[] GetBytes( UInt16 val, int len=2 )
        {
            // Assuming len is number of pBcd bytes to generate (1,2,3)
            return ( BcdBytes( (UInt64)val, Validate(len,3) ) );
        }

        public static byte   GetByte ( byte val )
        {
            return ( (byte)(((val >> 4) * 10) + (val & 0x0f)) );
        }

        public static UInt64 ToUInt64( byte[] buf, int idx, int len )
        {
            // Assuming len is number of pBcd bytes for conversion (1,2,3,4,5,6,7,8,9,10)
            return ( (UInt64)ToBcd( buf, idx, Validate(len,10) ) );
        }

        public static UInt32 ToUInt32( byte[] buf, int idx, int len )
        {
            // Assuming len is number of pBcd bytes for conversion (1,2,3,4,5)
            return ( (UInt32)ToBcd( buf, idx, Validate(len,5) ) );
        }

        public static UInt16 ToUInt16( byte[] buf, int idx, int len )
        {
            // Assuming len is number of pBcd bytes for conversion (1,2,3)
            return ( (UInt16)ToBcd( buf, idx, Validate(len,3) ) );
        }

        public static byte   ToByte  ( byte val )
        {
            return ( (byte)(((val / 10) << 4) + (val % 10)) );
        }
        #endregion

        #region Private Methods
        private static byte[] BcdBytes( String str, int len )
        {
            UInt64 tmp = new UInt64();

            // Assuming len is number of pBcd bytes to generate (1-10)
            try 
            {
                tmp = Convert.ToUInt64(str);
            }
            catch ( Exception )
            {
                //throw new ConversionException("test", e);
            }

            return ( BcdBytes( tmp, len ) );
        }

        private static byte[] BcdBytes( UInt64 val, int len )
        {
            byte[] ba  = new byte[len];

            // Assuming len is number of pBcd bytes to generate (1-10)
            for ( int i=0; i<len; i++ )
            {
                ba[i] = ToByte( (byte)(val % 100) );
                val /= 100;
            }
            if ( val != 0 ) throw new OverflowException("Bcd value is greater than storage allocated");

            return ( ba );
        }
        
        private static UInt64 ToBcd   ( byte[] buf, int idx, int len )
        {
            UInt64 acc = 0;

            try
            {
                // Assuming len is number of pBcd bytes for conversion (1-10)
                for( int i=0; i<len; i++ )
                {
                    acc = acc*100 + GetByte( buf[idx+i] );
                }
            }
            catch
            {
                throw new Exception("Test");
            }

            return ( acc );
        }

        private static int    Validate( int len, int max )
        {
            if ( (len < 1) || (len > max) )
            {
                throw new Exception("test");
            }

            return ( len );
        }
        #endregion
    }

    public static class ConvertHex
    {
        #region HEX Methods
        public static string CnvBytesToHex( byte[] buf, int idx, int len )
        {
            //Byte.Parse(str,System.Globalization.NumberStyles.HexNumber)
            //??
            return ( BitConverter.ToString( buf, idx, len ) );
        }

        public static byte[] CnvHexToBytes( string val )
        {
            // Hex string is processed as nibbles, ignoring any additional chars
            //?? Chomp of 0x if present, then eat 2 chars at a time from right ([] be little endian!!)
            return ( null );
        }

        public static byte[] GetBytes( String str )
        {
            int len = str.Length;

            if ( (len % 2) != 0 ) len++;

            return ( HexBytes( str.PadRight(len,' '), Validate(len/2,10) ) );
        }

        public static byte[] GetBytes( String str, int len )
        {
            return ( HexBytes( str.Trim().PadLeft(len*2,'0'), Validate(len,10) ) );
        }

        public static byte   GetByte ( byte val )
        {
            return ( (byte)(((val >> 4) * 16) + (val & 0x0f)) );
        }

        public static string CnvByteToHex( byte val )
        {
            return ( BitConverter.ToString( new byte[] {val}, 0, 1 ) );
        }

        public static byte CnvHexToByte( string str )
        {
            // Hex string is processed as nibbles, ignoring any additional chars??
            return ( Byte.Parse( str, System.Globalization.NumberStyles.HexNumber ) );
        }

        public static byte   ToByte  ( byte val )
        {
            return ( (byte)(((val / 16) << 4) + (val % 16)) );
        }
        #endregion

        #region Private Methods
        private static byte[] HexBytes( String str, int len )
        {
            UInt64 tmp= new UInt64();

            // Assuming len is number of pBcd bytes to generate (1-10)
            try 
            {
                tmp = Convert.ToUInt64(str);
            }
            catch ( Exception )
            {
                //throw new ConversionException("test", e);
            }

            return ( HexBytes( tmp, len ) );
        }

        private static byte[] HexBytes( UInt64 val, int len )
        {
            byte[] ba  = new byte[len];

            // Assuming len is number of pBcd bytes to generate (1-8)
            for ( int i=0; i<len; i++ )
            {
                ba[i] = ToByte( (byte)(val % 256) );
                val /= 256;
            }

            return ( ba );
        }
        
        private static UInt64 ToHex   ( byte[] buf, int idx, int len )
        {
            UInt64 acc = 0;

            // Assuming len is number of pHex bytes for conversion (1-8)
            for( int i=0; i<len; i++ )
            {
                acc = acc*256 + GetByte( buf[idx+i] );
            }
            return ( acc );
        }

        private static int    Validate( int len, int max )
        {
            if ( (len < 1) || (len > max) )
            {
                throw new Exception("test");
            }

            return ( len );
        }
        #endregion
    }

    public static class ConvertStr
    {
        #region STR Methods
        /// <summary>
        /// Converts a Unicode string to a byte[] string image
        /// </summary>
        /// <param name="str">Unicode string of arbitrary length</param>
        /// <returns>Converted Byte[] without any cleanup</returns>
        public static Byte[] GetBytes( String str )
        {
            byte[] ba = new byte[str.Length];

            for ( int i=0; i<str.Length; i++ )
            {
                ba[i] = Convert.ToByte( str[i] );
            }

            return ( ba );
        }

        /// <summary>
        /// Converts a byte[] string image to a Unicode string
        /// </summary>
        /// <param name="fs">Byte[] of arbitrary length</param>
        /// <returns>Converted Unicode string without any cleanup</returns>
        public static String ToString( Byte[] fs )
        {
            StringBuilder str = new StringBuilder( fs.Length );

            for( int i=0; i<fs.Length; i++ )
            {
                if ( fs[i] == (byte)'\0' ) break;

                str.Append( Convert.ToChar( fs[i] ) );
            }

            return ( str.ToString() );
        }
        #endregion
    }

    public static class Reverse
    {
        #region Byte[] Methods
        public static void Copy( byte[] src, int sidx, byte[] dst, int didx, int len )
        {
            // Reorder src when copying to dst
            for ( int i=0; i<len; i-- ) dst[len-1+didx-i] = src[sidx+i];
        }
      
        public static Byte[] SwapBytes( byte[] src )
        {
            // Reorder src when copying to dst
            return ( SwapBytes( src, 0, src.Length ) );
        }

        public static Byte[] SwapBytes( byte[] src, int idx, int len )
        {
            Byte[] dst = new Byte[len];

            // Reorder src when copying to dst
            Copy( src, idx, dst, 0, len );

            return ( dst );
        }
        #endregion

        #region SWAP Methods
        public static UInt64 SwapUInt64( UInt64 val )
        {
            return ( (UInt64)SwapUInt32( (UInt32)(val >> 32) ) | ((UInt64)SwapUInt32( (UInt32)(val & 0xffffffff) ) << 32) );
        }

        public static Int64  SwapInt64 ( Int64  val )
        {
            return ( (Int64)SwapUInt64( (UInt64)val ) );
        }

        public static UInt32 SwapUInt32( UInt32 val )
        {
            return ( (UInt32)SwapUInt16( (UInt16)(val >> 16) ) | ((UInt32)SwapUInt16( (UInt16)(val & 0xffff) ) << 16) );
        }

        public static Int32  SwapInt32 ( Int32  val )
        {
            return ( (Int32)SwapUInt32( (UInt32)val ) );
        }

        public static UInt16 SwapUInt16( UInt16 val )
        {
            return ( (UInt16) ( (UInt16)(val >> 8) | (UInt16)((val & 0xff) << 8) ) );
        }

        public static Int16  SwapInt16 ( Int16  val )
        {
            return ( (Int16)SwapUInt16( (UInt16)val ) );
        }
        #endregion
    }
}
