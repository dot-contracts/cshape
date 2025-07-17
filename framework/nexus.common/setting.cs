using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using nexus.common.dal;
using nexus.common.cache;

///D:\NextNet\current\binary\framework\nexus.common.dto.dll

namespace nexus.common
{
    public static partial class setting
    {
        private static bool Initialised = false;

        private static string MacAddr      = string.Empty;
        private static string SettingsFile = "C:\\Nexus\\Settings\\EndPoint.Txt";
        public  static string ConnectionString = "";

        //public static string AppSvr    = "http://apisand.nextnetsystems.com.au:8081";
        //public static string CommonSvr = "http://apisand.nextnetsystems.com.au:9010";
        //public static string GamingSvr = "http://apisand.nextnetsystems.com.au:9090";
        //public static string PromoSvr  = "http://apisand.nextnetsystems.com.au:9090";

        public static string AppSvr    = "http://25.5.135.59:3310";
        public static string CommonSvr = "http://25.5.135.59:3300";
        public static string GamingSvr = "http://25.5.135.59:3325";
        public static string PromoSvr  = "http://25.5.135.59:3340";


        public static string iisServer = "http://220.233.96.142:9010";

        public static string ExecName = System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);

        public static string ContentDir = "C:\\Nexus\\Content";//  "F:\\SVN\\NextNet\\NextNet_Dir\\Content";

        public static string Server     = "10.10.0.9";
        public static string Catalog    = "Nexus";
        public static string Username   = "Nexus";
        public static string Password   = "N3x9s1!3";

        public static string ftpServer  = string.Empty;
        public static string ftpUser    = string.Empty;
        public static string ftpPW      = string.Empty;

        public static string VenueId    = string.Empty;
        public static string WorkerId   = string.Empty;
        public static string ComputerId = string.Empty;

        public static string Vendor     = "SwiftPos";
        public static string Location   = "Pos301";

        public static bool GetSettings(string UniqueId = "")
        {
            Initialised = true;

            string inLine = "";
            string Value = "";

            Array vars;
            Array vart;

            //nMessage?.Invoke("Loading Settings file " + SettingsFile);

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
                            if (arr.GetLength(0) >= 2)
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

                                case "COMMONSVR":
                                    vars = (arr.GetValue(1).ToString() + ";;;;").Split(';');
                                    Value = vars.GetValue(0).ToString().Trim(); vart = (Value + "=").Split('=');
                                    CommonSvr = vart.GetValue(1).ToString();
                                    break;

                                case "GAMINGSVR":
                                    vars = (arr.GetValue(1).ToString() + ";;;;").Split(';');
                                    Value = vars.GetValue(0).ToString().Trim(); vart = (Value + "=").Split('=');
                                    GamingSvr = vart.GetValue(1).ToString();
                                    break;

                                }
                            }
                        }
                    }
                } while (true);
                oRdr.Close();

                using (SQLServer DB = new SQLServer(Server, Catalog, Username, Password))
                    ConnectionString = DB.ConnectionString;


                    //OnMessage?.Invoke("Settings file Loaded");

                    //if (GetSettings(Server, Catalog, Username, Password))
                    //{
                    //    Array CmdLines = Environment.GetCommandLineArgs();
                    //    if (CmdLines.GetLength(0) > 1)
                    //    {
                    //        string CmdLine = CmdLines.GetValue(1).ToString();
                    //        if (CmdLine.Contains("=")) ProcessSettings(CmdLine);
                    //    }
                    //    return true;
                    //}
                }

            return true;
        }

        public static bool GetSettings(string Server, string Catalog, string Username, string Password)
        {
            Array vars;
            Array vart;

            //OnMessage?.Invoke("Opening DB Settings");

            //using (SQLServer DB = new SQLServer(Server, Catalog, Username, Password))
            //{
            //    ConnectionString = DB.ConnectionString;

            //    OnMessage?.Invoke("Testing DB Connection " + ConnectionString);

            //    if (!DB.TestConnect(Server, Catalog, Username, Password))
            //    {
            //        OnMessage?.Invoke("Failed to Connect to DB");
            //        return false;
            //    }

            //    string EgmId = DB.ExecLookup("select devicepk from loc.device where macaddr='" + MacAddr + "'");
            //    int egmId = helpers.ToInt(EgmId);

            //    using (DataTable DT = DB.GetDataTable("cmn.OptionGet", "I~S~ParentCode~EgmCommon;I~S~ComputerId~" + egmId))
            //    {
            //        for (int i = 0; i < DT.Rows.Count; i++)
            //        {
            //            string value = DT.Rows[i]["Value"].ToString();
            //            if (value.Contains(";")) value = value.Replace(";", "|");
            //            vars = (value + "||||||||||").Split('|');

            //            string OpnCode = DT.Rows[i]["OptionCode"].ToString().ToUpper();

            //            switch (OpnCode)
            //            {


            //            }
            //        }
            //    }
            //}
            return true;
        }

        public static string getMACAddr()
        {

            //OnMessage?.Invoke("getting MAC Addr :" + MacAddr + ";");

            if (!String.IsNullOrEmpty(MacAddr)) return MacAddr;


            MacAddr = "";
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        return nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            catch (Exception ex) 
            {
                //OnMessage?.Invoke("MAC err:" + ex.Message + ";");
            }

            MacAddr = MacAddr.Replace(":", "-");

            //OnMessage?.Invoke("MAC Addr :" + MacAddr + ";");

            return MacAddr;
        }

       // public static bool IsConnected => NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;

        public static string GetLocalIPAddress()
        {
            //OnMessage?.Invoke("get Local IP");
            string localIP = string.Empty;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }

            }
            catch (Exception ex)
            {
                //OnMessage?.Invoke("IP err: " + ex.Message);
            }
            //OnMessage?.Invoke("LocalIP: " + localIP);
            return localIP;
        }

 
    }
}
