using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlVenue
    {

        private string     mLastVenueType    = "";
        private string     mConnectionString = "";
        private string     mDataError        = "";

        private dlCompany  mCompany;           public dlCompany Company      { get { return mCompany; }                        set { mCompany = value; } }
                                               public string VenuePK         { get { return mCompany.CompanyPK; }              set { mCompany.CompanyPK = value; } }
                                               public string CompanyNo       { get { return mCompany.CompanyNo; }              set { mCompany.CompanyNo = value; } }
                                               public string Description     { get { return mCompany.Description; }            set { mCompany.Description = value; } }
                                               public string Contact         { get { return mCompany.Contact; }                set { mCompany.Contact = value;   } }
                                               public string ContactID       { get { return mCompany.ContactID; }              set { mCompany.ContactID = value;   } }
                                               public string Address         { get { return mCompany.Address; }                set { mCompany.Address = value; } }
                                               public string AddressID       { get { return mCompany.AddressID; }              set { mCompany.AddressID = value; } }
                                               public string Phone           { get { return mCompany.Phone; }                  set { mCompany.Phone = value; } }
                                               public string PhoneID         { get { return mCompany.PhoneID; }                set { mCompany.PhoneID = value; } }
                                               public string Property        { get { return mCompany.Property; }               set { mCompany.Property = value; } }
                                               public bool   NewEntry        { get { return mCompany.NewEntry; }               set { mCompany.NewEntry = value; } }

        private nxProperty mVenueType;         public string  VenueType      { get { return mVenueType.Value; }                set { mVenueType.Value = value;   mVenueTypeID      = EnumCache.Instance.getTypeFromDesc  ("Entity.Company.Venue",   mVenueType.Value); } }
        private string     mVenueTypeID;       public string  VenueTypeID    { get { return mVenueTypeID; }                    set { mVenueTypeID = value;       mVenueType.Value  = EnumCache.Instance.getDescFromId    (                          mVenueTypeID); } }
        private nxProperty mVenueState;        public string  VenueState     { get { return mVenueState.Value; }               set { mVenueState.Value = value;  mVenueStateID     = EnumCache.Instance.getStateFromDesc ("Entity.Company",         mVenueState.Value); } }
        private string     mVenueStateID;      public string  VenueStateID   { get { return mVenueStateID; }                   set { mVenueStateID = value;      mVenueState.Value = EnumCache.Instance.getDescFromId    (                          mVenueStateID); } }

        private nxProperty mLicensee;          public string  Licensee       { get { return mLicensee.Value; }                 set { mLicensee.Value = value; } }
        private string     mLicenseeID;        public string  LicenseeID     { get { return mLicenseeID; }                     set { mLicenseeID = value; } }
        private nxProperty mLicenceType;       public string  LicenceType    { get { return mLicenceType.Value; }              set { mLicenceType.Value = value; mLicenceTypeID     = EnumCache.Instance.getTypeFromDesc ("Licence", mLicenceType.Value); } }
        private string     mLicenceTypeID;     public string  LicenceTypeID  { get { return mLicenceTypeID; }                  set { mLicenceTypeID = value;     mLicenceType.Value = EnumCache.Instance.getDescFromId   (           mLicenceTypeID); } }

        private nxProperty mLicenceNo;         public string  LicenceNo      { get { return mLicenceNo.Value; }                set { mLicenceNo.Value = value; } }

        private nxProperty mJurisdiction;      public string  Jurisdiction   { get { return mJurisdiction.Value; }             set { mJurisdiction.Value = value; } }
        private string     mJurisdictionID;    public string  JurisdictionID { get { return mJurisdictionID; }                 set { mJurisdictionID = value; } }

        private nxProperty mServer;            public string  Server         { get { return mServer.Value; }                   set { mServer.Value = value; } }
        private string     mServerID;          public string  ServerID       { get { return mServerID; }                       set { mServerID = value; } }

        private nxProperty mPeriodStart;       public string  PeriodStart    { get { return mPeriodStart.Value; }              set { mPeriodStart.Value = value; } }
        private nxProperty mInterval;          public string  Interval       { get { return mInterval.Value; }                 set { mInterval.Value = value; } }
        private string     mDayStart;          public string  DayStart       { get { return mDayStart; }                       set { mDayStart = value; } }


        //private System.Drawing.Image mVenueLogo;

        private bool mHasLogo = false;

        public dlVenue() 
        {
            mCompany = new dlCompany(setting.ConnectionString);
            Create(setting.ConnectionString);
        }
        public dlVenue(string ConnectionString)
        {
            mCompany = new dlCompany(ConnectionString);
            Create(ConnectionString);
        }
        public void Dispose() { }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;

            mVenueType    = new nxProperty("Venue",   "VenueType");
            mVenueState   = new nxProperty("Venue",   "VenueState");
            mLicensee     = new nxProperty("Venue",   "VenueLicensee");
            mLicenceType  = new nxProperty("Venue",   "VenueLicenceType");
            mLicenceNo    = new nxProperty("Venue",   "LicenceNo");
            mJurisdiction = new nxProperty("Venue",   "Jurisdiction");
            mServer       = new nxProperty("Venue",   "Server");
            mPeriodStart  = new nxProperty("Venue",   "PeriodStart");
            mInterval     = new nxProperty("Venue",   "Interval");

            Reset("Club");
        }
        public bool Open(string LocalUI = null, string SystemID = null)
        {
            Reset("Club");

            using (SQLServer DB = new SQLServer(mConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Company_Venue", ((LocalUI == null) ? "I~S~SystemID~" + SystemID : "I~S~VenuePK~" + LocalUI));
                if (DT.Rows.Count > 0) LoadFromRow(DT.Rows[0]);
                mDataError = DB.DataError;
            }

            return String.IsNullOrEmpty(mDataError);

        }

        private void LoadFromRow(DataRow Row)
        {
            mCompany.Open(Row["VenuePK"].ToString());

            mVenueTypeID        = Row["VenueTypeID"].ToString();
            mVenueStateID       = Row["VenueStateID"].ToString();
            mLicenseeID         = Row["LicenseeID"].ToString();
            mLicenceTypeID      = Row["LicenseTypeId"].ToString();
            mJurisdictionID     = Row["JurisdictionID"].ToString();
            mServerID           = Row["ServerID"].ToString();

            mVenueType.Value    = Row["VenueType"].ToString();
            mVenueState.Value   = Row["VenueState"].ToString();
            mLicensee.Value     = Row["Licensee"].ToString();
            mLicenceType.Value  = Row["LicenseType"].ToString();
            mLicenceNo.Value    = Row["LicenseNo"].ToString();
            mJurisdiction.Value = Row["Jurisdiction"].ToString();
            mServer.Value       = Row["Server"].ToString();
            mPeriodStart.Value  = Row["PeriodStart"].ToString();
            mInterval.Value     = Row["Interval"].ToString();

            DateTime dt;
            if (DateTime.TryParse(mPeriodStart.Value, out dt))
                mDayStart = dt.ToString("HH:mm tt");

        }

        public bool Update()
        {
            string Params = "";
            if (!mCompany.NewEntry) Params = "I~S~VenuePK~" + mCompany.CompanyPK + ";";
            Params += "I~S~VenueType~"    + mVenueTypeID + ";";
            Params += "I~S~VenueState~"   + mVenueStateID + ";";

            SQLServer DB = new SQLServer(mConnectionString);
            mCompany.CompanyPK = DB.ExecLookup("loc.mod_Company_Venue", Params + getParams()).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null; 

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mCompany.NewEntry)
                {
                    mVenueType.Update();
                    mVenueState.Update();
                    mLicensee.Update();
                    mLicenceType.Update();
                    mLicenceNo.Update();
                    mJurisdiction.Update();
                    mServer.Update();
                    mPeriodStart.Update();
                    mInterval.Update();
                }
                Open(LocalUI: mCompany.CompanyPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Venue", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mLicenseeID))        Params += "I~S~Licensee~"     + mLicenseeID + ";";
            if (!String.IsNullOrEmpty(mLicenceTypeID))     Params += "I~S~LicenseType~"  + mLicenceTypeID + ";";
            if (!String.IsNullOrEmpty(mLicenceNo.Value))   Params += "I~S~LicenseNo~"    + mLicenceNo.Value + ";";
            if (!String.IsNullOrEmpty(mJurisdictionID))    Params += "I~S~Jurisdiction~" + mJurisdictionID + ";";
            if (!String.IsNullOrEmpty(mServerID))          Params += "I~S~Server~"       + mServerID + ";";
            if (!String.IsNullOrEmpty(mPeriodStart.Value)) Params += "I~S~PeriodStart~"  + mPeriodStart.Value + ";";
            if (!String.IsNullOrEmpty(mInterval.Value))    Params += "I~S~Interval~"     + mInterval.Value + ";";
            Params += mCompany.getParams();
            return Params;
        }

        //public System.Drawing.Image VenueLogo
        //{
        //    get { return mVenueLogo; }
        //    set
        //    {
        //        mVenueLogo = value;
        //        mHasLogo = true;
        //    }
        //}
        //public bool HasLogo
        //{
        //    get { return mHasLogo; }
        //    set { mHasLogo = value; }
        //}

        public bool Changed
        {
            get { return mVenueType.Changed() || mVenueState.Changed() || mLicensee.Changed() || mLicenceType.Changed() || mLicenceNo.Changed() || mJurisdiction.Changed() || mServer.Changed() || mPeriodStart.Changed() || mInterval.Changed(); }
        }


        public void Reset()
        {
            Reset(VenueType);
        }

        public void Reset(string VenueType)
        {
            mCompany.Reset();

            mVenueType.Reset();
            mVenueState.Reset();
            mLicensee.Reset();
            mLicenceType.Reset();
            mLicenceNo.Reset();
            mJurisdiction.Reset();
            mServer.Reset();
            mPeriodStart.Reset();
            mInterval.Reset();

            mCompany.Description = "The Venue";
            mPeriodStart.Value = "Dec 1, 2000 4:00 AM";
            DateTime dt;
            if (DateTime.TryParse(mPeriodStart.Value, out dt))
                mDayStart = dt.ToString("HH:mm tt");

            if (!mLastVenueType.Equals(VenueType))
            {
                VenueTypeID = EnumCache.Instance.getTypeId("Venue", VenueType);
                mLastVenueType = VenueType;
            }
            if (String.IsNullOrEmpty(VenueStateID)) VenueStateID = EnumCache.Instance.getStateId ("Company", "Active");
        }

        public override string ToString()
        {
            return mCompany.Description;
        }
    }
}
