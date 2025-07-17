using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using nexus.common.dal;
using nexus.common.core;

namespace nexus.common.core
{
    public class Terminator
    {

        private string mServerIP   = "";            public string ServerIP    { get { return mServerIP; }   set { mServerIP = value; } }
        private string mServer     = "10.171.0.3";  public string Server      { get { return mServer; }     set { mServer = value; SetConnString(); } }
        private string mServerPort = "";            public string ServerPort  { get { return mServerPort; } set { mServerPort = value; SetConnString(); } }
        private string mConnect    = "Default";     public string Connect     { get { return mConnect; }    set { mConnect = value; } }
        private string mCatalog    = "Nexus";       public string Catalog     { get { return mCatalog; }    set { mCatalog = value; SetConnString(); } }
        private string mUsername   = "Nexus";       public string UserName    { get { return mUsername; }   set { mUsername = value; SetConnString(); } }
        private string mPassword   = "N3x9s1!3";    public string Password    { get { return mPassword; }   set { mPassword = value; SetConnString(); } }
        private string mSocket     = "localhost";   public string Socket      { get { return mSocket; }     set { mSocket = value; } }
        private string mSocketPort = "52563";       public string SocketPort  { get { return mSocketPort; } set { mSocketPort = value; } }
        private string mConnStr;                    public string ConnStr     { get { return mConnStr; } }


        public Terminator(string ConnectorName)
        {
            switch (ConnectorName)
            {
                case "Local":
                case "Slave":
                    mServer = "10.171.0.3";
                    break;
                case "Master":
                    mServer = "10.171.0.3";
                    break;
                case "Global":
                    mServer = "server.nextnetgaming.com.au";
                    break;
            }
            SetConnString();
        }

        public bool Create(string Settings, bool TestConnection)
        {

            bool Valid = true;

            Array arr = Settings.Split(';');
            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                if (arr.GetValue(i).ToString().Contains('='))
                {
                    Array art = arr.GetValue(i).ToString().Split('=');
                    switch (art.GetValue(0).ToString().ToUpper())
                    {
                        case "SERVER":
                            mServer = art.GetValue(1).ToString().Trim();
                            break;
                        case "CATALOG":
                            string ct = art.GetValue(1).ToString().Trim();
                            mCatalog = (String.IsNullOrEmpty(ct) ? mCatalog : ct);
                            break;
                        case "CONNECT":
                            mConnect = art.GetValue(1).ToString().Trim();
                            break;
                        case "USERNAME":
                            string un = art.GetValue(1).ToString().Trim();
                            mUsername = (String.IsNullOrEmpty(un) ? mUsername : un);
                            break;
                        case "PASSWORD":
                            string pw = art.GetValue(1).ToString().Trim();
                            mPassword = (String.IsNullOrEmpty(pw) ? mPassword : pw);
                            break;
                        case "SERVERPORT":
                            mServerPort = art.GetValue(1).ToString().Trim();
                            break;
                    }
                }
            }

            SetConnString();

            if (TestConnection) Valid = CanConnect();

            return Valid;
        }

        public string GetSettings()
        {
            string SaveStr = "Server=" + mServer;
            if (mCatalog != "Nexus")
                SaveStr += ";Catalog=" + mCatalog;
            if (mConnect != "Default")
                SaveStr += ";Connect=" + mConnect;
            if (mUsername != "Nexus")
            {
                SaveStr += ";Username=" + mUsername;
                if (mPassword != "N3x9s1!3") SaveStr += ";Password=" + mPassword;
            }
            if (!string.IsNullOrEmpty(mServerPort))
                SaveStr += ";ServerPort=" + mServerPort;
            return SaveStr;
        }

        public bool CanConnect()
        {
            bool Connected = false;
            //          if (PingTest(mServer))
            {
                SQLServer DB = new SQLServer(mConnStr);
                Connected = DB.Connect();
                DB.Dispose();
                DB = null;
            }
            return Connected;
        }

        private void SetConnString()
        {
            mConnStr = "Data Source=" + mServer + (String.IsNullOrEmpty(mServerPort) | mServerPort.Equals("1433") ? "" : "," + mServerPort) + ";Initial Catalog=" + mCatalog + ";User ID=" + mUsername + ";Password=" + mPassword + ";Trusted_Connection=False;Connection Timeout=60000";
        }

        public bool PingTest(string IPAddress)
        {
            bool Valid = false;

            string PingSvr = IPAddress;
            if (PingSvr.Contains("sqlexpress"))
            {
                Array arr = PingSvr.Split('\\');
                PingSvr = arr.GetValue(0).ToString();
            }
            if (!PingSvr.Contains(".com"))
            {
                if (!PingSvr.StartsWith("25."))
                {
                    if (!ValidIP(PingSvr))
                    {
                        if (PingSvr == ".") PingSvr = "127.0.0.1";
                        PingSvr = getLocalIP(PingSvr);
                        if (PingSvr.Equals(""))
                        {
                        }
                    }
                }
            }
            if (ValidIP(PingSvr) | PingSvr.StartsWith("25.") | PingSvr.Contains(".com"))
            {
                try
                {
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    PingReply reply = ping.Send(PingSvr);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success) Valid = true;
                }
                catch (Exception) { }
            }

            return Valid;
        }

        public string getLocalIP(string LocalAddress)
        {   
            try
            {
                IPHostEntry ipE = Dns.GetHostEntry(LocalAddress);
                // resolve name to ip
                IPAddress IpA;
                IPAddress[] IpAList = ipE.AddressList;
                for (int i = 0; i <= IpAList.GetLength(0) - 1; i++)
                {
                    IpA = IpAList[i];
                    string AFamily = IpA.AddressFamily.ToString();
                    if (AFamily.Equals(AddressFamily.InterNetwork.ToString()))
                    {
                        string AAddress = IpA.ToString();
                        if (ValidIP(AAddress))
                        {
                            return AAddress;
                        }
                    }
                }

                ipE = Dns.GetHostEntry("127.0.0.1");
                // resolve name to ip
                IpAList = ipE.AddressList;
                for (int i = 0; i <= IpAList.GetLength(0) - 1; i++)
                {
                    IpA = IpAList[i];
                    string AFamily = IpA.AddressFamily.ToString();
                    if (AFamily.Equals(AddressFamily.InterNetwork.ToString()))
                    {
                        string AAddress = IpA.ToString();
                        if (ValidIP(AAddress))
                        {
                            return AAddress;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return "";
        }
        public bool ValidIP(string IPAddress)
        {
            Array arr = IPAddress.Split('.');
            if (arr.GetLength(0) == 4)
            {
                if (IsValidSub(arr.GetValue(0).ToString()))
                {
                    if (IsValidSub(arr.GetValue(1).ToString()))
                    {
                        if (IsValidSub(arr.GetValue(2).ToString()))
                        {
                            if (IsValidSub(arr.GetValue(3).ToString()))
                            {
                                // if a hamachi driver installed then dont use
                                if (!(arr.GetValue(0).ToString() == "5" | arr.GetValue(0).ToString() == "25"))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool IsValidSub(string SubNet)
        {
            Int32 numSub;
            if (Int32.TryParse(SubNet, out numSub))
                return (numSub >= 1 && numSub <= 255);

            return false;
        }
    }
}
