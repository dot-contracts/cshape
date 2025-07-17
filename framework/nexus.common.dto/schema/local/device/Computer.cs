using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlComputer
    {



        private bool       mNewEntry         = true;     public bool NewEntry { get { return mNewEntry; } }

        private string     mConnectionString = "";
        private string     mDataError        = "";       public string DataError { get { return mDataError; } set { mDataError = value; }}

        private dlDevice   mDevice;            public dlDevice Device          { get { return mDevice; }                set { mDevice = value; } }
                                               public string   ComputerPK      { get { return mDevice.DevicePK; }       set { mDevice.DevicePK = value; } }
                                               public string   MACAddr         { get { return mDevice.MACAddr; }        set { mDevice.MACAddr = value; } }
                                               public string   IPAddr          { get { return mDevice.IPAddr; }         set { mDevice.IPAddr = value; } }
                                               public string   Description     { get { return mDevice.Description; }    set { mDevice.Description = value; } }
                                               public string   Manufacture    { get { return mDevice.Manufacture; }   set { mDevice.Manufacture = value; } }
                                               public string   ManufactureId   { get { return mDevice.ManufactureID; }  set { mDevice.ManufactureID = value;  } }
                                               public string   Serial          { get { return mDevice.Serial; }         set { mDevice.Serial = value; } }
                                               public string   Connection      { get { return mDevice.Connection; }     set { mDevice.Connection = value; } }
                                               public string   DeviceNo        { get { return mDevice.DeviceNo; }       set { mDevice.DeviceNo = value; } }
                                               public string   Location        { get { return mDevice.Location; }       set { mDevice.Location = value; } }
        
        private nxProperty mComputerType;      public string   ComputerType    { get { return mComputerType.Value; }    set { mComputerType.Value = value;  mComputerTypeID      = EnumCache.Instance.getTypeFromDesc  ("Computer", mComputerType.Value); } }
        private string     mComputerTypeID;    public string   ComputerTypeID  { get { return mComputerTypeID; }        set { mComputerTypeID = value;      mComputerType.Value  = EnumCache.Instance.getDescFromId    (            mComputerTypeID); } }

        private nxProperty mComputerState;     public string   ComputerState   { get { return mComputerState.Value; }   set { mComputerState.Value = value; mComputerStateID     = EnumCache.Instance.getStateFromDesc ("Device",   mComputerState.Value); } }
        private string     mComputerStateID;   public string   ComputerStateID { get { return mComputerStateID; }       set { mComputerStateID = value;     mComputerState.Value = EnumCache.Instance.getDescFromId    (            mComputerStateID); } }

        private nxProperty mShell;             public string   Shell           { get { return mShell.Value; }           set { mShell.Value = value; } }

        private nxProperty mLockPC;            public string   LockPC          { get { return mLockPC.Value; }          set { mLockPC.Value = value;  } }
        private nxProperty mDefaultTimeout;    public string   DefaultTimeout  { get { return mDefaultTimeout.Value; }  set { mDefaultTimeout.Value = value;  } }
        private nxProperty mDefaultUser;       public string   DefaultUser     { get { return mDefaultUser.Value; }     set { mDefaultUser.Value = value;  } }
        private nxProperty mDefaultModule;     public string   DefaultModule   { get { return mDefaultModule.Value; }   set { mDefaultModule.Value = value;  } }
        private nxProperty mDefaultSnapin;     public string   DefaultSnapin   { get { return mDefaultSnapin.Value; }   set { mDefaultSnapin.Value = value;  } }
        private nxProperty mStartupParams;     public string   StartupParams   { get { return mStartupParams.Value; }   set { mStartupParams.Value = value;  } }
        private nxProperty mDownloadDays;      public string   DownloadDays    { get { return mDownloadDays.Value; }    set { mDownloadDays.Value = value; } }
        private nxProperty mDownloadHours;     public string   DownloadHours   { get { return mDownloadHours.Value; }   set { mDownloadHours.Value = value; } }


        public dlComputer() 
        {
            mDevice = new dlDevice(setting.ConnectionString);
            Create(setting.ConnectionString);
        }
        public dlComputer(string ConnectionString)
        {
            mDevice = new dlDevice(setting.ConnectionString);
            Create(ConnectionString);
        }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;

            mComputerType    = new nxProperty("Computer", "ComputerType");
            mComputerState   = new nxProperty("Computer", "ComputerState");

            mShell           = new nxProperty("Computer", "Shell");
            mLockPC          = new nxProperty("Computer", "LockPC");
            mDefaultTimeout  = new nxProperty("Computer", "DefaultTimeout");
            mDefaultUser     = new nxProperty("Computer", "DefaultUser");
            mDefaultModule   = new nxProperty("Computer", "DefaultModule");
            mDefaultSnapin   = new nxProperty("Computer", "DefaultSnapin");
            mStartupParams   = new nxProperty("Computer", "StartupParams");
            mDownloadDays    = new nxProperty("Computer", "DownloadDays");
            mDownloadHours   = new nxProperty("Computer", "DownloadHours");

            Reset(); 
        }

        public bool Open(int ComputerPK) { return Open(ComputerPK.ToString()); }
        public bool Open(string LocalUI = null, string SystemID = null)
        {
            LocalUI = (LocalUI == "") ? "0" : LocalUI;
            if (!LocalUI.Equals("0") || !String.IsNullOrEmpty(SystemID))
            {
                using (SQLServer DB = new SQLServer(setting.ConnectionString))
                {
                    DataTable DT = DB.GetDataTable("loc.lst_Device_Computer", ((LocalUI == null) ? "I~S~SystemID~" + SystemID : "I~S~ComputerPK~" + LocalUI));
                    if (DT.Rows.Count > 0) LoadFromRow(DT.Rows[0]);
                }
                return String.IsNullOrEmpty(mDataError);
            }
            return false;
        }

        private void LoadFromRow(DataRow Row)
        {
            mNewEntry = false;

            mDevice.Open(Row["ComputerPK"].ToString());

            mComputerType.Value   = Row["ComputerType"].ToString();
            mComputerTypeID       = Row["ComputerTypeID"].ToString();

            mComputerState.Value  = Row["ComputerState"].ToString();
            mComputerStateID      = Row["ComputerStateID"].ToString();

            mShell.Value          = Row["Shell"].ToString();
            mLockPC.Value         = Row["LockPc"].ToString();
            mDefaultTimeout.Value = Row["DefaultTimeout"].ToString();
            mDefaultUser.Value    = Row["DefaultUserID"].ToString();
            mDefaultModule.Value  = Row["DefaultModuleID"].ToString();
            mDefaultSnapin.Value  = Row["DefaultSnapin"].ToString();
            mStartupParams.Value  = Row["StartupParams"].ToString();

            mDownloadDays.Value   = Row["DownloadDays"].ToString();
            mDownloadHours.Value  = Row["DownloadHours"].ToString();
        }

        public bool Update()
        {
            string Params = "";
            if (!mNewEntry) Params = "I~S~ComputerPK~" + mDevice.DevicePK + ";";
            Params += "I~S~ComputerType~"   + mComputerTypeID + ";";
            Params += "I~S~ComputerState~"  + mComputerStateID + ";";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                Params = Params + ";" +  getParams();
                mDevice.DevicePK = DB.ExecLookup("loc.mod_Device_Computer", Params).ToString();
                mDataError = DB.DataError;
            }

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mNewEntry)
                {
                    mComputerType.Update();
                    mComputerState.Update();
                    mShell.Update();
                    mLockPC.Update();
                    mDefaultTimeout.Update();
                    mDefaultUser.Update();
                    mDefaultModule.Update();
                    mDefaultSnapin.Update();
                    mStartupParams.Update();
                    mDownloadDays.Update();
                    mDownloadHours.Update();
                }
                Open(LocalUI: mDevice.DevicePK);
                mNewEntry = false;
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Computer", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }

        public string getParams()
        {
            string Params = " ";
            if (mShell.ValueInt >0)                           Params += "I~S~ShellUI~"         + mShell.ValueInt        + ";";
            if (!String.IsNullOrEmpty(mLockPC.Value))         Params += "I~S~LockPC~"          + mLockPC.Value          + ";";
            if (!String.IsNullOrEmpty(mDefaultTimeout.Value)) Params += "I~S~DefaultTimeout~"  + mDefaultTimeout.Value  + ";";
            if (!String.IsNullOrEmpty(mDefaultUser.Value))    Params += "I~S~DefaultUser~"     + mDefaultUser.Value     + ";";
            if (!String.IsNullOrEmpty(mDefaultModule.Value))  Params += "I~S~DefaultModule~"   + mDefaultModule.Value   + ";";
            if (!String.IsNullOrEmpty(mDefaultSnapin.Value))  Params += "I~S~DefaultSnapin~"   + mDefaultSnapin.Value   + ";";
            if (!String.IsNullOrEmpty(mStartupParams.Value))  Params += "I~S~StartupParams~"   + mStartupParams.Value   + ";";
            if (!String.IsNullOrEmpty(mDownloadDays.Value))   Params += "I~S~DownloadDays~"    + mDownloadDays.Value    + ";";
            if (!String.IsNullOrEmpty(mDownloadHours.Value))  Params += "I~S~DownloadHours~"   + mDownloadHours.Value   + ";";
            Params = Params + ";" + mDevice.getParams();
            return Params;
        }



        public void Dispose() { }

        public void Reset()
        {
            mNewEntry = true;

            if (String.IsNullOrEmpty(mComputerType.Value)) ComputerType = "System";

            ComputerTypeID  = EnumCache.Instance.getTypeId   ("Computer",  mComputerType.Value);
            ComputerStateID = EnumCache.Instance.getStateId  ("Device",    "Available");

            mComputerType.Value  = ComputerType;
            mComputerState.Value = ComputerState; 

            mShell.Value          = "False";
            mLockPC.Value         = "True";
            mDefaultTimeout.Value = "5";
            mDefaultUser.Value    = "0";
            mDefaultModule.Value  = "0";
            mDefaultSnapin.Value  = "0";
            mDownloadDays.Value   = "EveryDay";
            mDownloadHours.Value  = "AllDay";

        }

        public string GetSettings()
        {
            string SaveStr = "Id=";// +mVenueID;
            //SaveStr += ";No=" + mVenueNo;
            //SaveStr += ";Type=" + mVenueType;
            //SaveStr += ";Name=" + mVenueName;
            return SaveStr;
        }

        public void SetSettings(string Settings)
        {
            //mMACAddr = getMACAddr();

            Array arr = Settings.Split(';');
            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                if (arr.GetValue(i).ToString().Contains('='))
                {
                    Array art = arr.GetValue(i).ToString().Split('=');
                    switch (art.GetValue(0).ToString().ToUpper())
                    {
                        case "MAC":
                            //mMACAddr = art.GetValue(1).ToString();
                            break;
                        case "ID":
                            //mComputerPK= art.GetValue(1).ToString();
                            break;
                        case "TYPE":
                            mComputerTypeID = art.GetValue(1).ToString();
                            break;
                        case "NAME":
                            //mDescription = art.GetValue(1).ToString();
                            break;
                    }
                }
            }
        }

        public int intComputerPk()
        {
            return mDevice.intDevicePk();
        }

        public override string ToString()
        {
            return mDevice.DevicePK + ", " + mDevice.Description;
        }

        public void ModifiedeviceStatus()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DB.ExecNonQuery("Update loc.Device_Computer set online=1,LastSeen=getDate() where ComputerPK=" + mDevice.DevicePK);
            DB.Dispose();
            DB = null;
        }
        public bool Delete()
        {
            string SQL = "DECLARE @ComputerPK int; SET @ComputerPK=" + mDevice.DevicePK + " ";
            SQL += "DECLARE @ComputerState int; SET @ComputerState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            SQL += "update [loc].[Computer] set ComputerState=@ComputerState WHERE ComputerPK=@ComputerPK";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DB.ExecNonQuery(SQL);
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }


    }
}
