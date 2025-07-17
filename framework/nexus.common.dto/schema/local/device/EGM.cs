using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlEgm
    {

        private string     mConnectionString = "";

        private bool       mNewEntry = true;   public bool     NewEntry        { get { return mNewEntry; } }
        private string     mDataError;         public string   DataError       { get { return mDevice.DataError + ";" + mDataError; }   set { mDataError = value; } }

        private dlDevice   mDevice;            public dlDevice Device          { get { return mDevice; }                set { mDevice = value; } }
                                               public string   EgmPK           { get { return mDevice.DevicePK; }       set { mDevice.DevicePK = value; } }
                                               public string   MACAddr         { get { return mDevice.MACAddr; }        set { mDevice.MACAddr = value; } }
                                               public string   IPAddr          { get { return mDevice.IPAddr; }         set { mDevice.IPAddr = value; } }
                                               public string   Description     { get { return mDevice.Description; }    set { mDevice.Description = value; } }
                                               public string   Manufacture    { get { return mDevice.Manufacture; }   set { mDevice.Manufacture = value; } }
                                               public string   ManufactureId   { get { return mDevice.ManufactureID; }  set { mDevice.ManufactureID = value;  } }
                                               public string   Serial          { get { return mDevice.Serial; }         set { mDevice.Serial = value; } }
                                               public string   Connection      { get { return mDevice.Connection; }     set { mDevice.Connection = value; } }
                                               public string   DeviceNo        { get { return mDevice.DeviceNo; }       set { mDevice.DeviceNo = value; } }
                                               public string   Location        { get { return mDevice.Location; }       set { mDevice.Location = value; } }
                                               public string   EgmType         { get { return mDevice.DeviceType; }     set { mDevice.DeviceType = value; } }
                                               public string   EgmTypeId       { get { return mDevice.DeviceTypeID; }   set { mDevice.DeviceTypeID = value; } }
                                               public string   EgmState        { get { return mDevice.DeviceState; }    set { mDevice.DeviceState = value; } }
                                               public string   EgmStateId      { get { return mDevice.DeviceStateID; }  set { mDevice.DeviceStateID = value; } }


        //private nxProperty mEgmType;           public string   EgmType    { get { return mEgmType.Value; }    set { mEgmType.Value = value;  mEgmTypeID      = EnumCache.Instance.getTypeFromDesc  ("Egm",     mEgmType.Value); } }
        //private string     mEgmTypeID;         public string   EgmTypeID  { get { return mEgmTypeID; }        set { mEgmTypeID = value;      mEgmType.Value  = EnumCache.Instance.getDescFromId    (           mEgmTypeID); } }

        private string     mGameID;            public string   GameID       { get { return mGameID; }         set { mGameID = value; } }
        private string     mVarnoID;           public string   VarnoID      { get { return mVarnoID; }        set { mVarnoID = value; } }
        private string     mLocationID;        public string   LocationID   { get { return mLocationID; }     set { mLocationID = value; } }
        private string     mInterfaceID;       public string   InterfaceID  { get { return mInterfaceID; }    set { mInterfaceID = value; } }
        private string     mMemberID;          public string   MemberID     { get { return mMemberID; }       set { mMemberID = value; } }
        private string     mLineitemID;        public string   LineitemID   { get { return mLineitemID; }     set { mLineitemID = value; } }
        private string     mSnapshotID;        public string   SnapshotID   { get { return mSnapshotID; }     set { mSnapshotID = value; } }

        private nxProperty mTicketHopper;      public string   TicketHopper { get { return mTicketHopper.Value; } set { mTicketHopper.Value = value; } }
        private nxProperty mGMID;              public int      GMID         { get { return mGMID.ValueInt; }      set { mGMID.Value = value.ToString();  } }
        private nxProperty mHouseNo;           public string   HouseNo      { get { return mHouseNo.Value; }      set { mHouseNo.Value = value;  } }
        private nxProperty mBaseNo;            public string   BaseNo       { get { return mBaseNo.Value; }       set { mBaseNo.Value = value;  } }
        private nxProperty mDenom;             public string   Denom        { get { return mDenom.Value; }        set { mDenom.Value = value;  } }
        private nxProperty mVarNo;             public string   VarNo        { get { return mVarNo.Value; }        set { mVarNo.Value = value;  } }
        private nxProperty mSpecNo;            public string   SpecNo       { get { return mSpecNo.Value; }       set { mSpecNo.Value = value;  } }
        private nxProperty mRTP;               public string   RTP          { get { return mRTP.Value; }          set { mRTP.Value = value;  } }

        public dlEgm() 
        {
            mDevice = new dlDevice();
            Create(setting.ConnectionString);
        }
        public dlEgm(string ConnectionString)
        {
            mDevice = new dlDevice();
            Create(ConnectionString);
        }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;

            mDevice.Create(mConnectionString);

            //mEgmType         = new nxProperty("Egm", "EgmType");
            //mEgmState        = new nxProperty("Egm", "EgmState");
            mGMID            = new nxProperty("Egm", "GMID");
            mTicketHopper    = new nxProperty("Egm", "TicketHopper");
            mHouseNo         = new nxProperty("Egm", "HouseNo");
            mBaseNo          = new nxProperty("Egm", "BaseNo");
            mDenom           = new nxProperty("Egm", "Denom");
            mVarNo           = new nxProperty("Egm", "VarNo");
            mSpecNo          = new nxProperty("Egm", "SpecNo");
            mRTP             = new nxProperty("Egm", "RTP");

            Reset(); 
        }

        public bool Open(int EgmPK) { return Open(EgmPK.ToString()); }
        public bool Open(string LocalId)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Device_Egm", "I~S~EgmPK~" + LocalId);
                if (DT.Rows.Count > 0)
                    LoadFromRow(DT.Rows[0]);
            }

            return String.IsNullOrEmpty(mDataError); 

        }


        private void LoadFromRow(DataRow Row)
        {
            mDevice.Open(Row["EgmPK"].ToString());

            //mEgmTypeID           = Row["EgmTypeID"].ToString();
            //mEgmStateID          = Row["EgmStateID"].ToString();
            mGameID              = Row["GameID"].ToString();

            //mEgmType.Create      ( Row["EgmType"].ToString());
            //mEgmState.Create     ( Row["EgmState"].ToString());
            mGMID.Create         ( Row["GMID"].ToString());
            mTicketHopper.Create ( Row["TicketHopper"].ToString());
            //mHouseNo.Create      ( Row["HouseNo"].ToString());
            //mBaseNo.Create       ( Row["BaseNo"].ToString());
            mDenom.Create        ( Row["DenomID"].ToString());
            mVarNo.Create        ( Row["VarNo"].ToString());
            mSpecNo.Create       ( Row["SpecNo"].ToString());
            mRTP.Create          ( Row["RTP"].ToString());

            //DateTime dob;
            //string DOB = Row["Birthdate"].ToString();
            //if (string.IsNullOrEmpty(DOB))
            //    mBirthdate.Create( DateTime.Now.AddYears(-16).ToString("dd MMM, yyyy"));
            //else
            //{
            //    if (DateTime.TryParse(Row["Birthdate"].ToString(), out dob))
            //    {
            //        if (DateTime.Now.Subtract(dob).TotalDays > (365 * 150)) dob = DateTime.Now.AddYears(-150);
            //        mBirthdate.Create( dob.ToString("dd MMM, yyyy"));
            //    }
            //    else
            //        mBirthdate.Create( DateTime.Now.AddYears(-16).ToString("dd MMM, yyyy"));
            //}

            mNewEntry = false;

        }

        public bool Update()
        {

            mDevice.ReferenceNo = mGMID.Value; 

            string Params = "";
            if (!mNewEntry) Params = "I~N~EgmPK~" + EgmPK + ";";
            //Params += "I~N~EgmType~"      + mEgmTypeID + ";";
            //Params += "I~N~EgmState~"     + mEgmStateID + ";";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            mDevice.DevicePK = DB.ExecLookup("loc.mod_Device_EGM", Params + getParams());
            mDataError = DB.DataError;

            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                //mEgmType.Update();
                //mEgmState.Update();
                mTicketHopper.Update();
                mHouseNo.Update();
                mBaseNo.Update();
                mDenom.Update();
                mVarNo.Update();
                mSpecNo.Update();
                mRTP.Update();

                mDevice.UpdateProperty(); 

                Open(LocalId: mDevice.DevicePK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "EGM", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);
        }
        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mTicketHopper.Value)) Params += "I~N~TicketHopper~"   + mTicketHopper.Value + ";";
            if (!String.IsNullOrEmpty(mDenom.Value))        Params += "I~N~Denom~"          + mDenom.Value + ";";
            if (!String.IsNullOrEmpty(mSpecNo.Value))       Params += "I~S~SpecNo~"         + mSpecNo.Value + ";";
            if (!String.IsNullOrEmpty(mVarNo.Value))        Params += "I~S~VarNo~"          + mVarNo.Value  + ";";
            if (!String.IsNullOrEmpty(mRTP.Value))          Params += "I~F~RTP~"            + mRTP.Value  + ";";
            if (!String.IsNullOrEmpty(mGameID))             Params += "I~N~GameId~"         + mGameID + ";";
            Params += mDevice.getParams();
            return Params;
        }

        public void Find(string Column, string Value)
        {

        }

        public void Dispose() { }

        public void Reset()
        {
            mNewEntry = true;

        // if (String.IsNullOrEmpty(mEgmType.Value)) mEgmType.Value = "PRG";
        //    EgmTypeID  = EnumCache.Instance.getTypeId  ("Egm",    mEgmType.Value);
        //    EgmStateID = EnumCache.Instance.getStateId ("Device", "Available");

        //    if (mDevice == null)
        //        mDevice = new dlDevice("Egm");
        //    else
        //        mDevice.Reset();

        //    mEgmType.Value  = EgmType;
        //    mEgmState.Value = EgmState; 

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
                            //mEgmPK= art.GetValue(1).ToString();
                            break;
                        case "TYPE":
                           // mEgmTypeID = art.GetValue(1).ToString();
                            break;
                        case "NAME":
                            //mDescription.Value = art.GetValue(1).ToString();
                            break;
                    }
                }
            }
        }


        public override string ToString()
        {
            return mDevice.Description ;
        }
        public void ModifiedeviceStatus()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DB.ExecNonQuery("Update loc.Egm set online=1,LastSeen=getDate() where EgmPK=" + EgmPK.ToString());
            DB.Dispose();
            DB = null;
        }
        public bool Delete()
        {
            string SQL = "DECLARE @EgmPK int; SET @EgmPK=" + EgmPK + " ";
            SQL += "DECLARE @EgmState int; SET @EgmState=" + EnumCache.Instance.getStateId("Entity", "Deleted") + " ";
            SQL += "update [loc].[Egm] set EgmState=@EgmState WHERE EgmPK=@EgmPK";

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
