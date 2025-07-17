using System;
using System.Text;
using System.Runtime.InteropServices;

namespace nexus.common
{
    static public class arrays
    {

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct CVT     {[FieldOffset(0)] public byte cval0; [FieldOffset(1)] public byte cval1; [FieldOffset(2)] public byte cval2; [FieldOffset(3)] public byte cval3; [FieldOffset(0)] public int ival; [FieldOffset(0)] public long lval; }
        public static CVT cvt = new CVT();

        public static StringBuilder str = new StringBuilder("");
        public static byte[] convrt = new byte[10];

        public static int AddLong(ref byte[] Buffer, int index, long value)
        {
            cvt.lval = value;
            Buffer[index++] = cvt.cval0;        // lsb
            Buffer[index++] = cvt.cval1;
            Buffer[index++] = cvt.cval2;
            Buffer[index++] = cvt.cval3;        // msb
            return index;
        }

        public static int AddInt(ref byte[] Buffer, int index, long value)
        {
            cvt.lval = value;
            Buffer[index++] = cvt.cval0;        // lsb
            Buffer[index++] = cvt.cval1;        // msb
            return index;
        }

        public static int AddDec(ref byte[] Buffer, int index, double value)
        {
            double d = (double)100.00;
            cvt.lval = (long)(value * d); 
            Buffer[index++] = cvt.cval0;        // lsb
            Buffer[index++] = cvt.cval1;
            Buffer[index++] = cvt.cval2;
            Buffer[index++] = cvt.cval3;        // msb
            return index;
        }

        public static int AddString(ref byte[] Buffer, int index, string s)
        {
            s = (string.IsNullOrEmpty(s) ? "" : s);
            if (s!=null)
            {
                try
                {
                    int strlen = s.Length;
                    char[] strcvt = s.ToCharArray(0, strlen);

                    Buffer[index++] = (byte)strlen;                   //store the length

                    for (int i = 0; i < strlen; i++)
                        Buffer[index++] = (byte)strcvt[i];
                }
                catch {  }
            }
            return index;
        }

        public static int GetLong(byte[] buffer, ref int index)
        {
            cvt.cval0 = buffer[index++];        // lsb
            cvt.cval1 = buffer[index++];
            cvt.cval2 = buffer[index++];
            cvt.cval3 = buffer[index++];        // msb

            return (int)arrays.cvt.lval;
        }

        public static int GetIntEx(byte[] buffer, ref int index)
        {
            cvt.cval0 = buffer[index++];        // lsb
            cvt.cval1 = buffer[index++];
            cvt.cval2 = 0;
            cvt.cval3 = 0;                      // msb

            return (int)arrays.cvt.lval;
        }

        public static int GetInt(byte[] buffer, ref int index)
        {
            cvt.cval0 = buffer[index++];        // lsb
            cvt.cval1 = buffer[index++];
            cvt.cval2 = 0;
            cvt.cval3 = 0;                      // msb

            return (int)arrays.cvt.lval;
        }

        public static double GetDec(byte[] buff, ref int index)
        {
            try
            {
                cvt.cval0 = buff[index++];       //lsb
                cvt.cval1 = buff[index++];
                cvt.cval2 = buff[index++];
                cvt.cval3 = buff[index++];       //msb
                double dbl = 100;
                return cvt.ival / dbl;
            }
            catch
            {
                //str = string.Format("BufftoInt  index exception {0}", index);
                //PutText(str);
                return (0);
            }
        }

        public static int GetString(byte[] packet, int index, ref string dest)
        {
            dest = "";
            StringBuilder str = new StringBuilder();
            int k = packet[index++];                   //string length - todo - number of strings
            if (k<packet.Length)
            {
                for (int j = 0; j < k; j++) str.Append((char)packet[index++]);
                dest = str.ToString();
            }
            return index;// + (k + 1);
        }




        public static void BCDtoBuff(int len, byte[] egmPKT, int index)
        {
            //convert up to 10 bcd chars to 5 bytes of hex 'cause 4 bytes  of 'long' isnt big enough
            int chr; int i; int DR0;

            for (i = 0; i < len; i++)
            {
                chr = egmPKT[index];                                //get both nibbles
                chr = chr >> 4;                                     //hi to lo
                DR0 = chr * 6;                                      //multiply it by 6
                chr = egmPKT[index++];                          //get original vlaue
                chr -= DR0;                                         //subtract magic number
                convrt[i] = (byte)chr;
            }
            for (; i < 10; i++) convrt[i] = 0;             //pad remainder with zeros
        }

        public static long HextoLong()
        {
            long hundreds;
            long tenthous;
            long millions;
            long hundrdmil;
            long calc;

            hundreds = convrt[1] * 100; tenthous = convrt[2] * 10000L;
            millions = convrt[3] * 1000000L; hundrdmil = convrt[4] * 100000000L;
            calc = convrt[0]; calc += hundreds;
            calc += tenthous; calc += millions;
            calc += hundrdmil; return (calc);
        }

        //public static int BuffToString(int index, ref byte[] packet, ref string dest)
        //{
        //    int j, k;

        //    str.Clear();
        //    k = packet[index++];                                               //string length - todo - number of strings
        //    for (j = 0; j < k; index++, j++)
        //        str.Append((char)packet[index]);
        //    dest = str.ToString();
        //    return (k + 1);
        //}


        //public static int ExtractVarString(int index, ref byte[] packet, ref string dest)
        //{
        //    int  j, l;
        //    char chr;

        //    str.Clear();
        //    l = (int)packet[index++];       //get string length
        //    for (j = 0; j < l; index++, j++)
        //    {
        //        chr = (char)packet[index];
        //        str.Append(chr);
        //    }
        //    dest = str.ToString();
        //    return (l);                 //length of this string
        //}

        //public static int ExtractString(int index, ref byte[] packet, ref string dest)
        //{
        //    int  j;
        //    char chr;

        //    str.Clear();
        //    for (j = 0; j < 15; index++, j++)
        //    {                         //long card number plus '.'
        //        chr = (char)packet[index];
        //        if (chr == '.') break;
        //        else str.Append(chr);
        //    }
        //    dest = str.ToString();
        //    return (index + 1);
        //}


        //public static int BuffToLong(byte[] buff, ref int index)
        //{
        //    try
        //    {
        //        cvt.cval0 = buff[index++];       //lsb
        //        cvt.cval1 = buff[index++];
        //        cvt.cval2 = buff[index++];
        //        cvt.cval3 = buff[index++];       //msb
        //        return (int)cvt.ival;
        //    }
        //    catch
        //    {
        //        //str = string.Format("BufftoInt  index exception {0}", index);
        //        //PutText(str);
        //        return (0);
        //    }
        //}
        //public static int BuffToInt(byte[] buff, ref int index)
        //{
        //    try
        //    {
        //        cvt.cval0 = buff[index++];       //lsb
        //        cvt.cval1 = buff[index++];
        //        cvt.cval2 = 0;
        //        cvt.cval3 = 0;                   //msb
        //        return (int)cvt.ival;
        //    }
        //    catch
        //    {
        //        //str = string.Format("BufftoInt  index exception {0}", index);
        //        //PutText(str);
        //        return (0);
        //    }
        //}


        //public int AddLong(ref byte[] Buffer, int index, long value)
        //{
        //    cvt.lval = value;
        //    Buffer[index++] = cvt.cval0;        // lsb
        //    Buffer[index++] = cvt.cval1;
        //    Buffer[index++] = cvt.cval2;
        //    Buffer[index++] = cvt.cval3;        // msb
        //    return index + 5;
        //}

        //public int AddInt(ref byte[] Buffer, int index, long value)
        //{
        //    cvt.lval = value;
        //    Buffer[index++] = cvt.cval0;        // lsb
        //    Buffer[index++] = cvt.cval1;        // msb
        //    return index + 3;
        //}

        //public void GetLong(ref byte[] buffer, int index)
        //{
        //    cvt.cval0 = buffer[index++];        // lsb
        //    cvt.cval1 = buffer[index++];
        //    cvt.cval2 = buffer[index++];
        //    cvt.cval3 = buffer[index++];        // msb
        //}

        //public void GetInt(ref byte[] buffer, int index)
        //{
        //    cvt.cval0 = buffer[index++];        // lsb
        //    cvt.cval1 = buffer[index++];
        //    cvt.cval2 = 0;
        //    cvt.cval3 = 0;                      // msb
        //}



        /*	public int StringtoCents (ref String str)
            {
                int i, l, numVal;			String strng;
                char chr;

                l = str.Length;
                strng = "";
                for (i = 1; i < l; i++) {							//filter out extraneous
                    chr = str [i];
                    switch (chr) {
                    case '.':								break;
                    case '$':								break;
                    case ' ':								break;
                    default: sb.Append ((char)chr);	break;
                    }
                }
                strng = sb.ToString ();					sb.Clear ();
                try {		
                    numVal = Convert.ToInt32 (strng);
                } catch (FormatException e) {
                    AutoInterface.Instance.MainForm.TextOutput("Input string is not a sequence of digits. {0}",e);
                    return(-1);
                } catch (OverflowException e) {
                    AutoInterface.Instance.MainForm.TextOutput("The number cannot fit in an Int32. {0}",e);
                    return(-1);
                }
                if (numVal < Int32.MaxValue)			return(numVal);				
                else	{
                    AutoInterface.Instance.MainForm.TextOutput("numVal cannot be incremented beyond its current value");
                    return(-1);
                }
            }	*/

        //private void bcd()
        //{
        //    int i, j;
        //    byte ch, dh;
        //    string convt;
        //    double dbl=0;

        //    convt = dbl.ToString("0000000000");
        //    for (i = 0; i < 10; i++)                                          //remove ascii section i.e. 34 = 4 and 
        //    {                                                                 //move to genbuf
        //        ch = (byte)convt[i]; ch &= 0x0F; genbuf[i] = (byte)ch;
        //    }
        //    for (i = 9, j = 0; i > -1; i--, j++)                                      //make packed bcd
        //    {
        //        dh = genbuf[i--]; ch = genbuf[i];
        //        ch = (byte)(ch << 4); ch |= dh;
        //        fixbuf[j] = ch;
        //    }
        //}

    }
}
