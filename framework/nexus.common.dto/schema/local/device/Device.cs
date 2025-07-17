using System;
using System.Data;
using System.Linq;

using nexus.common.dto;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlDevice
    {
        private string     mConnectionString = "";
        private string     mDataError;         public string   DataError       { get { return mEntity.DataError + ";" + mDataError; }   set { mDataError = value; } }
        private bool       mChanged;           public bool     Changed         { get { return mChanged; }                               set { mChanged = value; } }

        private dlEntity   mEntity;            public dlEntity Entity           { get { return mEntity; }                set { mEntity = value; } }
                                               public string   DevicePK         { get { return mEntity.EntityPK; }       set { mEntity.EntityPK = value; } }
                                               public string   Description      { get { return mEntity.Description; }    set { mEntity.Description = value; } }
                                               public bool     NewEntry         { get { return mEntity.NewEntry; }       set { mEntity.NewEntry = value; } }

        private nxProperty mDeviceType;        public string   DeviceType       { get { return mDeviceType.Value; }      set { mDeviceType.Value = value;                                                         mDeviceTypeID        = EnumCache.Instance.getTypeFromDesc  ("Entity.Device",     mDeviceType.Value); } }
        private string     mDeviceTypeID;      public string   DeviceTypeID     { get { return mDeviceTypeID; }          set { if (!value.Equals(mDeviceTypeID))         mChanged = true; mDeviceTypeID = value;  mDeviceType.Value    = EnumCache.Instance.getDescFromId    (                     mDeviceTypeID); } }

        private nxProperty mDeviceState;       public string   DeviceState      { get { return mDeviceState.Value; }     set { mDeviceState.Value = value;                                                        mDeviceStateID       = EnumCache.Instance.getStateFromDesc ("Entity.Device",     mDeviceState.Value); } }
        private string     mDeviceStateID;     public string   DeviceStateID    { get { return mDeviceStateID; }         set { if (!value.Equals(mDeviceStateID))        mChanged = true; mDeviceStateID = value; mDeviceState.Value   = EnumCache.Instance.getDescFromId    (                     mDeviceStateID); } }

        private nxProperty mManufacture;      public string   Manufacture     { get { return mManufacture.Value; }    set { mManufacture.Value = value;                                                       mManufactureID     = ManufactureCache.Instance.LookUpValue ( "Description",   mManufacture.Value, "CompanyPk"   ); } }
        private string     mManufactureID;    public string   ManufactureID    { get { return mManufactureID; }         set { if (!value.Equals(mManufactureID))        mChanged = true; mManufactureID = value;mManufacture.Value = ManufactureCache.Instance.LookUpValue ( "CompanyPk",     mManufactureID,     "Description" ); } }

        private nxProperty mSerial;            public string   Serial           { get { return mSerial.Value; }          set { if (!value.Equals(mSerial.Value))         mChanged = true; mSerial.Value = value; } }
        private nxProperty mFingerprint;       public string   Fingerprint      { get { return mFingerprint.Value; }     set { if (!value.Equals(mFingerprint.Value))    mChanged = true; mFingerprint.Value = value; } }

        private string     mMACAddr = "";      public string   MACAddr          { get { return mMACAddr; }               set { if (!value.Equals(mMACAddr))              mChanged = true; mMACAddr = value; }}
        private string     mIPAddr = "";       public string   IPAddr           { get { return mIPAddr; }                set { if (!value.Equals(mIPAddr))               mChanged = true; mIPAddr = value; } }

        private string     mConnection;        public string   Connection       { get { return mConnection; }            set { if (!value.Equals(mConnection))           mChanged = true; mConnection = value; } }
        private string     mDeviceNo;          public string   DeviceNo         { get { return mDeviceNo; }              set { if (!value.Equals(mDeviceNo))             mChanged = true; mDeviceNo = value; } }
        private string     mLocation;          public string   Location         { get { return mLocation; }              set { if (!value.Equals(mLocation))             mChanged = true; mLocation = value; } }
        private string     mEndPointID;        public string   EndPointID       { get { return mEndPointID; }            set { if (!value.Equals(mEndPointID))           mChanged = true; mEndPointID = value; } }

        private nxProperty mReferenceNo;       public string   ReferenceNo      { get { return mReferenceNo.Value; }     set { if (!value.Equals(mReferenceNo.Value))    mChanged = true; mReferenceNo.Value = value; } }
        private nxProperty mOnline;            public string   Online           { get { return mOnline.Value; }          set { if (!value.Equals(mOnline.Value))         mChanged = true; mOnline.Value = value; } }
        private nxProperty mLastSeen;          public string   LastSeen         { get { return mLastSeen.Value; }        set { if (!value.Equals(mLastSeen.Value))       mChanged = true; mLastSeen.Value = value; } }
        private nxProperty mAutoReboot;        public string   AutoReboot       { get { return mAutoReboot.Value; }      set { if (!value.Equals(mAutoReboot.Value))     mChanged = true; mAutoReboot.Value = value; } }
        private nxProperty mAutoRebootTime;    public string   AutoRebootTime   { get { return mAutoRebootTime.Value; }  set { if (!value.Equals(mAutoRebootTime.Value)) mChanged = true; mAutoRebootTime.Value = value;  } }


        public dlDevice() 
        {
            mMACAddr = setting.getMACAddr();
            mEntity = new dlEntity("Device");
            Create(setting.ConnectionString);
        }
        public dlDevice(string ConnectionString)
        {
            mMACAddr = setting.getMACAddr();
            mEntity  = new dlEntity("Device");
            Create(ConnectionString);
        }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;
            mEntity.Create(mConnectionString);

            mManufacture    = new nxProperty("Device", "Manufacture");
            mSerial          = new nxProperty("Device", "Serial");
            mFingerprint     = new nxProperty("Device", "Fingerprint");

            mDeviceType      = new nxProperty("Device", "DeviceType");
            mDeviceState     = new nxProperty("Device", "DeviceState");
            mReferenceNo     = new nxProperty("Device", "ReferenceNo");
            mOnline          = new nxProperty("Device", "Online");
            mLastSeen        = new nxProperty("Device", "LastSeen");
            mAutoReboot      = new nxProperty("Device", "AutoReboot");
            mAutoRebootTime  = new nxProperty("Device", "AutoRebootTime");

            Reset(); 
        }

        public bool Open(int DevicePK)    { return Open(DevicePK.ToString()); }
        public bool Open(string DevicePK)
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Device", "I~S~DevicePK~" + DevicePK);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {

            mEntity.Open(Row["DevicePK"].ToString());

            mSerial.Create         (Row["Serial"].ToString());
            mFingerprint.Create    (Row["Fingerprint"].ToString());
            ManufactureID         = Row["ManufactureID"].ToString();

            mDeviceTypeID         = Row["DeviceTypeID"].ToString(); 
            mDeviceStateID        = Row["DeviceStateID"].ToString();

            mDeviceType.Create     (Row["DeviceType"].ToString());
            mDeviceState.Create    (Row["DeviceState"].ToString());

            //mOnline.Value         = Row["Online"].ToString();
            mLastSeen.Create       (Row["LastSeen"].ToString());
            mIPAddr                = Row["IPAddr"].ToString();
            mMACAddr               = Row["MACAddr"].ToString();

            mReferenceNo.Create    (Row["ReferenceNo"].ToString());
            mAutoReboot.Create     (Row["AutoReboot"].ToString());
            mAutoRebootTime.Create (Row["AutoRebootTime"].ToString());



        }

        /// <summary>
        /// 
        /// </summary>
        public bool Update()
        {
            string Params = "";
            if (!mEntity.NewEntry) Params = "I~S~DevicePK~" + mEntity.EntityPK + ";";
            Params += "I~S~DeviceType~"     + mDeviceTypeID          + ";";
            Params += "I~S~DeviceState~"    + mDeviceStateID         + ";";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                mEntity.EntityPK = DB.ExecLookup("loc.mod_Device", Params + getParams()).ToString();
                mDataError = DB.DataError;
            }

            if (String.IsNullOrEmpty(mDataError))
            {
                UpdateProperty();
                Open(mEntity.EntityPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Device", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mConnection))           Params += "I~N~Connection~"     + mConnection            + ";";
            if (!String.IsNullOrEmpty(mLocation))             Params += "I~N~Location~"       + mLocation              + ";";
            if (!String.IsNullOrEmpty(mManufactureID))       Params += "I~N~ManufactureID~"  + mManufactureID        + ";";
            if (!String.IsNullOrEmpty(mSerial.Value))         Params += "I~S~Serial~"         + mSerial.Value          + ";";
            if (!String.IsNullOrEmpty(mFingerprint.Value))    Params += "I~S~FingerPrint~"    + mFingerprint.Value     + ";";
            if (!String.IsNullOrEmpty(mDeviceNo))             Params += "I~N~DeviceNo~"       + mDeviceNo              + ";";
            if (!String.IsNullOrEmpty(mReferenceNo.Value))    Params += "I~S~ReferenceNo~"    + mReferenceNo.Value     + ";";
            if (!String.IsNullOrEmpty(mEndPointID))           Params += "I~N~EndPointID~"     + mEndPointID            + ";";
            if (!String.IsNullOrEmpty(mOnline.Value))         Params += "I~N~Online~"         + mOnline.Value          + ";";
            if (!String.IsNullOrEmpty(mLastSeen.Value))       Params += "I~S~LastSeen~"       + mLastSeen.Value        + ";";
            if (!String.IsNullOrEmpty(mMACAddr))              Params += "I~S~MACAddr~"        + mMACAddr               + ";";
            if (!String.IsNullOrEmpty(mIPAddr))               Params += "I~S~IPAddr~"         + mIPAddr                + ";";
            if (!String.IsNullOrEmpty(mAutoReboot.Value))     Params += "I~S~AutoReboot~"     + mAutoReboot.Value      + ";";
            if (!String.IsNullOrEmpty(mAutoRebootTime.Value)) Params += "I~S~AutoRebootTime~" + mAutoRebootTime.Value  + ";";
            Params = Params + ";" + mEntity.getParams();
            return Params;
        }
        public void UpdateProperty()
        {
            if (!mEntity.NewEntry)
            {
                mEntity.UpdateProperty();

                mManufacture.Update();
                mSerial.Update();
                mFingerprint.Update();
                mDeviceType.Update();
                mDeviceState.Update();
                mReferenceNo.Update();
                mOnline.Update();
                mLastSeen.Update();
                mAutoReboot.Update();
                mAutoRebootTime.Update();
            }
        }


        public void Find(string Column, string Value)
        {

        }

        public void Dispose() { }

        public void Reset()
        {
            if (mEntity == null)
                mEntity = new dlEntity("Device");
            else
                mEntity.Reset("Device");

            if (String.IsNullOrEmpty(mDeviceType.Value)) mDeviceType.Value = "Computer";

            DeviceTypeID  = EnumCache.Instance.getTypeId  ("Device",  mDeviceType.Value);
            DeviceStateID = EnumCache.Instance.getStateId ("Device", "Available");

            mSerial.Reset();
            mFingerprint.Reset();

            mDeviceType.Value  = DeviceType;
            mDeviceState.Value = DeviceState; 

            mReferenceNo.Value          = "";
            mAutoReboot.Value     = "True";
            mAutoRebootTime.Value = "5:00 AM";
            //mLockPC.Value         = "True";
            //mDefaultTimeout.Value = "5";
            //mDefaultUser.Value    = "";
            //mDefaultModule.Value  = "";
            //mLogging.Value       = "0";
            //mDownloadDays.Value   = "EveryDay";
            //mDownloadHours.Value  = "AllDay";

            mEntity.NewEntry = true;
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
            mMACAddr = setting.getMACAddr();

            Array arr = Settings.Split(';');
            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                if (arr.GetValue(i).ToString().Contains('='))
                {
                    Array art = arr.GetValue(i).ToString().Split('=');
                    switch (art.GetValue(0).ToString().ToUpper())
                    {
                        case "MAC":
                            mMACAddr = art.GetValue(1).ToString();
                            break;
                        case "ID":
                            //mDevicePK= art.GetValue(1).ToString();
                            break;
                        case "TYPE":
                            mDeviceTypeID = art.GetValue(1).ToString();
                            break;
                        case "NAME":
                            //mDescription = art.GetValue(1).ToString();
                            break;
                    }
                }
            }
        }

        public int intDevicePk()
        {
            return mEntity.intEntityPk(); 
        }
        public override string ToString()
        {
            return mEntity.Description;
        }

        public bool Delete()
        {
            //string SQL = "DECLARE @DevicePK int; SET @DevicePK=" + mDevicePK + " ";
            //SQL += "DECLARE @DeviceState int; SET @DeviceState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            //SQL += "update [loc].[Device] set DeviceState=@DeviceState WHERE DevicePK=@DevicePK";

            //SQLServer DB = new SQLServer(setting.ConnectionString);
            //DB.ExecNonQuery(SQL);
            //mDataError = DB.DataError;
            //DB.Dispose();
            //DB = null;

            //nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }


    }
}
