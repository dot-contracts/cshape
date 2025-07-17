using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using nexus.common.core;
using nexus.common.dal;

namespace nexus.common.core
{
    public class Connector
    {
        private string     mConnectorName;
        
        private Terminator mTerminator;
        private Venue      mVenue    = new Venue();     public Venue      Venue        { get { return mVenue; }        set { mVenue = value; } }
        private Computer   mComputer = new Computer();  public Computer   Computer     { get { return mComputer; }     set { mComputer = value; } }
        private string     mConnectError = "";          public string     ConnectError { get { return mConnectError; } set { mConnectError = value; } }

        public string ServerIP   { get { return mTerminator.ServerIP;   } set { mTerminator.ServerIP = value; } }
        public string Server     { get { return mTerminator.Server;     } set { mTerminator.Server = value; } }
        public string ServerPort { get { return mTerminator.ServerPort; } set { mTerminator.ServerPort = value; } }
        public string Connect    { get { return mTerminator.Connect;    } set { mTerminator.Connect = value; } }
        public string Catalog    { get { return mTerminator.Catalog;    } set { mTerminator.Catalog = value; } }
        public string UserName   { get { return mTerminator.UserName;   } set { mTerminator.UserName = value; } }
        public string Password   { get { return mTerminator.Password;   } set { mTerminator.Password = value; } }
        public string ConnStr    { get { return mTerminator.ConnStr;    } }

        public string Socket     { get { return mTerminator.Socket; }     set { mTerminator.Socket = value; } }
        public string SocketPort { get { return mTerminator.SocketPort; } set { mTerminator.SocketPort = value; } }


        public Connector()                      { mConnectorName = "Local";       mTerminator = new Terminator(mConnectorName); }
        public Connector (string ConnectorName) { mConnectorName = ConnectorName; mTerminator = new Terminator(mConnectorName); }

        public bool Create(string Settings, string VenueSetting, string ComputerSetting, bool TestConnection, bool LookUp)
        {
            bool Valid = false;

            if (mTerminator.Create(Settings, TestConnection))
            {

                mComputer = new Computer  (mTerminator.ConnStr, ComputerSetting);
                mVenue    = new Venue     (mTerminator.ConnStr, VenueSetting);

                if (mConnectorName.Equals("Local"))
                   nexus.common.cache.EnumCache.Instance.Create();  // load the enum cache ASAP!

                shell.Instance.LoadRootWorker();
                Valid = true;

                if (LookUp)
                {
                    mVenue.LoadVenue(mTerminator.ConnStr);
                    mComputer.LoadComputer(mTerminator.ConnStr, mVenue.VenuePK);
                }
            }

            return Valid;
        }

        public bool CanConnect()
        {
            mConnectError = "";
            Boolean Connected = false;
            //          if (mTerminator.PingTest(mTerminator.Server))
            {
                SQLServer DB = new SQLServer(mTerminator.ConnStr);
                Connected = DB.Connect();
                mConnectError = DB.DataError;
                DB.Dispose();
                DB = null;
            }
            //          else
            //          {
            //              mConnectError = "Cannot ping " + mTerminator.Server;
            //          }
            return Connected;
        }

        public string GetSettings()
        {
            return mTerminator.GetSettings();
        }
    }
}
