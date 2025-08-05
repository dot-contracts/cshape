using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using nexus.common.dal;
using nexus.common.core;
using nexus.common.cache;
//using nexus.common.control;

namespace nexus.common.core
{
    public sealed class shell
    {
                                                                     public Venue      Venue         { get { return mLAN.Venue; }     set { mLAN.Venue = value; } }

                                                                     public Computer   Computer      { get { return mLAN.Computer; }          set { mLAN.Computer = value; } }
                                                                     public string     DefaultUser   { get { return Computer.DefaultUser; }   set { Computer.DefaultUser = value; } }
                                                                     public string     DefaultModule { get { return Computer.DefaultModule; } set { Computer.DefaultModule = value; } }

        private Worker    mWorker;                                   public Worker     Worker        { get { return mWorker; }        set { mWorker = value; } }

        private Connector mLAN;                                      public Connector  LAN           { get { return mLAN; }           set { mLAN = value; } }
        private Connector mWAN;                                      public Connector  WAN           { get { return mWAN; }           set { mWAN = value; } }
        private Connector mMAS;                                      public Connector  MAS           { get { return mMAS; }           set { mMAS = value; } }
        //private Option    mOption;                                   public Option     Option        { get { return mOption; }        set { mOption = value; } }

        private string    mMAC  = "";                                public string     MAC           { get { return getMACAddr();} }          
        private string    mMediaStr  = "";                           public string     MediaStr      { get { return mMediaStr;} }          
        private string    mModuleID  = "";                           public string     ModuleID      { get { return mModuleID; }      set { mModuleID = value; } }

        private string    mBaseDir = "C:\\Nexus";                    public string     BaseDir       { get { return mBaseDir;}        set { mBaseDir=value; } }
        private bool      mValid = false;                            public bool       Valid         { get { return mValid; } }

        public PropertyBag<NameValue> mPaths;                        public PropertyBag<NameValue> Paths    { get { return mPaths; }    set { mPaths   = value; } }
        public PropertyBag<NameValue> mPropBin;                      public PropertyBag<NameValue> PropBin  { get { return mPropBin; }  set { mPropBin = value; } }

        private string    mSettDir = "C:\\Nexus\\Settings";
        private string    mSettingFile = "";
        private string    mError = "";


        public string     ComputerID      { get { return mLAN.Computer.ComputerPK; } }
        public int        ComputerId      { get { return helpers.ToInt(mLAN.Computer.ComputerPK); } }
        public string     ComputerType    { get { return mLAN.Computer.ComputerType; } }
        public string     ComputerName    { get { return mLAN.Computer.Description; } }
        public string     VenueID         { get { return mLAN.Venue.VenuePK; } }
        public int        VenueId         { get { return helpers.ToInt(mLAN.Venue.VenuePK); } }
        public string     VenueType       { get { return mLAN.Venue.VenueType; } }
        public string     WorkerID        { get { return mWorker.WorkerPK; } }
        public int        WorkerId        { get { return helpers.ToInt((string.IsNullOrEmpty(mWorker.WorkerPK) ? "0" : mWorker.WorkerPK )); } }
        public string     WorkerName      { get { return mWorker.Description; } }

        public string     GroupId         { get { return ""; } }
        
        public  string    ServerIP        { get { return mLAN.Server; } }
        public  string    ServerPort      { get { return mLAN.ServerPort; } }
        public  string    Catalog         { get { return mLAN.Catalog; } }
        public  string    LANConnStr      { get { return mLAN.ConnStr; } }
        public  string    WANConnStr      { get { return mWAN.ConnStr; } }
        public  bool      IsAppServer     { get { return false; } }
        public  string    ConnStr         { get { return mLAN.ConnStr; } }
        
        public  string    AppDir          { get { return mBaseDir; } }
        public  string    Basetype        { get { return "Nexus"; } }


        public delegate void OnWorkerChangeEventHandler(int WorkerRoleID);  public event OnWorkerChangeEventHandler OnWorkerChange; 


        private static shell mInstance = new shell();
        public static  shell Instance { get { return mInstance; } }


        private shell()  
        {
            mLAN = new Connector();

            mPropBin = new PropertyBag<NameValue>(); 
        }

        public bool Create(string ModuleName, string ParamPath)
        {

            //setting.GetSettings();

            mLAN = new Connector();
            mLAN.Server     = setting.Server;
            mLAN.Catalog    = setting.Catalog;
            mLAN.Socket     = "localhost";
            mLAN.SocketPort = "52563";

            if (Connect(false))
            {
                return EnumCache.Instance.Create(); 
            }

            return false;
        }


        private bool Connect(bool LoadFromServer)
        {
            string LocalStr  = ".";
            string GlobalStr = "server.xcitemedia.com.au";
            string MasterStr = "";

            string DeviceStr = "";
            string VenueStr  = "";
            string MediaStr  = "";

            string ExecName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().CodeBase.ToString());

            mBaseDir = "C:\\Nexus\\";
            mSettDir = Path.Combine(mBaseDir, "Settings");
            if (!Directory.Exists(mSettDir)) Directory.CreateDirectory(mSettDir);

            mSettingFile = Path.Combine(mSettDir, ExecName + ".TXT");
            if (!File.Exists(mSettingFile))
                mSettingFile = Path.Combine(mSettDir, "ENDPOINT.TXT");

            mValid = false;

            if (File.Exists(mSettingFile))
            {
                System.IO.StreamReader oRdr = new System.IO.StreamReader(mSettingFile);

                string inLine = "";
                do
                {
                    if (oRdr.EndOfStream)      break; 
                    inLine = oRdr.ReadLine();
                    if ((inLine != null))
                    {
                        if (inLine.Contains(","))
                        {
                            Array arr = inLine.Split(',');
                            if (arr.GetLength(0) == 2)
                            {
                                switch (arr.GetValue(0).ToString().ToUpper().Trim())
                                {
                                    case "LOCAL":     LocalStr  = arr.GetValue(1).ToString().Trim();  break;
                                    case "GLOBAL":    GlobalStr = arr.GetValue(1).ToString().Trim();  break;
                                    case "MASTER":    MasterStr = arr.GetValue(1).ToString().Trim();  break;
                                    case "COMPUTER":  DeviceStr = arr.GetValue(1).ToString().Trim();  break;
                                    case "VENUE":     VenueStr  = arr.GetValue(1).ToString().Trim();  break;
                                    case "MEDIA":     mMediaStr = arr.GetValue(1).ToString().Trim();  break;
                                }
                            }
                        }
                    }
                } while (true);

                oRdr.Close();

                media.media.Instance.Create(mBaseDir, MediaStr);

                string PathFile = Path.Combine(mSettDir, "PATH.TXT");

                if (File.Exists(PathFile))
                {
                    oRdr = new System.IO.StreamReader(PathFile);

                    inLine = "";
                    do
                    {
                        if (oRdr.EndOfStream) break;
                        inLine = oRdr.ReadLine();
                        if ((inLine != null))
                        {
                            if (inLine.Contains("="))
                            {
                                Array arr = inLine.Split('=');
                                if (arr.GetLength(0) == 2)
                                    mPaths.Add(new NameValue(arr.GetValue(0).ToString().ToUpper().Trim(), arr.GetValue(1).ToString().Trim()));
                            }
                        }
                    } while (true);

                    oRdr.Close();
                }


                if (LoadFromServer)
                {
                    CreateConnectors();

                    if (mWAN.Create(GlobalStr, VenueStr, DeviceStr, true, true))
                    {
                        if (mLAN.Create(LocalStr, VenueStr, DeviceStr, false, false))
                        {
                            //mLAN.Device.Create(mWAN.Device.GetSettings());
                            //mLAN.Venue.Create(mWAN.Venue.GetSettings());
                        }
                    }
                }
                else
                {
                    if (mLAN.Create(LocalStr, VenueStr, DeviceStr, true, true))
                    {
                        CreateConnectors();

                        mWAN.Create(GlobalStr, VenueStr, DeviceStr, false, false);
                        if (!String.IsNullOrEmpty(MasterStr)) mMAS.Create(MasterStr, VenueStr, DeviceStr, false, false);
                        mValid = true;
                    }
                    else
                    {
                        CreateConnectors();
                        if (mLAN.Create("Server=" + GetWanConnection(mLAN.Server) , VenueStr, DeviceStr, true, true))
                        {
                            //mLAN.Computer.Create(mLAN.Device.GetSettings)
                            //mLAN.Venue.Create(mLAN.Venue.GetSettings)
                        }
                        mWAN.Create(GlobalStr, VenueStr, DeviceStr, false, false);
                    }
                }

                //uno  if (mValid) mOption = new Option("EndPoint.Settings");
            }
            else { mError="Cannot find Settings File:" + mSettingFile + ":"; }

            if (mValid | (!File.Exists(mSettingFile)) ) Save();

            return mValid;
        }
        private void CreateConnectors()
        {
            mWAN = new Connector("Global");
            mWAN.Server     = "server.xcitemedia.com.au";
            mWAN.Catalog    = "GlobalNet";
            mWAN.Socket     = "server.xcitemedia.com.au";
            mWAN.SocketPort = "52561";

            mMAS = new Connector("Master");
            mMAS.Server     = "";
            mMAS.Catalog    = "";
            mMAS.Socket     = "";
            mMAS.SocketPort = "52564";
        }
        private string GetWanConnection(string Default)
        {
            string ret = Default;
            string Connection = string.Empty;
            using (SQLServer wDB = new dal.SQLServer(WAN.Server, "terminal", "Flexinet", "s3cr3t"))
                Connection = wDB.ExecLookup("select Connection from hosts left join terminals on terminals.hostid=hosts.id where terminals.mac='" + MAC + "'");

            ret = String.IsNullOrEmpty(Connection) ? ret : Connection;
            return ret;
        }


        public void Save()
        {
            setting.ComputerId = ComputerID;

            string bkFile = Path.ChangeExtension(mSettingFile, ".bk");

            if (File.Exists(mSettingFile))
            {
                if (File.Exists(bkFile)) File.Delete(bkFile);
                File.Copy(mSettingFile, bkFile);

                if (File.Exists(bkFile))
                {
                    StreamWriter FO = new StreamWriter(mSettingFile, false);
                    StreamReader FI = new System.IO.StreamReader(bkFile);

                    FO.WriteLine("Inserted  " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff"));

                    bool[] props = new bool[5];

                    string inLine = "";
                    do
                    {
                        if (FI.EndOfStream) break;
                        inLine = FI.ReadLine();
                        if ((inLine != null))
                        {
                            if (inLine.Contains(","))
                            {
                                if (!inLine.Contains("Inserted")  && !inLine.Contains("Created"))
                                {
                                    Array arr = inLine.Split(',');
                                    if (arr.GetLength(0) == 2)
                                    {
                                        switch (arr.GetValue(0).ToString().ToUpper().Trim())
                                        {
                                            case "LOCAL":
                                                props[0] = true;
                                                FO.WriteLine("Local,    " + mLAN.GetSettings());
                                                break;
                                            case "COMPUTER":
                                                props[1] = true;
                                                FO.WriteLine("Computer, " + mLAN.Computer.GetSettings());
                                                break;
                                            case "VENUE":
                                                props[2] = true;
                                                FO.WriteLine("Venue,    " + mLAN.Venue.GetSettings());
                                                break;
                                            case "GLOBAL":
                                                if (mWAN != null)
                                                {
                                                    props[3] = true;
                                                    FO.WriteLine("Global,   " + mWAN.GetSettings());
                                                }
                                                break;
                                            case "MASTER":
                                                if (mMAS != null)
                                                {
                                                    props[4] = true;
                                                    FO.WriteLine("Master,   " + mMAS.GetSettings());
                                                }
                                                break;
                                            default:
                                                FO.WriteLine(inLine);   // otherwise pass it through
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    } while (true);

                    if (!props[0]) FO.WriteLine("Local,    " + mLAN.GetSettings());
                    if (!props[1]) FO.WriteLine("Computer, " + mLAN.Computer.GetSettings());
                    if (!props[2]) FO.WriteLine("Venue,    " + mLAN.Venue.GetSettings());
                    if (mWAN != null)
                    {
                        if (!props[3]) FO.WriteLine("Global,   " + mWAN.GetSettings());
                    }
                    if (mMAS != null)
                    {
                        if (!props[4]) FO.WriteLine("Master,   " + mMAS.GetSettings());
                    }

                    FI.Close();
                    FO.Close();

                    File.Delete(bkFile);
                }

            }
            else
            {
                StreamWriter FO = new StreamWriter(mSettingFile, false);
                FO.WriteLine("Inserted  " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff"));
                FO.WriteLine("Local,    " + mLAN.GetSettings());
                FO.WriteLine("Computer, " + mLAN.Computer.GetSettings());
                FO.WriteLine("Venue,    " + mLAN.Venue.GetSettings());
                if (mWAN != null) FO.WriteLine("Global,   " + mWAN.GetSettings());
                if (mMAS != null) FO.WriteLine("Master,   " + mMAS.GetSettings());
                FO.Close();
            }
        }

        public void Update()   { }

        public void LoadRootWorker()
        {
            mWorker = new Worker();
            mWorker.LoadRootWorker();
        }

        private string getMACAddr()
        {
            mMAC = setting.getMACAddr();
            return mMAC;
        }

        public string GetPath(string PathType, string PathName)
        {
            return mPaths[PathName].Value;
        }


        public bool ExecuteSQL(string Package, string Process, string SQL)
        {
            bool Executed = true;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                int exec = DB.ExecNonQuery(SQL);
                if (!exec.Equals(1)) Executed = false;

                if (!Executed)
                {
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + DB.DataError.Replace("'", "") + "')");
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + SQL.Replace("'", "") + "')");
                }
            }

            return Executed;
        }
        public string ExecuteLookUp(string Package, string Process, string SQL)
        {
            string lookup = string.Empty;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                lookup = DB.ExecLookup(SQL);

                if (!DB.DataError.Equals(""))
                {
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + DB.DataError.Replace("'", "") + "')");
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + SQL.Replace("'", "") + "')");
                }
            }

            return lookup;
        }
        public DataTable GetDataTable(string Package, string Process, string SQL)
        {
            DataTable data = new DataTable();

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                data = DB.GetDataTable(SQL);
                if (!DB.DataError.Equals(""))
                {
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + DB.DataError.Replace("'", "") + "')");
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + SQL.Replace("'", "") + "')");
                }
            }
            return data;
        }
        public DataSet GetDataSet(string Package, string Process, string SQL)
        {
            DataSet data = new DataSet();
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                data = DB.GetDataSet(SQL);
                if (!DB.DataError.Equals(""))
                {
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + DB.DataError.Replace("'", "") + "')");
                    DB.ExecNonQuery("Insert Into Log (TerminalID,Package,Command,LogDate,Message) Values(-1,'" + Package + "','" + Process + "','" + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") + "','" + SQL.Replace("'", "") + "')");
                }
            }
            return data;
        }


    }
}
