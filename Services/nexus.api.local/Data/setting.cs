using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Management;
//using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

using nexus.common.dal;
//using nexus.common.cache;
using System.Drawing;
using System.Xml.Linq;


namespace nexus.common.dal
{
    public static partial class setting
    {

        private static string SettingsFile = "C:\\Nexus\\Settings\\EndPoint.Txt";

        public static string HostServer = "192.168.172.4";
        public static int HostPort = 5340;

        public static string EgmServer = "192.168.172.4";
        public static int EgmPort = 5340;

        public static int UDPPort = 5340;

        public static string CrtServer = "192.168.172.4";
        public static int CrtPort = 5350;

        public static string Server = "192.168.172.4";
        public static string Catalog = "Nexus";
        public static string Username = "Nexus";
        public static string Password = "N3x9s1!3";
        public static string ConnectionString = "";
        public static string MacAddr = "";

        public static string LocalUI = "87";
        public static string SystemID = "None";
        public static string Type = "Club";
        public static string Name = "Nextet";

        public static string FRServer = "192.168.172.4";
        public static string FRusername = "igtest";
        public static string FRpassword = "Ig@bt3v@l";

        public static string IDVVersion = "https://idface.idvpacific.com.au:8080/api_version";
        public static string IDVLiveness = "https://idface.idvpacific.com.au:8080/check_liveness";
        public static string IDVMatch    = "https://match.idverifi.au/api/v1/faceapi/compare";
        public static string IDVusername = "MNR3G3HMDG603V7";
        public static string IDVpassword = "1O6IMNMLW1TVPCR";

        public static string EgmCom = "COM5";
        public static string RFIDPort = "COM4";
        public static string RFIDType = "0";
        public static int    CamPort = -1;
        public static int    CamExposure = -1;
        public static int    WalkawayCredit = 100;
        public static int    WalkawayTime = 10;
        public static int    ReserveTime = 180;            //seconds 

        public static bool   IsSimulator = false;
        public static bool   PeriodMeters = true;
        public static bool   IsFloorSim = false;
        public static bool   IsCCU = false;
        public static bool   IsDebug = false;
        public static bool   IsFRLogin = false;
        public static bool   IsFRpin = false;
        public static bool   IsCamera = false;
        public static bool   IsTicket = false;
        public static bool   IsPubSys = false;
        public static bool   FRpin = false;
        public static bool   Limitpin = false;
        public static bool   Changepin = false;
        public static bool   IntelCam = false;
        public static bool   SyncTime = true;
        public static bool   ShowLimits = true;
        public static int    Screen = -1;   // what screen do you want the PAM to run on?
        public static int    PinTimeout = 0; // pin timeout in minutes

        public static string TestCard = "0001022974";
        public static string WithdrawLimit = "1000";
        public static string LossLimit = "1000";

        public static string VenueId = "0";
        public static string VenueName = "NextNet";
        public static string ComputerId = "0";
        public static string WorkerId = "0";

        public static string ftpServer = "ftp://221.121.132.41";

        public static string ExecName = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);

        public static string ContentDir = "D:\\Content";//  "F:\\SVN\\NextNet\\NextNet_Dir\\Content";

        //public static bool GetSettings() { return GetSettings(getMACAddr()); }
        public static bool GetSettings()
        {
            string inLine = "";
            string Value = "";

            Array vars;
            Array vart;

            //EventCache.Instance.InsertEvent("Information", ActionCache.Instance.strId("Action", "Setting", "Message"), "Loading Settings file " + SettingsFile);

            if (System.IO.File.Exists(SettingsFile))
            {
                System.IO.StreamReader oRdr = new System.IO.StreamReader(SettingsFile);
                do
                {
                    if (oRdr.EndOfStream) break;
                    inLine = oRdr.ReadLine();
                    if ((inLine != null))
                    {
                        if (inLine.Contains(","))
                        {
                            Array arr = inLine.Split(',');
                            if (arr.GetLength(0) == 2)
                            {
                                string elem = arr.GetValue(0).ToString().ToUpper().Trim();
                                switch (elem)
                                {
                                    case "LOCAL":
                                        vars = (arr.GetValue(1).ToString() + ";;;;").Split(';');
                                        Value = vars.GetValue(0).ToString().Trim(); vart = (Value + "=").Split('='); Server = vart.GetValue(1).ToString();
                                        Value = vars.GetValue(1).ToString().Trim(); vart = (Value + "=").Split('='); Catalog = vart.GetValue(1).ToString();
                                        Value = vars.GetValue(2).ToString().Trim(); vart = (Value + "=").Split('='); Username = vart.GetValue(1).ToString();
                                        Value = vars.GetValue(3).ToString().Trim(); vart = (Value + "=").Split('='); Password = vart.GetValue(1).ToString();

                                        Catalog  = String.IsNullOrEmpty(Catalog) ? "Nexus" : Catalog;
                                        Username = String.IsNullOrEmpty(Username) ? "Nexus" : Username;
                                        Password = String.IsNullOrEmpty(Password) ? "N3x9s1!3" : Password;
                                        break;

                                    case "CAMPORT":
                                        vars = (arr.GetValue(1).ToString() + ";;;;").Split(';');
                                        Value = vars.GetValue(0).ToString().Trim(); vart = (Value + "=").Split('='); int.TryParse(vars.GetValue(1).ToString(), out CamPort);
                                        break;

                                    case "CAMEXPPSURE":
                                        vars = (arr.GetValue(1).ToString() + ";;;;").Split(';');
                                        Value = vars.GetValue(0).ToString().Trim(); vart = (Value + "=").Split('='); int.TryParse(vars.GetValue(1).ToString(), out CamExposure);
                                        break;

                                    case "CONTENTDIR": ContentDir = arr.GetValue(1).ToString().Trim(); break;

                                    case "VENUE":
                                        vars = (arr.GetValue(1).ToString() + ";;;;").Split(';');
                                        Value = vars.GetValue(0).ToString().Trim(); vart = (Value + "=").Split('='); LocalUI = vart.GetValue(1).ToString();
                                        Value = vars.GetValue(1).ToString().Trim(); vart = (Value + "=").Split('='); SystemID = vart.GetValue(1).ToString();
                                        Value = vars.GetValue(2).ToString().Trim(); vart = (Value + "=").Split('='); Type = vart.GetValue(1).ToString();
                                        Value = vars.GetValue(3).ToString().Trim(); vart = (Value + "=").Split('='); VenueName = vart.GetValue(1).ToString();
                                        break;
                               }
                            }
                        }
                    }
                } while (true);
                oRdr.Close();

                if (GetSettings(Server, Catalog, Username, Password))
                {
                    Array CmdLines = Environment.GetCommandLineArgs();
                    if (CmdLines.GetLength(0) > 1)
                    {
                        string CmdLine = CmdLines.GetValue(1).ToString();
                        if (CmdLine.Contains("=")) ProcessSettings(CmdLine);
                    }
                    return true;
                }
            }

            return false;
        }


        //System.Windows.Forms.MessageBox.Show(":" + Server + ": :" + Catalog + ": :" + Username + ": :" + Password + ":" );

        public static bool GetSettings(string Server, string Catalog, string Username, string Password)
        {
            Array vars;
            Array vart;

            using (SQLServer DB = new SQLServer(Server, Catalog, Username, Password))
            {
                ConnectionString = DB.ConnectionString;

                if (!DB.Connect()) return false;

                using (DataTable DT = DB.GetDataTable("cmn.OptionGet", "I~S~ParentCode~EgmCommon"))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        string value = DT.Rows[i]["Value"].ToString();
                        if (value.Contains(";")) value = value.Replace(";", "|");
                        vars = (value + "||||||||||").Split('|');

                        string OpnCode = DT.Rows[i]["OptionCode"].ToString().ToUpper();
                        switch (OpnCode)
                        {
                            case "IDVSERVER":
                                IDVVersion  = vars.GetValue(0).ToString().Trim();
                                IDVLiveness = vars.GetValue(1).ToString().Trim();
                                IDVMatch    = vars.GetValue(2).ToString().Trim();
                                IDVusername = vars.GetValue(3).ToString().Trim();
                                IDVpassword = vars.GetValue(4).ToString().Trim();
                                break;

                            case "FACESERVER":
                                FRServer   = vars.GetValue(0).ToString().Trim();
                                FRusername = vars.GetValue(1).ToString().Trim();
                                FRpassword = vars.GetValue(2).ToString().Trim();
                                break;

                            case "HOSTSVR":
                            case "HOSTSERVER":
                                HostServer = vars.GetValue(0).ToString().Trim();
                                int.TryParse(vars.GetValue(1).ToString(), out HostPort);
                                break;

                            case "EGMSVR":
                            case "EGMSERVER":
                                EgmServer = vars.GetValue(0).ToString().Trim();
                                int.TryParse(vars.GetValue(1).ToString(), out EgmPort);
                                break;

                            case "CRTSVR":
                            case "CRTSERVER":
                                CrtServer = vars.GetValue(0).ToString().Trim();
                                int.TryParse(vars.GetValue(1).ToString(), out CrtPort);
                                break;

                            case "RFID":
                                RFIDPort = vars.GetValue(0).ToString().Trim();
                                RFIDType = vars.GetValue(1).ToString().Trim();
                                break;

                            case "SETTINGS": ProcessSettings(value); break;

                            case "FTPSERVER":
                                ftpServer = vars.GetValue(0).ToString().Trim();
                                break;


                            case "UDPPORT": int.TryParse(vars.GetValue(1).ToString(), out UDPPort); break;
                            case "TESTCARD": TestCard = vars.GetValue(0).ToString().Trim(); break;
                            case "EGMCOM": EgmCom = vars.GetValue(0).ToString().Trim(); break;
                        }
                    }
                }
            }
            //FRServer = "https://192.168.50.200/watchface/api/v1|igtest|Ig@bt3v@l";              // !!!
            return true;
        }

        private static void ProcessSettings(string settings)
        {
            Array vars = settings.Split('|');

            for (int j = 0; j < vars.GetLength(0); j++)
            {
                Array vart = (vars.GetValue(j).ToString().Trim() + "=").Split('=');

                switch (vart.GetValue(0).ToString().ToUpper())
                {
                    case "ISSIMULATOR":    IsSimulator     = vart.GetValue(1).ToString().ToUpper().Equals("TRUE"); IsFloorSim = false; break; 
                    case "ISDEBUG":        IsDebug         = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    case "INTELCAM":       IntelCam        = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break; 
                    case "FRLOGIN":        IsFRLogin       = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    case "FRPIN":          IsFRpin         = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    case "ISCCU":          IsCCU           = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    case "SCREEN":         Screen          = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "ISCAMERA":       IsCamera        = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break; 
                    case "ISTICKET":       IsTicket        = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break; 
                    case "CAMPORT":        CamPort         = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "CAMEXPOSURE":    CamExposure     = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "WALKAWAYTIME":   WalkawayTime    = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "WALKAWAYCREDIT": WalkawayCredit  = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "PINTIMEOUT":     PinTimeout      = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "RESERVETIME":    ReserveTime     = Convert.ToInt32(vart.GetValue(1).ToString());                             break;
                    case "PERIODMETERS":   PeriodMeters    = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    case "ISPUBSYS":       IsPubSys        = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    case "SHOWLIMITS":     ShowLimits      = vart.GetValue(1).ToString().ToUpper().Equals("TRUE");                     break;
                    default: break;
                }
            }

            //FRServer = "https://192.168.50.200/watchface/api/v1|igtest|Ig@bt3v@l";  //see above to remove debug setting !!!
            //IsDebug = true;
            //IsCamera = true;
            //IntelCam = true;
            //IsFRLogin = true;
            //IsSimulator = false;
            //IsPubSys = true;
        }

        //private static string getMACAddr()
        //{

        //    if (!String.IsNullOrEmpty(MacAddr)) return MacAddr;

        //    MacAddr = "";
        //    try
        //    {
        //        System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
        //        foreach (System.Management.ManagementObject mo in mc.GetInstances())
        //        {
        //            if (mo["IPEnabled"].Equals(true))
        //            {
        //                MacAddr = mo["MacAddress"].ToString();
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception) { }

        //    MacAddr = MacAddr.Replace(":", "-");

        //    return MacAddr;
        //}

        public static string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static string BroadCastIP()
        {
            string broadCastIP = GetLocalIPAddress();
            broadCastIP = broadCastIP.Substring(0, broadCastIP.LastIndexOf('.'))+ ".255";
            return broadCastIP;
        }
    }
}
