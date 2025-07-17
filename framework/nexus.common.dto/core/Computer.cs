using System;
using System.Linq;

using System.Data;
using System.Net;
using System.Net.Sockets;

using nexus.common.dal;
using nexus.common.dto;
using nexus.common.cache;

namespace nexus.common.core
{
    public class Computer : dlComputer
    {

        private string  mLocalUI  = "";
        private string  mSystemID = "";

        private string mConnStr = "";

        public Computer()  {}

        public Computer(string ConnectionString, string Settings)
        {
            base.Create(ConnectionString);

            Array arr = Settings.Split(';');
            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                if (arr.GetValue(i).ToString().Contains('='))
                {
                    Array art = arr.GetValue(i).ToString().Split('=');
                    switch (art.GetValue(0).ToString().ToUpper())
                    {
                        case "LocalId":
                            mLocalUI= art.GetValue(1).ToString();
                            break;
                        case "SystemID":
                            mSystemID = art.GetValue(1).ToString();
                            break;
                    }
                }
            }
        }

        public bool LoadComputer(string ConnectionString, string VenueID)
        {
            mConnStr = ConnectionString;

            SQLServer DB = new SQLServer(mConnStr);

            base.Reset();

            bool ret = false;
            
            if (!String.IsNullOrEmpty(mSystemID))
                base.Open(SystemID: mSystemID);

            //if (base.NewEntry)
            //{
            //    mLocalUI = VenueID;
            //    base.Open(LocalUI: mLocalUI);
            //}

            if (base.NewEntry)
            {
                string SQL = "select EntityID from loc.endpoint where macaddr='" + shell.Instance.MAC + "' and endpointstate=cmn.getStatePK('Entity','Active')";

                int computerId = 0;
                if (int.TryParse(DB.ExecLookup(SQL), out computerId))
                    ret = base.Open(computerId.ToString());
                else
                {
                    dlComputer Computer   = new dlComputer(mConnStr);
                    Computer.Device.Entity.VenueID      = VenueID;
                    Computer.Description                = Environment.MachineName;
                    Computer.IPAddr                     = getLocalIP("");
                    Computer.MACAddr                    = MACAddr;
                    Computer.ComputerType               = "System";
                    Computer.Update();

                    EventCache.Instance.InsertEvent("Information", ActionCache.Instance.strId("Action", "Computer", "Computer"), Environment.MachineName + " Computer Inserted");

                    string ComputerId = Computer.ComputerPK;

                    int cId = 0;
                    if (int.TryParse(ComputerId, out cId))
                    {
                        dlEndpoint EP = new dlEndpoint(ConnectionString);

                        string MAC = setting.getMACAddr();
                        if (MAC != null)
                        {
                            MAC = MAC.Replace(":", "-");
                            if (string.IsNullOrEmpty(MACAddr)) MACAddr = MAC;

                            EP = new dlEndpoint(ConnectionString);
                            EP.EntityID = ComputerId;
                            EP.MACAddr = MAC;
                            EP.Update();

                            if (Computer.Device.EndPointID == null)
                            {
                                Computer.Device.EndPointID = EP.EndpointUK;
                                Computer.Device.Update();
                            }
                        }
                    }

                    ret = !Computer.NewEntry;
                }

            }

            DB.Dispose();
            DB = null;

            return ret;

        }

        public new string GetSettings()
        {
            string SaveStr  = "LocalID="   + base.ComputerPK;
                   SaveStr += ";SystemID=" + base.Device.Entity.SystemID;
            return SaveStr;
        }

        public string getLocalIP(string LocalAddress)
        {
            try
            {
                IPHostEntry ipE = Dns.GetHostEntry(LocalAddress);
                IPAddress IpA;
                IPAddress[] IpAList = ipE.AddressList;

                if (!String.IsNullOrEmpty(LocalAddress))
                {
                    // resolve name to ip
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
                if (IsValidSub(arr.GetValue(0).ToString(), false))
                {
                    if (IsValidSub(arr.GetValue(1).ToString(),true))
                    {
                        if (IsValidSub(arr.GetValue(2).ToString(), true))
                        {
                            if (IsValidSub(arr.GetValue(3).ToString(), true))
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
        private bool IsValidSub(string SubNet, bool AllowZero)
        {
            Int32 numSub;
            if (Int32.TryParse(SubNet, out numSub))
                return (numSub >= (AllowZero ? 0 : 1) && numSub <= 255);

            return false;
        }

    }
}
